using HDEMS.Application.DTOs;

namespace HDEMS.Application.Interfaces;

/// <summary>
/// 认证服务接口
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 用户登录
    /// </summary>
    Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);

    /// <summary>
    /// 刷新Token
    /// </summary>
    Task<ApiResponse<string>> RefreshTokenAsync(string token);

    /// <summary>
    /// 登出
    /// </summary>
    Task<ApiResponse> LogoutAsync();

    /// <summary>
    /// 修改密码
    /// </summary>
    Task<ApiResponse> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);

    /// <summary>
    /// 重置管理员密码
    /// </summary>
    Task<ApiResponse> ResetAdminPasswordAsync(string newPassword);
}
