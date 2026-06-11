using FptConnect.Application.Common;
using FptConnect.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FptConnect.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;

    public AuthController(AppDbContext db, IPasswordHasher hasher, IJwtTokenService jwt)
    {
        _db = db; _hasher = hasher; _jwt = jwt;
    }

    public record LoginRequest(string Identifier, string Password);

    /// <summary>API-001 login (rút gọn Sprint 0): generic error, không tiết lộ account tồn tại (TC-002).</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var email = req.Identifier.Trim().ToLowerInvariant();
        var user = await _db.Users
            .Include(u => u.Roles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.EmailNormalized == email || u.EmployeeCode == req.Identifier);

        if (user is null || user.Status != "Active" || !_hasher.Verify(req.Password, user.PasswordHash))
            return Unauthorized(new { code = "INVALID_CREDENTIALS", title = "Sai thông tin đăng nhập" });

        user.LastLoginAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var roles = user.Roles.Select(r => r.Role.Code).ToList();
        var token = _jwt.CreateAccessToken(user, roles);
        return Ok(new
        {
            data = new
            {
                accessToken = token,
                user = new { id = user.PublicId, name = user.FullName, email = user.EmailNormalized, roles }
            },
            meta = new { timestampUtc = DateTime.UtcNow }
        });
    }
}
