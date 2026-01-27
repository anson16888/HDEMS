using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace HDEMS.Infrastructure.Contexts;

/// <summary>
/// 审计上下文，用于获取当前用户信息
/// </summary>
public class AuditContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// 获取当前用户ID
    /// </summary>
    public string? CurrentUserId => _httpContextAccessor.HttpContext?.User
        .FindFirst(ClaimTypes.NameIdentifier)?.Value;

    /// <summary>
    /// 获取当前用户名
    /// </summary>
    public string? CurrentUserName => _httpContextAccessor.HttpContext?.User
        .FindFirst(ClaimTypes.Name)?.Value;

    /// <summary>
    /// 获取当前用户显示名称
    /// </summary>
    public string? CurrentUserDisplayName => CurrentUserName ?? CurrentUserId;

    /// <summary>
    /// 获取当前用户所属医院ID
    /// </summary>
    public Guid? CurrentHospitalId
    {
        get
        {
            var hospitalIdStr = _httpContextAccessor.HttpContext?.User
                .FindFirst("HospitalId")?.Value;
            if (Guid.TryParse(hospitalIdStr, out var hospitalId))
            {
                return hospitalId;
            }
            return null;
        }
    }
}
