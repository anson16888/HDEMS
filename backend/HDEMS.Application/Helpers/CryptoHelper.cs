using System.Security.Cryptography;
using System.Text;

namespace HDEMS.Application.Helpers;

/// <summary>
/// 加密辅助类
/// </summary>
public static class CryptoHelper
{
    /// <summary>
    /// 计算SHA256哈希值
    /// </summary>
    public static string ComputeSHA256Hash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hash).ToLower();
    }

    /// <summary>
    /// 计算MD5哈希值
    /// </summary>
    public static string ComputeMD5Hash(string input)
    {
        using var md5 = MD5.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = md5.ComputeHash(bytes);
        return Convert.ToHexString(hash).ToLower();
    }

    /// <summary>
    /// 生成随机字符串
    /// </summary>
    public static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    /// <summary>
    /// 生成UUID
    /// </summary>
    public static Guid GenerateGuid()
    {
        return Guid.NewGuid();
    }

    /// <summary>
    /// 生成短UUID（不含横线）
    /// </summary>
    public static string GenerateShortGuid()
    {
        return Guid.NewGuid().ToString("N");
    }
}
