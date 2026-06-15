namespace FptConnect.Domain.Entities;

/// <summary>
/// Audit append-only, hash-chained (Bible DB-32). Không sửa/xóa qua ứng dụng.
/// PII phải được mask trước khi ghi (không lưu token/password/phone đầy đủ).
/// </summary>
public class AuditLog
{
    public long Id { get; set; }
    public long? ActorUserId { get; set; }
    public string Action { get; set; } = default!;          // vd: AUTH_LOGIN_SUCCESS
    public string ResourceType { get; set; } = default!;    // vd: Session
    public string? ResourceId { get; set; }
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    public string? TraceId { get; set; }
    public byte[]? IpHash { get; set; }
    public string? DataJson { get; set; }                   // metadata đã mask
    public byte[]? PrevHash { get; set; }
    public byte[] EntryHash { get; set; } = default!;
}
