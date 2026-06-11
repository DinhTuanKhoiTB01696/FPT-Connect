using FptConnect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FptConnect.Application.Common;

/// <summary>Port tới persistence (Clean Architecture: Application không biết EF cụ thể).</summary>
public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<Customer> Customers { get; }
    DbSet<CustomerStatusHistory> CustomerStatusHistory { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

public interface IJwtTokenService
{
    string CreateAccessToken(User user, IEnumerable<string> roles);
}

public interface ICurrentUser
{
    long? UserId { get; }
}
