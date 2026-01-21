using System.ComponentModel;

namespace HDEMS.Application.Extensions;

/// <summary>
/// 枚举扩展方法
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// 获取枚举的Description特性值
    /// </summary>
    public static string GetDescription<T>(this T value) where T : Enum
    {
        var field = value.GetType().GetField(value.ToString());
        if (field == null)
            return value.ToString();

        var attribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false)
                            .FirstOrDefault() as DescriptionAttribute;

        return attribute?.Description ?? value.ToString();
    }

    /// <summary>
    /// 将枚举转换为列表（用于下拉框等）
    /// </summary>
    public static List<EnumItem> ToEnumList<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T))
                   .Cast<T>()
                   .Select(e => new EnumItem
                   {
                       Value = Convert.ToInt32(e),
                       Text = e.GetDescription(),
                       Name = e.ToString()
                   })
                   .ToList();
    }

    /// <summary>
    /// 枚举项
    /// </summary>
    public class EnumItem
    {
        public int Value { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
