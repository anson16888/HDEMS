using System.ComponentModel;

namespace HDEMS.Domain.Enums;

/// <summary>
/// 物资类型
/// </summary>
public enum MaterialType
{
    /// <summary>
    /// 食品类
    /// </summary>
    [Description("食品类")]
    Food = 1,

    /// <summary>
    /// 医疗用品
    /// </summary>
    [Description("医疗用品")]
    Medical = 2,

    /// <summary>
    /// 救援设备
    /// </summary>
    [Description("救援设备")]
    Equipment = 3,

    /// <summary>
    /// 衣物类
    /// </summary>
    [Description("衣物类")]
    Clothing = 4,

    /// <summary>
    /// 其他
    /// </summary>
    [Description("其他")]
    Other = 5
}
