using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace FptConnect.IntegrationTests;

/// <summary>Khởi động API in-memory và kiểm tra root redirect tới Swagger (chưa có nghiệp vụ).</summary>
public class ApiSmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public ApiSmokeTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task Root_redirects_to_swagger()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var res = await client.GetAsync("/");
        res.StatusCode.Should().Be(HttpStatusCode.Redirect);
        res.Headers.Location!.ToString().Should().Contain("swagger");
    }
}
