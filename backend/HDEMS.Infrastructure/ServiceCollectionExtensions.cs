using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using HDEMS.Infrastructure.Configuration;
using HDEMS.Infrastructure.Services;
using HDEMS.Infrastructure.Contexts;

namespace HDEMS.Infrastructure;

/// <summary>
/// 基础设施服务扩展
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加基础设施服务
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 注册配置
        services.Configure<JwtConfig>(options =>
            configuration.GetSection(JwtConfig.SectionName).Bind(options));
        services.Configure<DatabaseConfig>(options =>
            configuration.GetSection(DatabaseConfig.SectionName).Bind(options));
        services.Configure<SystemConfig>(options =>
            configuration.GetSection(SystemConfig.SectionName).Bind(options));

        // 注册 HTTP 访问器
        services.AddHttpContextAccessor();

        // 注册服务
        services.AddSingleton<JwtService>();
        services.AddScoped<DataSeeder>();
        services.AddScoped<PasswordService>();
        services.AddScoped<AuditContext>();

        return services;
    }
}
