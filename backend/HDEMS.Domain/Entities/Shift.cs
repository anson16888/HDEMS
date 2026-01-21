using FreeSql.DataAnnotations;

namespace HDEMS.Domain.Entities;

/// <summary>
/// 班次实体
/// </summary>
[Table(Name = "t_shift")]
[Index("idx_shift_code", "ShiftCode", true)]
public class Shift : BaseEntity
{
    /// <summary>
    /// 班次编码
    /// </summary>
    [Column(StringLength = 50, IsNullable = false)]
    public string ShiftCode { get; set; } = string.Empty;

    /// <summary>
    /// 班次名称
    /// </summary>
    [Column(StringLength = 50, IsNullable = false)]
    public string ShiftName { get; set; } = string.Empty;

    /// <summary>
    /// 时间段（可选）
    /// </summary>
    [Column(StringLength = 50)]
    public string? TimeRange { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    [Column(Position = 5)]
    public int SortOrder { get; set; }
}
