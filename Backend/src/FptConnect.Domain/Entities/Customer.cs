using FptConnect.Domain.Common;

namespace FptConnect.Domain.Entities;

/// <summary>Lead/customer aggregate (Bible DB-11, rút gọn cho Sprint 0).</summary>
public class Customer : BaseEntity
{
    public string FullName { get; set; } = default!;
    public string? PhoneE164 { get; set; }
    public byte[]? PhoneHash { get; set; }
    public string? Email { get; set; }
    public string StatusCode { get; set; } = "New";
    public string? SourceCode { get; set; }
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public long? OwnerUserId { get; set; }

    public ICollection<CustomerStatusHistory> StatusHistory { get; set; } = new List<CustomerStatusHistory>();
}

/// <summary>Lịch sử pipeline bất biến (Bible DB-12).</summary>
public class CustomerStatusHistory
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;
    public string FromStatus { get; set; } = default!;
    public string ToStatus { get; set; } = default!;
    public string? ReasonText { get; set; }
    public long? ChangedBy { get; set; }
    public DateTime ChangedAtUtc { get; set; } = DateTime.UtcNow;
}
