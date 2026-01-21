using System.ComponentModel;

namespace HDEMS.Domain.Enums;

/// <summary>
/// 用户状态
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// 启用
    /// </summary>
    [Description("启用")]
    Active = 1,

    /// <summary>
    /// 禁用
    /// </summary>
    [Description("禁用")]
    Inactive = 2,

    /// <summary>
    /// 锁定
    /// </summary>
    [Description("锁定")]
    Locked = 3
}
