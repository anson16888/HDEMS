using BCrypt.Net;

namespace HDEMS.Infrastructure.Services;

/// <summary>
/// 密码服务
/// </summary>
public class PasswordService
{
    /// <summary>
    /// 哈希密码
    /// </summary>
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// 验证密码
    /// </summary>
    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    /// <summary>
    /// 生成随机密码
    /// </summary>
    public string GenerateRandomPassword(int length = 8)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
