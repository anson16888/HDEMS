using FreeSql.DataAnnotations;

namespace HDEMS.Domain.Entities;

/// <summary>
/// 医院配置表
/// </summary>
[Table(Name = "t_hospital_config")]
public class HospitalConfig
{
    /// <summary>
    /// 主键ID
    /// </summary>
    [Column(IsPrimary = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 医院名称
    /// </summary>
    [Column(StringLength = 100)]
    public string HospitalName { get; set; } = string.Empty;

    /// <summary>
    /// 医院电话
    /// </summary>
    [Column(StringLength = 20)]
    public string? HospitalPhone { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
