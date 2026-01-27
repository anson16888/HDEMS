namespace HDEMS.Infrastructure.Configuration;

/// <summary>
/// 系统配置
/// </summary>
public class SystemConfig
{
    public const string SectionName = "SystemConfig";

    public string? SystemInitKey { get; set; }
    public string? DefaultAdminPassword { get; set; }
    public string? HospitalName { get; set; }
    public string? SystemLevel { get; set; }
    /// <summary>
    /// 系统组织名称（当hospitalId为Guid.Empty时使用）
    /// </summary>
    public string? SystemOrgName { get; set; }
}
