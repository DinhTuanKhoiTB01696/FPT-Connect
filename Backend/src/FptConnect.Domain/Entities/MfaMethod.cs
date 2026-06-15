using FptConnect.Domain.Common;

namespace FptConnect.Domain.Entities;

/// <summary>
/// Phương thức MFA (Bible DB-10). TOTP secret mã hoá; recovery code chỉ lưu hash.
/// </summary>
public class MfaMethod : BaseEntity
{
    public long UserId { get; set; }
    public string Type { get; set; } = "TOTP";
    public byte[] SecretEncrypted { get; set; } = default!;
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAtUtc { get; set; }
    /// <summary>Danh sách hash recovery code, mỗi dòng một hash (BCrypt).</summary>
    public string? RecoveryCodeHashes { get; set; }
}
