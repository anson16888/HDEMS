using FreeSql.DataAnnotations;
using HDEMS.Domain.Enums;

namespace HDEMS.Domain.Entities;

/// <summary>
/// 物资实体
/// </summary>
[Table(Name = "t_material")]
[Index("idx_material_code", "MaterialCode", true)]
[Index("idx_material_type", "MaterialType")]
[Index("idx_status", "Status")]
[Index("idx_hospital_id", "HospitalId")]
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
    /// 物资类型
    /// </summary>
    [Column]
    public MaterialType MaterialType { get; set; }

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
    /// 医院ID
    /// </summary>
    [Column(Position = 11)]
    public Guid HospitalId { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [Column(StringLength = -1)]
    public string? Remark { get; set; }

    /// <summary>
    /// 库存状态
    /// </summary>
    [Column(Position = 6, IsNullable = false)]
    public MaterialStatus Status { get; set; } = MaterialStatus.Normal;

    // 导航属性
    [Navigate(nameof(HospitalId))]
    public Hospital? Hospital { get; set; }
}
