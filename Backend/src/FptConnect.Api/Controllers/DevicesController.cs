using FptConnect.Application.Common;
using FptConnect.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FptConnect.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/devices")]
public class DevicesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ICurrentUser _current;
    private readonly IAuditWriter _audit;
    private readonly IClock _clock;

    public DevicesController(AppDbContext db, ICurrentUser current, IAuditWriter audit, IClock clock)
    {
        _db = db; _current = current; _audit = audit; _clock = clock;
    }

    public record RenameRequest(string Name);

    // API-093 — danh sách thiết bị của user (lastSeen masked theo ngày)
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var uid = _current.UserId!.Value;
        var items = await _db.Devices.AsNoTracking()
            .Where(d => d.UserId == uid && !d.IsDeleted)
            .OrderByDescending(d => d.LastSeenAtUtc)
            .Select(d => new
            {
                id = d.PublicId,
                d.Name,
                d.Platform,
                d.RiskStatus,
                lastSeenAtUtc = d.LastSeenAtUtc
            })
            .ToListAsync();
        return Ok(new { data = items, meta = new { count = items.Count } });
    }

    // API-094 — đổi tên thiết bị
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Rename(Guid id, [FromBody] RenameRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Name) || req.Name.Length > 120)
            return UnprocessableEntity(new { code = "VALIDATION_FAILED", errors = new { name = "NAME_INVALID" } });

        var uid = _current.UserId!.Value;
        var device = await _db.Devices.FirstOrDefaultAsync(d => d.PublicId == id && d.UserId == uid && !d.IsDeleted);
        if (device is null) return NotFound();

        device.Name = req.Name.Trim();
        device.UpdatedBy = uid;
        await _db.SaveChangesAsync();
        return Ok(new { data = new { id = device.PublicId, device.Name } });
    }

    // API-095 — thu hồi thiết bị: revoke mọi phiên của thiết bị + soft-delete (UC-053, BR-018, TC-309)
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Revoke(Guid id)
    {
        var uid = _current.UserId!.Value;
        var device = await _db.Devices.FirstOrDefaultAsync(d => d.PublicId == id && d.UserId == uid && !d.IsDeleted);
        if (device is null) return NotFound();

        var now = _clock.UtcNow;
        var sessions = await _db.Sessions
            .Where(s => s.DeviceId == device.Id && s.RevokedAtUtc == null)
            .ToListAsync();
        foreach (var s in sessions) s.Revoke(now, "device_revoked");

        device.IsDeleted = true;
        device.DeletedAtUtc = now;
        device.DeletedBy = uid;
        device.DeleteReason = "user_revoked_device";
        await _db.SaveChangesAsync();

        await _audit.WriteAsync("DEVICE_REVOKED", "Device", device.Id.ToString(), uid,
            new { revokedSessions = sessions.Count });
        return NoContent();
    }
}
