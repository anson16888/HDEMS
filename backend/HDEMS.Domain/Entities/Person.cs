using FreeSql.DataAnnotations;

namespace HDEMS.Domain.Entities;

/// <summary>
/// 人员实体（排班用人员基础信息）
/// </summary>
[Table(Name = "t_person")]
[Index("idx_person_code", "PersonCode", true)]
public class Person : BaseEntity
{
    /// <summary>
    /// 人员编码
    /// </summary>
    [Column(StringLength = 50, IsNullable = false)]
    public string PersonCode { get; set; } = string.Empty;

    /// <summary>
    /// 姓名
    /// </summary>
    [Column(StringLength = 50, IsNullable = false)]
    public string PersonName { get; set; } = string.Empty;

    /// <summary>
    /// 手机号
    /// </summary>
    [Column(StringLength = 20)]
    public string? Phone { get; set; }

    /// <summary>
    /// 科室ID
    /// </summary>
    [Column(Position = 5)]
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// 职级ID
    /// </summary>
    [Column(Position = 6)]
    public Guid? RankId { get; set; }

    /// <summary>
    /// 职称ID
    /// </summary>
    [Column(Position = 7)]
    public Guid? TitleId { get; set; }

    /// <summary>
    /// 医院ID
    /// </summary>
    [Column(Position = 8)]
    public Guid? HospitalId { get; set; }

    // 导航属性
    [Navigate(nameof(HospitalId))]
    public Hospital? Hospital { get; set; }

    [Navigate(nameof(DepartmentId))]
    public Department? Department { get; set; }

    [Navigate(nameof(RankId))]
    public PersonRank? Rank { get; set; }

    [Navigate(nameof(TitleId))]
    public PersonTitle? Title { get; set; }
}
