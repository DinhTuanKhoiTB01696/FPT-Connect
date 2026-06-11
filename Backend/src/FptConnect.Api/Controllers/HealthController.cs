using FptConnect.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FptConnect.Api.Controllers;

[ApiController]
[Route("api/v1/health")]
public class HealthController : ControllerBase
{
    private readonly AppDbContext _db;
    public HealthController(AppDbContext db) => _db = db;

    /// <summary>Liveness — không phụ thuộc dependency ngoài (Bible API-088, TC-291).</summary>
    [HttpGet("live")]
    public IActionResult Live() => Ok(new { status = "alive", timeUtc = DateTime.UtcNow });

    /// <summary>Readiness — kiểm tra DB (Bible API-089, TC-292).</summary>
    [HttpGet("ready")]
    public async Task<IActionResult> Ready()
    {
        try
        {
            var ok = await _db.Database.CanConnectAsync();
            return ok ? Ok(new { status = "ready" }) : StatusCode(503, new { status = "db_unavailable" });
        }
        catch
        {
            return StatusCode(503, new { status = "db_unavailable" });
        }
    }
}
