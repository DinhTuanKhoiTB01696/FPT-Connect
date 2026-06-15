using FptConnect.Application.Auth;
using FptConnect.Application.Common;
using FptConnect.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FptConnect.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/sessions")]
public class SessionsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly SessionService _sessions;
    private readonly ICurrentUser _current;
    private readonly IAuditWriter _audit;

    public SessionsController(AppDbContext db, SessionService sessions, ICurrentUser current, IAuditWriter audit)
    {
        _db = db; _sessions = sessions; _current = current; _audit = audit;
    }

    // API-008 — danh sách phiên của chính user (masked)
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var uid = _current.UserId!.Value;
        var currentSid = _current.SessionId;
        var now = DateTime.UtcNow;
        var items = await _db.Sessions.AsNoTracking()
            .Where(s => s.UserId == uid)
            .OrderByDescending(s => s.CreatedAtUtc)
            .Select(s => new
            {
                id = s.PublicId,
                createdAtUtc = s.CreatedAtUtc,
                expiresAtUtc = s.ExpiresAtUtc,
                revokedAtUtc = s.RevokedAtUtc,
                isCurrent = s.Id == currentSid,
                isActive = s.RevokedAtUtc == null && s.ReplacedById == null && s.ExpiresAtUtc > now
            })
            .ToListAsync();
        return Ok(new { data = items, meta = new { count = items.Count } });
    }

    // API-009 — revoke một phiên (chỉ phiên của chính mình; anti-enumeration trả 404)
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Revoke(Guid id)
    {
        var uid = _current.UserId!.Value;
        var session = await _db.Sessions.FirstOrDefaultAsync(s => s.PublicId == id && s.UserId == uid);
        if (session is null) return NotFound();

        await _sessions.RevokeAsync(session.Id, uid, "user_revoked");
        await _audit.WriteAsync("SESSION_REVOKED", "Session", session.Id.ToString(), uid);
        return NoContent();
    }
}
