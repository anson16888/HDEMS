using FreeSql.DataAnnotations;

namespace HDEMS.Domain.Entities;

/// <summary>
/// 人员职称实体
/// </summary>
[Table(Name = "t_person_title")]
[Index("idx_title_code", "TitleCode", true)]
public class PersonTitle : BaseEntity
{
    /// <summary>
    /// 职称编码
    /// </summary>
    [Column(StringLength = 50, IsNullable = false)]
    public string TitleCode { get; set; } = string.Empty;

    /// <summary>
    /// 职称名称
    /// </summary>
    [Column(StringLength = 50, IsNullable = false)]
    public string TitleName { get; set; } = string.Empty;

    /// <summary>
    /// 职称等级
    /// </summary>
    [Column(StringLength = 20)]
    public string? TitleLevel { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    [Column(Position = 5)]
    public int SortOrder { get; set; }
}
