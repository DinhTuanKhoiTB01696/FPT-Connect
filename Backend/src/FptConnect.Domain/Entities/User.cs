using FptConnect.Domain.Common;

namespace FptConnect.Domain.Entities;

/// <summary>Danh tính người dùng (Bible DB-03).</summary>
public class User : BaseEntity
{
    public string EmployeeCode { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string EmailNormalized { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string Status { get; set; } = "Active";
    public DateTime? LastLoginAtUtc { get; set; }

    public ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
}

/// <summary>Role versioned (Bible DB-04).</summary>
public class Role : BaseEntity
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool IsSystem { get; set; }
    public int Version { get; set; } = 1;
}

/// <summary>Gán role cho user (Bible DB-06, rút gọn cho Sprint 0).</summary>
public class UserRole
{
    public long UserId { get; set; }
    public User User { get; set; } = default!;
    public long RoleId { get; set; }
    public Role Role { get; set; } = default!;
}
