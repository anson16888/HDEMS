using System.ComponentModel;

namespace HDEMS.Domain.Enums;

/// <summary>
/// 用户角色
/// </summary>
public enum UserRole
{
    /// <summary>
    /// 系统管理员
    /// </summary>
    [Description("系统管理员")]
    Admin = 1,

    /// <summary>
    /// 值班管理员
    /// </summary>
    [Description("值班管理员")]
    DutyAdmin = 2,

    /// <summary>
    /// 物资管理员
    /// </summary>
    [Description("物资管理员")]
    MaterialAdmin = 3,

    /// <summary>
    /// 管理人员
    /// </summary>
    [Description("管理人员")]
    Viewer = 4
}
