using FreeSql.DataAnnotations;

namespace HDEMS.Domain.Entities;

/// <summary>
/// 物资库存阈值配置
/// </summary>
[Table(Name = "t_material_threshold")]
public class MaterialThreshold
{
    /// <summary>
    /// 主键ID
    /// </summary>
    [Column(IsPrimary = true)]
    public Guid Id { get; set; }

    /// <summary>
    /// 物资类型ID
    /// </summary>
    [Column(IsNullable = false)]
    public Guid MaterialTypeId { get; set; }

    /// <summary>
    /// 物资类型导航属性
    /// </summary>
    [Navigate(nameof(MaterialTypeId))]
    public MaterialTypeDict? MaterialType { get; set; }

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

    /// <summary>
    /// 排序号
    /// </summary>
    [Column(IsNullable = false)]
    public int SortOrder { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [Column(StringLength = -1)]
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
