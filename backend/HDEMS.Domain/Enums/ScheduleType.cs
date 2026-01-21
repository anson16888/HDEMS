using System.ComponentModel;

namespace HDEMS.Domain.Enums;

/// <summary>
/// 排班类型
/// </summary>
public enum ScheduleType
{
    /// <summary>
    /// 局级行政排班
    /// </summary>
    [Description("局级行政排班")]
    Bureau = 1,

    /// <summary>
    /// 院级行政排班
    /// </summary>
    [Description("院级行政排班")]
    Hospital = 2,

    /// <summary>
    /// 院内主任排班
    /// </summary>
    [Description("院内主任排班")]
    Director = 3
}
