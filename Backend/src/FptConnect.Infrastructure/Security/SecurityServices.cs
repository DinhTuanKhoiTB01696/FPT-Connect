using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FptConnect.Application.Common;
using FptConnect.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OtpNet;

namespace FptConnect.Infrastructure.Security;

public class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}

public class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}

public class JwtTokenService : IJwtTokenService
{
    private const string MfaAudience = "fpt-connect-mfa-challenge";
    private readonly IConfiguration _config;
    public JwtTokenService(IConfiguration config) => _config = config;

    private (SymmetricSecurityKey key, IConfigurationSection jwt) Key()
    {
        var jwt = _config.GetSection("Jwt");
        return (new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)), jwt);
    }

    public string CreateAccessToken(User user, IEnumerable<string> roles, long sessionId)
    {
        var (key, jwt) = Key();
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.PublicId.ToString()),
            new("uid", user.Id.ToString()),
            new("sid", sessionId.ToString()),
            new(JwtRegisteredClaimNames.Email, user.EmailNormalized),
            new("name", user.FullName),
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"], audience: jwt["Audience"], claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(jwt["AccessTokenMinutes"] ?? "10")),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string CreateMfaChallengeToken(long userId)
    {
        var (key, jwt) = Key();
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"], audience: MfaAudience,
            claims: new[] { new Claim("uid", userId.ToString()), new Claim("purpose", "mfa") },
            expires: DateTime.UtcNow.AddMinutes(5), signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public long? ReadMfaChallenge(string challengeToken)
    {
        var (key, jwt) = Key();
        try
        {
            var principal = new JwtSecurityTokenHandler().ValidateToken(challengeToken, new TokenValidationParameters
            {
                ValidateIssuer = true, ValidIssuer = jwt["Issuer"],
                ValidateAudience = true, ValidAudience = MfaAudience,
                ValidateIssuerSigningKey = true, IssuerSigningKey = key,
                ValidateLifetime = true, ClockSkew = TimeSpan.FromSeconds(30)
            }, out _);
            var uid = principal.FindFirst("uid")?.Value;
            return long.TryParse(uid, out var id) ? id : null;
        }
        catch
        {
            return null;
        }
    }
}

public class TotpService : ITotpService
{
    public string GenerateSecret() => Base32Encoding.ToString(KeyGeneration.GenerateRandomKey(20));

    public string BuildOtpAuthUri(string secret, string accountName, string issuer = "FPT Connect")
    {
        var enc = Uri.EscapeDataString(issuer);
        var acc = Uri.EscapeDataString(accountName);
        return $"otpauth://totp/{enc}:{acc}?secret={secret}&issuer={enc}&algorithm=SHA1&digits=6&period=30";
    }

    public bool VerifyCode(string secret, string code)
    {
        if (string.IsNullOrWhiteSpace(code)) return false;
        var totp = new Totp(Base32Encoding.ToBytes(secret));
        // window ±1 step để bù lệch đồng hồ
        return totp.VerifyTotp(code.Trim(), out _, new VerificationWindow(previous: 1, future: 1));
    }

    public IReadOnlyList<string> GenerateRecoveryCodes(int count = 10)
    {
        var list = new List<string>(count);
        for (var i = 0; i < count; i++)
        {
            var raw = CryptoTokens.NewSecret(8).ToLowerInvariant().Replace("-", "").Replace("_", "");
            list.Add($"{raw[..4]}-{raw[4..8]}");
        }
        return list;
    }
}
