namespace HDEMS.Application.Extensions;

/// <summary>
/// 字符串扩展方法
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// 判断字符串是否为空或空白
    /// </summary>
    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// 判断字符串是否不为空且不为空白
    /// </summary>
    public static bool IsNotNullOrWhiteSpace(this string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// 截取字符串，超出部分用省略号替代
    /// </summary>
    public static string Truncate(this string? value, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            return value ?? string.Empty;

        return value.Substring(0, maxLength) + suffix;
    }

    /// <summary>
    /// 转换为Pascal命名
    /// </summary>
    public static string ToPascalCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        var words = value.Split(new[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);
        return string.Concat(words.Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower()));
    }

    /// <summary>
    /// 隐藏手机号中间四位
    /// </summary>
    public static string MaskPhone(this string? phone)
    {
        if (string.IsNullOrEmpty(phone) || phone.Length < 11)
            return phone ?? string.Empty;

        return phone.Substring(0, 3) + "****" + phone.Substring(7);
    }
}
