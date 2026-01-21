using FreeSql.DataAnnotations;
using HDEMS.Domain.Enums;

namespace HDEMS.Domain.Entities;

/// <summary>
/// 物资库存阈值配置
/// </summary>
[Table(Name = "t_material_threshold")]
public class MaterialThreshold : BaseEntity
{
    /// <summary>
    /// 物资类型
    /// </summary>
    [Column(IsNullable = false)]
    public MaterialType MaterialType { get; set; }

    /// <summary>
    /// 预警阈值
    /// </summary>
    [Column(IsNullable = false)]
    public int Threshold { get; set; } = 5;

    /// <summary>
    /// 启用状态
    /// </summary>
    [Column(IsNullable = false)]
    public bool IsEnabled { get; set; } = true;
}
