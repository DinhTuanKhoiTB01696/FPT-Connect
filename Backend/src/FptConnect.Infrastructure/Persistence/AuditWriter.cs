using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FptConnect.Application.Common;
using FptConnect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FptConnect.Infrastructure.Persistence;

/// <summary>
/// Ghi audit append-only, hash-chained (Bible DB-32). Mỗi entry: EntryHash = SHA256(PrevHash || payload).
/// Lưu ý: dữ liệu truyền vào phải đã mask PII (không token/password/phone đầy đủ).
/// </summary>
public class AuditWriter : IAuditWriter
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public AuditWriter(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task WriteAsync(string action, string resourceType, string? resourceId,
        long? actorUserId, object? data = null, byte[]? ipHash = null, CancellationToken ct = default)
    {
        var prev = await _db.AuditLogs
            .OrderByDescending(a => a.Id)
            .Select(a => a.EntryHash)
            .FirstOrDefaultAsync(ct);

        var occurred = _clock.UtcNow;
        var dataJson = data is null ? null : JsonSerializer.Serialize(data);

        var payload = $"{action}|{resourceType}|{resourceId}|{actorUserId}|{occurred:O}|{dataJson}";
        var entryHash = ComputeHash(prev, payload);

        _db.AuditLogs.Add(new AuditLog
        {
            Action = action,
            ResourceType = resourceType,
            ResourceId = resourceId,
            ActorUserId = actorUserId,
            OccurredAtUtc = occurred,
            TraceId = Activity.Current?.Id,
            IpHash = ipHash,
            DataJson = dataJson,
            PrevHash = prev,
            EntryHash = entryHash
        });
        await _db.SaveChangesAsync(ct);
    }

    private static byte[] ComputeHash(byte[]? prev, string payload)
    {
        var buffer = new List<byte>();
        if (prev is not null) buffer.AddRange(prev);
        buffer.AddRange(Encoding.UTF8.GetBytes(payload));
        return SHA256.HashData(buffer.ToArray());
    }
}
