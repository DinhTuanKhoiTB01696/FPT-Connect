namespace FptConnect.Domain.Common;

/// <summary>
/// Cột chuẩn cho mọi aggregate theo Project Bible 6.1: audit + soft-delete có thể truy vết.
/// </summary>
public abstract class BaseEntity
{
    public long Id { get; set; }
    public Guid PublicId { get; set; } = Guid.NewGuid();

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public long? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public long? UpdatedBy { get; set; }

    // Soft-delete có thể truy vết (Bible 6.1 / FR-026 / BR-017)
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public long? DeletedBy { get; set; }
    public string? DeleteReason { get; set; }

    // Optimistic concurrency
    public byte[]? RowVersion { get; set; }
}
