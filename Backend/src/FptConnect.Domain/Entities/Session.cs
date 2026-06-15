using FptConnect.Domain.Common;

namespace FptConnect.Domain.Entities;

/// <summary>
/// Phiên refresh-token (Bible DB-08). Refresh rotation + reuse detection theo TokenFamilyId.
/// Chỉ lưu HASH của refresh token, không lưu bản rõ.
/// </summary>
public class Session : BaseEntity
{
    public long UserId { get; set; }
    public Guid TokenFamilyId { get; set; } = Guid.NewGuid();
    public byte[] RefreshTokenHash { get; set; } = default!;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
    public string? RevokedReason { get; set; }
    public long? ReplacedById { get; set; }
    public byte[]? IpHash { get; set; }
    public byte[]? UserAgentHash { get; set; }
    public long? DeviceId { get; set; }

    /// <summary>Phiên còn dùng được: chưa revoke, chưa bị thay thế (rotation), chưa hết hạn.</summary>
    public bool IsActive(DateTime nowUtc) =>
        RevokedAtUtc is null && ReplacedById is null && ExpiresAtUtc > nowUtc;

    public void Revoke(DateTime nowUtc, string reason)
    {
        if (RevokedAtUtc is not null) return; // idempotent
        RevokedAtUtc = nowUtc;
        RevokedReason = reason;
    }
}
