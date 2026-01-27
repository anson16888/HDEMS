using FreeSql.DataAnnotations;
using HDEMS.Domain.Enums;

namespace HDEMS.Domain.Entities;

/// <summary>
/// 排班实体
/// </summary>
[Table(Name = "t_schedule")]
[Index("idx_schedule_date", "ScheduleDate")]
[Index("idx_schedule_type", "ScheduleType")]
public class Schedule : BaseEntity
{
    /// <summary>
    /// 排班日期
    /// </summary>
    [Column(Position = 2, IsNullable = false)]
    public DateTime ScheduleDate { get; set; }

    /// <summary>
    /// 排班类型
    /// </summary>
    [Column(Position = 3, IsNullable = false)]
    public ScheduleType ScheduleType { get; set; }

    /// <summary>
    /// 班次ID
    /// </summary>
    [Column(Position = 4, IsNullable = false)]
    public Guid ShiftId { get; set; }

    /// <summary>
    /// 人员ID（可选，关联人员表）
    /// </summary>
    [Column(Position = 5)]
    public Guid? PersonId { get; set; }

    /// <summary>
    /// 人员姓名
    /// </summary>
    [Column(StringLength = 50, Position = 6, IsNullable = false)]
    public string PersonName { get; set; } = string.Empty;

    /// <summary>
    /// 联系电话
    /// </summary>
    [Column(StringLength = 20, Position = 7, IsNullable = false)]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 职级ID
    /// </summary>
    [Column(Position = 8)]
    public Guid? RankId { get; set; }

    /// <summary>
    /// 科室ID
    /// </summary>
    [Column(Position = 9)]
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// 职称ID
    /// </summary>
    [Column(Position = 10)]
    public Guid? TitleId { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [Column(StringLength = 200)]
    public string? Remark { get; set; }

    /// <summary>
    /// 所属医院ID
    /// </summary>
    [Column(Position = 11)]
    public Guid? HospitalId { get; set; }

    // 导航属性
    [Navigate(nameof(ShiftId))]
    public Shift? Shift { get; set; }

    [Navigate(nameof(PersonId))]
    public Person? Person { get; set; }

    [Navigate(nameof(RankId))]
    public PersonRank? Rank { get; set; }

    [Navigate(nameof(DepartmentId))]
    public Department? Department { get; set; }

    [Navigate(nameof(TitleId))]
    public PersonTitle? Title { get; set; }

    [Navigate(nameof(HospitalId))]
    public Hospital? Hospital { get; set; }
}
