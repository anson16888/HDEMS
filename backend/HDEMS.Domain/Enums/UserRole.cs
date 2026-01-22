using System.ComponentModel;

namespace HDEMS.Domain.Enums;

/// <summary>
/// 用户角色
/// </summary>
public enum UserRole
{
    /// <summary>
    /// 系统管理员（拥有所有权限）
    /// </summary>
    [Description("系统管理员")]
    SYSTEM_ADMIN = 0,

    /// <summary>
    /// 物资管理员（管理物资相关功能）
    /// </summary>
    [Description("物资管理员")]
    MATERIAL_ADMIN = 1,

    /// <summary>
    /// 值班管理员（管理排班相关功能）
    /// </summary>
    [Description("值班管理员")]
    SCHEDULE_ADMIN = 2
}
