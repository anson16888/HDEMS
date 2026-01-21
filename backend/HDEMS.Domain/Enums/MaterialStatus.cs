using System.ComponentModel;

namespace HDEMS.Domain.Enums;

/// <summary>
/// 物资库存状态
/// </summary>
public enum MaterialStatus
{
    /// <summary>
    /// 正常
    /// </summary>
    [Description("正常")]
    Normal = 1,

    /// <summary>
    /// 库存偏低
    /// </summary>
    [Description("库存偏低")]
    Low = 2,

    /// <summary>
    /// 已耗尽
    /// </summary>
    [Description("已耗尽")]
    Out = 3,

    /// <summary>
    /// 已过期
    /// </summary>
    [Description("已过期")]
    Expired = 4,

    /// <summary>
    /// 即将过期
    /// </summary>
    [Description("即将过期")]
    ExpiringSoon = 5
}
