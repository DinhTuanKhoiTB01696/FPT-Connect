using FptConnect.Domain.Common;

namespace FptConnect.Domain.Entities;

/// <summary>Thiết bị của người dùng + push registration (Bible DB-09).</summary>
public class Device : BaseEntity
{
    public long UserId { get; set; }
    public byte[] DeviceKeyHash { get; set; } = default!;
    public string Name { get; set; } = "Thiết bị";
    public string? Platform { get; set; }
    public string RiskStatus { get; set; } = "Normal"; // Normal | Elevated | High
    public DateTime LastSeenAtUtc { get; set; } = DateTime.UtcNow;
}
