using FreeSql.DataAnnotations;

namespace HDEMS.Domain.Entities;

/// <summary>
/// 物资类型字典实体
/// </summary>
[Table(Name = "t_material_type_dict")]
[Index("idx_type_code", "TypeCode", true)]
public class MaterialTypeDict
{
    /// <summary>
    /// 主键ID
    /// </summary>
    [Column(IsPrimary = true)]
    public Guid Id { get; set; }

    /// <summary>
    /// 类型编码
    /// </summary>
    [Column(StringLength = 50, IsNullable = false)]
    public string TypeCode { get; set; } = string.Empty;

    /// <summary>
    /// 类型名称
    /// </summary>
    [Column(StringLength = 50, IsNullable = false)]
    public string TypeName { get; set; } = string.Empty;

    /// <summary>
    /// 显示颜色（用于前端展示）
    /// </summary>
    [Column(StringLength = 20)]
    public string? Color { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    [Column(IsNullable = false)]
    public int SortOrder { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [Column(IsNullable = false)]
    public bool IsEnabled { get; set; } = true;

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
