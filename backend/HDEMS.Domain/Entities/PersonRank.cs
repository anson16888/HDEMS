using FreeSql.DataAnnotations;

namespace HDEMS.Domain.Entities;

/// <summary>
/// 人员职级实体
/// </summary>
[Table(Name = "t_person_rank")]
[Index("idx_rank_code", "RankCode", true)]
public class PersonRank : BaseEntity
{
    /// <summary>
    /// 职级编码
    /// </summary>
    [Column(StringLength = 50, IsNullable = false)]
    public string RankCode { get; set; } = string.Empty;

    /// <summary>
    /// 职级名称
    /// </summary>
    [Column(StringLength = 50, IsNullable = false)]
    public string RankName { get; set; } = string.Empty;

    /// <summary>
    /// 职级分类（局级/院级/行政）
    /// </summary>
    [Column(StringLength = 20)]
    public string? RankCategory { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    [Column(Position = 5)]
    public int SortOrder { get; set; }
}
