using FreeSql.DataAnnotations;

namespace HDEMS.Domain.Entities;

/// <summary>
/// 物资实体
/// </summary>
[Table(Name = "t_material")]
[Index("idx_material_code", "MaterialCode", true)]
[Index("idx_material_type_id", "MaterialTypeId")]
public class Material : BaseEntity
{
    /// <summary>
    /// 物资编号
    /// </summary>
    [Column(StringLength = 50)]
    public string MaterialCode { get; set; } = string.Empty;

    /// <summary>
    /// 物资名称
    /// </summary>
    [Column(StringLength = 50, IsNullable = false)]
    public string MaterialName { get; set; } = string.Empty;

    /// <summary>
    /// 物资类型ID
    /// </summary>
    [Column(IsNullable = false)]
    public Guid MaterialTypeId { get; set; }

    /// <summary>
    /// 物资类型
    /// </summary>
    [Navigate(nameof(MaterialTypeId))]
    public MaterialTypeDict? MaterialType { get; set; }

    /// <summary>
    /// 物资规格
    /// </summary>
    [Column(StringLength = 100)]
    public string? Specification { get; set; }

    /// <summary>
    /// 库存数量
    /// </summary>
    [Column(Position = 5, IsNullable = false)]
    public decimal Quantity { get; set; }

    /// <summary>
    /// 单位
    /// </summary>
    [Column(StringLength = 20)]
    public string? Unit { get; set; }

    /// <summary>
    /// 生产日期
    /// </summary>
    [Column(Position = 8)]
    public DateTime? ProductionDate { get; set; }

    /// <summary>
    /// 质保期（月）
    /// </summary>
    [Column(Position = 9)]
    public int? ShelfLife { get; set; }

    /// <summary>
    /// 过期日期（系统自动计算）
    /// </summary>
    [Column(Position = 10)]
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// 存放位置
    /// </summary>
    [Column(StringLength = 100, IsNullable = false)]
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// 备注
    /// </summary>
    [Column(StringLength = -1)]
    public string? Remark { get; set; }

    /// <summary>
    /// 库存状态 (0-正常, 1-库存偏低, 2-已耗尽, 3-已过期, 4-即将过期)
    /// </summary>
    [Column(Position = 6, IsNullable = false)]
    public int Status { get; set; }
}
