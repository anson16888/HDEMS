namespace HDEMS.Infrastructure.Configuration;

/// <summary>
/// 数据库配置
/// </summary>
public class DatabaseConfig
{
    public const string SectionName = "DatabaseConfig";

    public string? DbType { get; set; }
    public bool? AutoSyncStructure { get; set; }
    public bool? AutoSeedData { get; set; }
}
