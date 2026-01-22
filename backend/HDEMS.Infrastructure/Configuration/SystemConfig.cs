namespace HDEMS.Infrastructure.Configuration;

/// <summary>
/// 系统配置
/// </summary>
public class SystemConfig
{
    public const string SectionName = "SystemConfig";

    public string? SystemInitKey { get; set; }
    public string? DefaultAdminPassword { get; set; }
}
