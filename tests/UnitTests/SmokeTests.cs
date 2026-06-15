using FluentAssertions;
using Xunit;

namespace FptConnect.UnitTests;

/// <summary>Smoke test xác nhận project structure build & chạy được (chưa có nghiệp vụ).</summary>
public class SmokeTests
{
    [Fact]
    public void Solution_structure_is_initialized()
    {
        true.Should().BeTrue();
    }
}
