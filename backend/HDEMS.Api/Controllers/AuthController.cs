using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HDEMS.Application.DTOs;
using HDEMS.Application.Interfaces;
using System.Security.Claims;

namespace HDEMS.Api.Controllers;

/// <summary>
/// 认证控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="request">登录请求</param>
    /// <returns>登录响应</returns>
    [HttpPost("login")]
    public async Task<ApiResponse<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        return await _authService.LoginAsync(request);
    }

    /// <summary>
    /// 刷新Token
    /// </summary>
    /// <param name="token">旧Token</param>
    /// <returns>新Token</returns>
    [HttpPost("refresh")]
    public async Task<ApiResponse<string>> Refresh([FromBody] string token)
    {
        return await _authService.RefreshTokenAsync(token);
    }

    /// <summary>
    /// 用户登出
    /// </summary>
    /// <returns>登出结果</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ApiResponse> Logout()
    {
        return await _authService.LogoutAsync();
    }

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    /// <returns>用户信息</returns>
    [HttpGet("current-user")]
    [Authorize]
    public ApiResponse<CurrentUser> GetCurrentUser()
    {
        var user = new CurrentUser
        {
            Id = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString()),
            Username = User.FindFirst(ClaimTypes.Name)?.Value ?? "",
            Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList(),
            HospitalId = Guid.TryParse(User.FindFirst("HospitalId")?.Value, out var hid) ? hid : null,
            IsCommissionUser = bool.Parse(User.FindFirst("IsCommissionUser")?.Value ?? "false")
        };

        return ApiResponse<CurrentUser>.Ok(user);
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    /// <param name="request">修改密码请求</param>
    /// <returns>操作结果</returns>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<ApiResponse> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        return await _authService.ChangePasswordAsync(userId, request);
    }

    /// <summary>
    /// 重置管理员密码
    /// </summary>
    /// <param name="newPassword">新密码</param>
    /// <returns>操作结果</returns>
    [HttpPost("reset-admin-password")]
    public async Task<ApiResponse> ResetAdminPassword([FromBody] string newPassword)
    {
        return await _authService.ResetAdminPasswordAsync(newPassword);
    }

    /// <summary>
    /// 当前用户信息
    /// </summary>
    public class CurrentUser
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        public Guid? HospitalId { get; set; }
        public bool IsCommissionUser { get; set; }
    }
}
