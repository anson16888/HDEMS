using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using HDEMS.Application.Interfaces;
using HDEMS.Application.Services;

namespace HDEMS.Application;

/// <summary>
/// 应用服务扩展
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加应用服务
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // 注册服务
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IMaterialService, MaterialService>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<IBasicDataService, BasicDataService>();
        services.AddScoped<IImportExportService, ImportExportService>();

        // AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));

        return services;
    }
}
