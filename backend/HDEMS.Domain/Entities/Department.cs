using FreeSql.DataAnnotations;

namespace HDEMS.Domain.Entities;

/// <summary>
/// 科室实体
/// </summary>
[Table(Name = "t_department")]
[Index("idx_dept_code", "DepartmentCode", true)]
public class Department : BaseEntity
{
    /// <summary>
    /// 科室编码
    /// </summary>
    [Column(StringLength = 50, IsNullable = false)]
    public string DepartmentCode { get; set; } = string.Empty;

    /// <summary>
    /// 科室名称
    /// </summary>
    [Column(StringLength = 100, IsNullable = false)]
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>
    /// 科室类型
    /// </summary>
    [Column(StringLength = 20)]
    public string? DepartmentType { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    [Column(Position = 5)]
    public int SortOrder { get; set; }
}
