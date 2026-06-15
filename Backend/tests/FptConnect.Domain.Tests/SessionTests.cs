using FptConnect.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace FptConnect.Domain.Tests;

public class SessionTests
{
    private static Session NewSession(DateTime now) => new()
    {
        Id = 1,
        UserId = 10,
        RefreshTokenHash = new byte[32],
        ExpiresAtUtc = now.AddDays(30)
    };

    [Fact]
    public void Active_session_is_usable()
    {
        var now = DateTime.UtcNow;
        NewSession(now).IsActive(now).Should().BeTrue();
    }

    [Fact]
    public void Expired_session_is_not_active()
    {
        var now = DateTime.UtcNow;
        var s = NewSession(now);
        s.ExpiresAtUtc = now.AddSeconds(-1);
        s.IsActive(now).Should().BeFalse();
    }

    [Fact]
    public void Replaced_session_is_not_active_reuse_signal()
    {
        var now = DateTime.UtcNow;
        var s = NewSession(now);
        s.ReplacedById = 2; // đã rotation -> token cũ dùng lại = reuse
        s.IsActive(now).Should().BeFalse();
    }

    [Fact]
    public void Revoke_is_idempotent_and_sets_reason()
    {
        var now = DateTime.UtcNow;
        var s = NewSession(now);
        s.Revoke(now, "logout");
        var first = s.RevokedAtUtc;
        s.Revoke(now.AddMinutes(5), "logout_again");
        s.RevokedAtUtc.Should().Be(first);          // không ghi đè lần revoke đầu
        s.RevokedReason.Should().Be("logout");
        s.IsActive(now).Should().BeFalse();
    }
}
