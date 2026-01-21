namespace HDEMS.Infrastructure.Configuration;

/// <summary>
/// JWT 配置
/// </summary>
public class JwtConfig
{
    public const string SectionName = "JwtConfig";

    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 480;
    public int RefreshBeforeMinutes { get; set; } = 60;
}
