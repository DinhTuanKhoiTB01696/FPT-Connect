using System.Text;
using FptConnect.Application.Auth;
using FptConnect.Application.Common;
using FptConnect.Domain.Entities;
using FptConnect.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace FptConnect.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private static readonly string[] MfaRequiredRoles = { "ADMIN", "MANAGER" };

    private readonly AppDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;
    private readonly ITotpService _totp;
    private readonly IPasswordHasher _bcrypt; // dùng lại cho hash recovery code
    private readonly SessionService _sessions;
    private readonly IAuditWriter _audit;
    private readonly ICurrentUser _current;
    private readonly IDataProtectionProvider _dpp;

    public AuthController(AppDbContext db, IPasswordHasher hasher, IJwtTokenService jwt, ITotpService totp,
        SessionService sessions, IAuditWriter audit, ICurrentUser current, IDataProtectionProvider dpp)
    {
        _db = db; _hasher = hasher; _bcrypt = hasher; _jwt = jwt; _totp = totp;
        _sessions = sessions; _audit = audit; _current = current; _dpp = dpp;
    }

    private IDataProtector Protector => _dpp.CreateProtector("mfa.totp.secret");
    private string? Ip => HttpContext.Connection.RemoteIpAddress?.ToString();
    private string? Ua => Request.Headers.UserAgent.ToString();
    private byte[]? IpHash => Ip is null ? null : CryptoTokens.Sha256(Ip);

    public record LoginRequest(string Identifier, string Password);
    public record MfaVerifyRequest(string ChallengeToken, string Code);
    public record MfaConfirmRequest(string Code);
    public record LogoutRequest(string? Scope);

    // ---------------------------------------------------------------- API-001
    [HttpPost("login")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var email = (req.Identifier ?? "").Trim().ToLowerInvariant();
        var user = await _db.Users.Include(u => u.Roles).ThenInclude(r => r.Role)
            .FirstOrDefaultAsync(u => u.EmailNormalized == email || u.EmployeeCode == req.Identifier);

        // Generic error — không tiết lộ account tồn tại (TC-002)
        if (user is null || !_hasher.Verify(req.Password ?? "", user.PasswordHash))
        {
            await _audit.WriteAsync("AUTH_LOGIN_FAILED", "User", user?.PublicId.ToString(), user?.Id,
                new { reason = "invalid_credentials" }, IpHash);
            return Unauthorized(Problem401("INVALID_CREDENTIALS", "Sai thông tin đăng nhập"));
        }
        if (user.Status == "Locked")
            return StatusCode(403, Problem("ACCOUNT_LOCKED", "Tài khoản bị khóa", 403));
        if (user.Status != "Active")
            return StatusCode(403, Problem("ACCOUNT_INACTIVE", "Tài khoản không hoạt động", 403));

        var roles = user.Roles.Select(r => r.Role.Code).ToList();
        var privileged = roles.Any(r => MfaRequiredRoles.Contains(r));
        var verifiedMfa = await _db.MfaMethods
            .AnyAsync(m => m.UserId == user.Id && m.Type == "TOTP" && m.IsVerified);

        if (privileged && verifiedMfa)
        {
            await _audit.WriteAsync("AUTH_MFA_CHALLENGE", "User", user.PublicId.ToString(), user.Id, null, IpHash);
            return Ok(new { data = new { mfaRequired = true, challengeToken = _jwt.CreateMfaChallengeToken(user.Id) },
                meta = Meta() });
        }

        return await IssueTokensAsync(user, roles, mustEnrollMfa: privileged && !verifiedMfa);
    }

    // ---------------------------------------------------------------- API-002
    [HttpPost("mfa/verify")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> MfaVerify([FromBody] MfaVerifyRequest req)
    {
        var uid = _jwt.ReadMfaChallenge(req.ChallengeToken ?? "");
        if (uid is null)
            return Unauthorized(Problem401("MFA_INVALID", "Phiên xác thực MFA không hợp lệ hoặc hết hạn"));

        var user = await _db.Users.Include(u => u.Roles).ThenInclude(r => r.Role)
            .FirstOrDefaultAsync(u => u.Id == uid.Value);
        var mfa = await _db.MfaMethods.FirstOrDefaultAsync(m => m.UserId == uid && m.Type == "TOTP" && m.IsVerified);
        if (user is null || mfa is null)
            return Unauthorized(Problem401("MFA_INVALID", "Không tìm thấy phương thức MFA"));

        var secret = Unprotect(mfa.SecretEncrypted);
        var ok = _totp.VerifyCode(secret, req.Code ?? "") || TryConsumeRecoveryCode(mfa, req.Code ?? "");
        if (!ok)
        {
            await _audit.WriteAsync("AUTH_MFA_FAILED", "User", user.PublicId.ToString(), user.Id, null, IpHash);
            return Unauthorized(Problem401("MFA_INVALID", "Mã OTP không đúng"));
        }
        await _db.SaveChangesAsync(); // lưu trạng thái recovery code nếu vừa dùng

        var roles = user.Roles.Select(r => r.Role.Code).ToList();
        await _audit.WriteAsync("AUTH_MFA_SUCCESS", "User", user.PublicId.ToString(), user.Id, null, IpHash);
        return await IssueTokensAsync(user, roles, mustEnrollMfa: false);
    }

    // ---------------------------------------------------------------- API-003
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] Dictionary<string, string>? body)
    {
        body ??= new Dictionary<string, string>();
        body.TryGetValue("refreshToken", out var refreshToken);
        var result = await _sessions.RotateAsync(refreshToken ?? "", Ip, Ua);

        if (result.Outcome == RefreshOutcome.Reused)
        {
            await _audit.WriteAsync("AUTH_REFRESH_REUSE", "Session", null, result.UserId,
                new { action = "family_revoked" }, IpHash);
            return Unauthorized(Problem401("REFRESH_REUSE_DETECTED", "Token đã bị thu hồi"));
        }
        if (result.Outcome == RefreshOutcome.Invalid || result.Session is null)
            return Unauthorized(Problem401("REFRESH_INVALID", "Refresh token không hợp lệ"));

        var user = await _db.Users.Include(u => u.Roles).ThenInclude(r => r.Role)
            .FirstAsync(u => u.Id == result.Session.UserId);
        var roles = user.Roles.Select(r => r.Role.Code).ToList();
        var access = _jwt.CreateAccessToken(user, roles, result.Session.Id);
        await _audit.WriteAsync("AUTH_REFRESH_OK", "Session", result.Session.Id.ToString(), user.Id, null, IpHash);

        return Ok(new { data = new { accessToken = access, refreshToken = result.RawToken }, meta = Meta() });
    }

    // ---------------------------------------------------------------- API-006 (enroll)
    [Authorize]
    [HttpPost("mfa/enroll")]
    public async Task<IActionResult> MfaEnroll()
    {
        var uid = _current.UserId!.Value;
        var user = await _db.Users.FirstAsync(u => u.Id == uid);

        var secret = _totp.GenerateSecret();
        _db.MfaMethods.Add(new MfaMethod
        {
            UserId = uid, Type = "TOTP", IsVerified = false,
            SecretEncrypted = Protect(secret), CreatedBy = uid
        });
        await _db.SaveChangesAsync();
        await _audit.WriteAsync("AUTH_MFA_ENROLL_STARTED", "User", user.PublicId.ToString(), uid, null, IpHash);

        return Ok(new { data = new { secret, otpauthUri = _totp.BuildOtpAuthUri(secret, user.EmailNormalized) },
            meta = Meta() });
    }

    // ---------------------------------------------------------------- API-007 (confirm)
    [Authorize]
    [HttpPost("mfa/confirm")]
    public async Task<IActionResult> MfaConfirm([FromBody] MfaConfirmRequest req)
    {
        var uid = _current.UserId!.Value;
        var mfa = await _db.MfaMethods
            .Where(m => m.UserId == uid && m.Type == "TOTP" && !m.IsVerified)
            .OrderByDescending(m => m.Id).FirstOrDefaultAsync();
        if (mfa is null)
            return UnprocessableEntity(Problem("MFA_NOT_PENDING", "Không có MFA đang chờ xác nhận", 422));

        if (!_totp.VerifyCode(Unprotect(mfa.SecretEncrypted), req.Code ?? ""))
            return UnprocessableEntity(Problem("MFA_CODE_INVALID", "Mã OTP không đúng", 422));

        var codes = _totp.GenerateRecoveryCodes();
        mfa.IsVerified = true;
        mfa.VerifiedAtUtc = DateTime.UtcNow;
        mfa.RecoveryCodeHashes = string.Join('\n', codes.Select(c => _bcrypt.Hash(c)));
        await _db.SaveChangesAsync();
        await _audit.WriteAsync("AUTH_MFA_CONFIRMED", "User", null, uid, null, IpHash);

        // recovery codes hiển thị MỘT LẦN
        return Ok(new { data = new { recoveryCodes = codes }, meta = Meta() });
    }

    // ---------------------------------------------------------------- API-004
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest? req)
    {
        var uid = _current.UserId!.Value;
        if (string.Equals(req?.Scope, "all", StringComparison.OrdinalIgnoreCase))
        {
            await _sessions.RevokeAllForUserAsync(uid, "logout_all");
            await _audit.WriteAsync("AUTH_LOGOUT_ALL", "User", null, uid, null, IpHash);
        }
        else if (_current.SessionId is { } sid)
        {
            await _sessions.RevokeAsync(sid, uid, "logout");
            await _audit.WriteAsync("AUTH_LOGOUT", "Session", sid.ToString(), uid, null, IpHash);
        }
        return NoContent();
    }

    // ---------------------------------------------------------------- API-005
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var uid = _current.UserId!.Value;
        var user = await _db.Users.Include(u => u.Roles).ThenInclude(r => r.Role).FirstAsync(u => u.Id == uid);
        var hasMfa = await _db.MfaMethods.AnyAsync(m => m.UserId == uid && m.IsVerified);
        return Ok(new { data = new
        {
            id = user.PublicId, name = user.FullName, email = user.EmailNormalized,
            roles = user.Roles.Select(r => r.Role.Code), mfaEnabled = hasMfa
        }, meta = Meta() });
    }

    // ---------------------------------------------------------------- helpers
    private async Task<IActionResult> IssueTokensAsync(User user, List<string> roles, bool mustEnrollMfa)
    {
        var (session, raw) = await _sessions.CreateAsync(user.Id, Ip, Ua, null);
        user.LastLoginAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        var access = _jwt.CreateAccessToken(user, roles, session.Id);
        await _audit.WriteAsync("AUTH_LOGIN_SUCCESS", "Session", session.Id.ToString(), user.Id, null, IpHash);

        return Ok(new { data = new
        {
            accessToken = access,
            refreshToken = raw,
            mustEnrollMfa,
            user = new { id = user.PublicId, name = user.FullName, email = user.EmailNormalized, roles }
        }, meta = Meta() });
    }

    private bool TryConsumeRecoveryCode(MfaMethod mfa, string code)
    {
        if (string.IsNullOrWhiteSpace(mfa.RecoveryCodeHashes) || string.IsNullOrWhiteSpace(code)) return false;
        var hashes = mfa.RecoveryCodeHashes.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
        var idx = hashes.FindIndex(h => _bcrypt.Verify(code.Trim(), h));
        if (idx < 0) return false;
        hashes.RemoveAt(idx); // recovery code dùng một lần
        mfa.RecoveryCodeHashes = string.Join('\n', hashes);
        return true;
    }

    private byte[] Protect(string value) => Encoding.UTF8.GetBytes(Protector.Protect(value));
    private string Unprotect(byte[] bytes) => Protector.Unprotect(Encoding.UTF8.GetString(bytes));

    private object Meta() => new { timestampUtc = DateTime.UtcNow, traceId = HttpContext.TraceIdentifier };
    private object Problem(string code, string title, int status) =>
        new { type = $"https://fptconnect/errors/{code.ToLowerInvariant()}", title, status, code, traceId = HttpContext.TraceIdentifier };
    private object Problem401(string code, string title) => Problem(code, title, 401);
}
