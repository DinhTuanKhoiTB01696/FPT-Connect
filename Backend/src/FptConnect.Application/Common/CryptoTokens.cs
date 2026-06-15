using System.Security.Cryptography;

namespace FptConnect.Application.Common;

/// <summary>
/// Tiện ích sinh/băm token đối xứng cho refresh token và hash định danh (IP, device, user-agent).
/// Chỉ dùng BCL crypto, không phụ thuộc hạ tầng -> đặt ở Application để test được.
/// </summary>
public static class CryptoTokens
{
    /// <summary>Sinh secret ngẫu nhiên url-safe (mặc định 32 byte).</summary>
    public static string NewSecret(int bytes = 32)
    {
        var buffer = RandomNumberGenerator.GetBytes(bytes);
        return Convert.ToBase64String(buffer)
            .Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }

    /// <summary>SHA-256 của một chuỗi (dùng cho refresh token hash, IP/UA hash).</summary>
    public static byte[] Sha256(string value) =>
        SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(value));

    /// <summary>So sánh hash theo thời gian hằng định để tránh timing attack.</summary>
    public static bool HashEquals(byte[] a, byte[] b) =>
        CryptographicOperations.FixedTimeEquals(a, b);
}
