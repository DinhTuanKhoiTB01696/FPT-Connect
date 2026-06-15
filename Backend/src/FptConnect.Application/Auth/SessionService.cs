using FptConnect.Application.Common;
using FptConnect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FptConnect.Application.Auth;

public enum RefreshOutcome { Ok, Invalid, Reused }

public sealed record RefreshResult(RefreshOutcome Outcome, Session? Session = null, string? RawToken = null, long? UserId = null);

/// <summary>
/// Quản lý vòng đời refresh-session: tạo khi login, rotation khi refresh, phát hiện reuse (UC-002/003, TC-011..020).
/// Refresh token bản rõ chỉ tồn tại trong RAM và trả cho client; DB chỉ giữ SHA-256 hash.
/// </summary>
public sealed class SessionService
{
    private const int RefreshLifetimeDays = 30;
    private readonly IAppDbContext _db;
    private readonly IClock _clock;

    public SessionService(IAppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<(Session session, string rawToken)> CreateAsync(
        long userId, string? ip, string? userAgent, long? deviceId, CancellationToken ct = default)
    {
        var raw = CryptoTokens.NewSecret(32);
        var session = new Session
        {
            UserId = userId,
            TokenFamilyId = Guid.NewGuid(),
            RefreshTokenHash = CryptoTokens.Sha256(raw),
            ExpiresAtUtc = _clock.UtcNow.AddDays(RefreshLifetimeDays),
            IpHash = ip is null ? null : CryptoTokens.Sha256(ip),
            UserAgentHash = userAgent is null ? null : CryptoTokens.Sha256(userAgent),
            DeviceId = deviceId,
            CreatedBy = userId
        };
        _db.Sessions.Add(session);
        await _db.SaveChangesAsync(ct);
        return (session, raw);
    }

    public async Task<RefreshResult> RotateAsync(
        string rawRefreshToken, string? ip, string? userAgent, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(rawRefreshToken))
            return new RefreshResult(RefreshOutcome.Invalid);

        var hash = CryptoTokens.Sha256(rawRefreshToken);
        var current = await _db.Sessions.FirstOrDefaultAsync(s => s.RefreshTokenHash == hash, ct);
        if (current is null)
            return new RefreshResult(RefreshOutcome.Invalid);

        var now = _clock.UtcNow;

        // Reuse detection: token đã bị thay thế/revoke nhưng vẫn được dùng -> revoke cả family.
        if (!current.IsActive(now))
        {
            await RevokeFamilyAsync(current.TokenFamilyId, now, "reuse_detected", ct);
            return new RefreshResult(RefreshOutcome.Reused, UserId: current.UserId);
        }

        var raw = CryptoTokens.NewSecret(32);
        var next = new Session
        {
            UserId = current.UserId,
            TokenFamilyId = current.TokenFamilyId,
            RefreshTokenHash = CryptoTokens.Sha256(raw),
            ExpiresAtUtc = current.ExpiresAtUtc, // giữ absolute lifetime của family
            IpHash = ip is null ? null : CryptoTokens.Sha256(ip),
            UserAgentHash = userAgent is null ? null : CryptoTokens.Sha256(userAgent),
            DeviceId = current.DeviceId,
            CreatedBy = current.UserId
        };
        _db.Sessions.Add(next);
        await _db.SaveChangesAsync(ct);

        current.ReplacedById = next.Id; // current không còn active (rotation)
        await _db.SaveChangesAsync(ct);

        return new RefreshResult(RefreshOutcome.Ok, next, raw, current.UserId);
    }

    public async Task RevokeAsync(long sessionId, long actorUserId, string reason, CancellationToken ct = default)
    {
        var s = await _db.Sessions.FirstOrDefaultAsync(x => x.Id == sessionId, ct);
        if (s is null) return; // idempotent
        s.Revoke(_clock.UtcNow, reason);
        await _db.SaveChangesAsync(ct);
    }

    public async Task RevokeAllForUserAsync(long userId, string reason, CancellationToken ct = default)
    {
        var now = _clock.UtcNow;
        var sessions = await _db.Sessions
            .Where(s => s.UserId == userId && s.RevokedAtUtc == null)
            .ToListAsync(ct);
        foreach (var s in sessions) s.Revoke(now, reason);
        await _db.SaveChangesAsync(ct);
    }

    private async Task RevokeFamilyAsync(Guid familyId, DateTime now, string reason, CancellationToken ct)
    {
        var family = await _db.Sessions
            .Where(s => s.TokenFamilyId == familyId && s.RevokedAtUtc == null)
            .ToListAsync(ct);
        foreach (var s in family) s.Revoke(now, reason);
        await _db.SaveChangesAsync(ct);
    }
}
