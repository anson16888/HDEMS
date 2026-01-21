namespace HDEMS.Application.Extensions;

/// <summary>
/// 日期时间扩展方法
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// 获取日期所在周的第一天（周一）
    /// </summary>
    public static DateTime GetStartOfWeek(this DateTime date)
    {
        var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-diff).Date;
    }

    /// <summary>
    /// 获取日期所在周的最后一天（周日）
    /// </summary>
    public static DateTime GetEndOfWeek(this DateTime date)
    {
        return date.GetStartOfWeek().AddDays(6).AddDays(1).AddTicks(-1);
    }

    /// <summary>
    /// 获取日期所在月的第一天
    /// </summary>
    public static DateTime GetStartOfMonth(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1);
    }

    /// <summary>
    /// 获取日期所在月的最后一天
    /// </summary>
    public static DateTime GetEndOfMonth(this DateTime date)
    {
        return date.GetStartOfMonth().AddMonths(1).AddTicks(-1);
    }

    /// <summary>
    /// 判断是否是工作日
    /// </summary>
    public static bool IsWeekday(this DateTime date)
    {
        return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
    }

    /// <summary>
    /// 判断是否是周末
    /// </summary>
    public static bool IsWeekend(this DateTime date)
    {
        return !date.IsWeekday();
    }

    /// <summary>
    /// 格式化为中文日期
    /// </summary>
    public static string ToChineseDateString(this DateTime date)
    {
        return date.ToString("yyyy年MM月dd日");
    }

    /// <summary>
    /// 格式化为中文日期时间
    /// </summary>
    public static string ToChineseDateTimeString(this DateTime date)
    {
        return date.ToString("yyyy年MM月dd日 HH:mm:ss");
    }

    /// <summary>
    /// 获取两个日期之间的天数差（绝对值）
    /// </summary>
    public static int GetDaysDiff(this DateTime startDate, DateTime endDate)
    {
        return Math.Abs((endDate.Date - startDate.Date).Days);
    }

    /// <summary>
    /// 获取年龄
    /// </summary>
    public static int GetAge(this DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;

        if (birthDate > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}
