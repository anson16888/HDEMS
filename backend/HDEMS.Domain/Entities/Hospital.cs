using FreeSql.DataAnnotations;

namespace HDEMS.Domain.Entities;

/// <summary>
/// 医院实体
/// </summary>
[Table(Name = "t_hospital")]
[Index("idx_hospital_code", "HospitalCode", true)]
public class Hospital : BaseEntity
{
    /// <summary>
    /// 医院编码
    /// </summary>
    [Column(StringLength = 50, IsNullable = false)]
    public string HospitalCode { get; set; } = string.Empty;

    /// <summary>
    /// 医院名称
    /// </summary>
    [Column(StringLength = 100, IsNullable = false)]
    public string HospitalName { get; set; } = string.Empty;

    /// <summary>
    /// 医院简称
    /// </summary>
    [Column(StringLength = 50)]
    public string? ShortName { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [Column(StringLength = 200)]
    public string? Address { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [Column(StringLength = 20)]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// 负责人姓名
    /// </summary>
    [Column(StringLength = 50)]
    public string? ContactPerson { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    [Column(Position = 7)]
    public int SortOrder { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [Column(Position = 8)]
    public int Status { get; set; } = 1;
}
