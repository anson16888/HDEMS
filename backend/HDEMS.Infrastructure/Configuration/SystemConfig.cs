namespace HDEMS.Infrastructure.Configuration;

/// <summary>
/// 系统配置
/// </summary>
public class SystemConfig
{
    public const string SectionName = "SystemConfig";

    public string? SystemInitKey { get; set; }
    public string? DefaultAdminPassword { get; set; }
    /// <summary>
    /// 当前部署的医院名称（导入时自动使用）
    /// </summary>
    public string? HospitalName { get; set; }
    /// <summary>
    /// 系统权限级别：Hospital-医院级，Bureau-卫健委级
    /// </summary>
    public string SystemLevel { get; set; } = "Hospital";
}
