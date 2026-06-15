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

    // IAM — Sprint 1
    DbSet<Session> Sessions { get; }
    DbSet<Device> Devices { get; }
    DbSet<MfaMethod> MfaMethods { get; }
    DbSet<AuditLog> AuditLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

public interface IJwtTokenService
{
    /// <summary>Phát access token; sessionId nhúng claim "sid" phục vụ revoke/truy vết.</summary>
    string CreateAccessToken(User user, IEnumerable<string> roles, long sessionId);
    /// <summary>Phát challenge token ngắn hạn cho bước MFA (chưa cấp access token).</summary>
    string CreateMfaChallengeToken(long userId);
    /// <summary>Đọc userId từ challenge token hợp lệ; null nếu sai/het hạn.</summary>
    long? ReadMfaChallenge(string challengeToken);
}

/// <summary>Sinh và xác minh TOTP + recovery codes.</summary>
public interface ITotpService
{
    string GenerateSecret();
    string BuildOtpAuthUri(string secret, string accountName, string issuer = "FPT Connect");
    bool VerifyCode(string secret, string code);
    IReadOnlyList<string> GenerateRecoveryCodes(int count = 10);
}

/// <summary>Ghi audit bất biến, hash-chained, đã mask PII.</summary>
public interface IAuditWriter
{
    Task WriteAsync(string action, string resourceType, string? resourceId,
        long? actorUserId, object? data = null, byte[]? ipHash = null, CancellationToken ct = default);
}

public interface ICurrentUser
{
    long? UserId { get; }
    long? SessionId { get; }
}

public interface IClock
{
    DateTime UtcNow { get; }
}
