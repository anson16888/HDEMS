using FreeSql;
using HDEMS.Application.DTOs;
using HDEMS.Application.Extensions;
using HDEMS.Application.Interfaces;
using HDEMS.Domain.Entities;
using HDEMS.Domain.Enums;
using HDEMS.Infrastructure.Configuration;
using HDEMS.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace HDEMS.Application.Services;

/// <summary>
/// 认证服务实现
/// </summary>
public class AuthService : IAuthService
{
    private readonly IFreeSql _fsql;
    private readonly JwtService _jwtService;
    private readonly PasswordService _passwordService;
    private readonly ILogger<AuthService> _logger;
    private readonly SystemConfig _systemConfig;

    public AuthService(IFreeSql fsql, JwtService jwtService, PasswordService passwordService, ILogger<AuthService> logger, IOptions<SystemConfig> systemConfig)
    {
        _fsql = fsql;
        _jwtService = jwtService;
        _passwordService = passwordService;
        _logger = logger;
        _systemConfig = systemConfig.Value;
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
    {
        // 查找用户
        var user = await _fsql.Select<User>()
            .Include(u => u.Hospital)
            .Where(u => u.Username == request.Username)
            .FirstAsync();

        if (user == null)
        {
            return ApiResponse<LoginResponse>.Fail(401, "用户名或密码错误");
        }

        // 验证密码
        if (!_passwordService.VerifyPassword(request.Password, user.Password))
        {
            return ApiResponse<LoginResponse>.Fail(401, "用户名或密码错误");
        }

        // 检查用户状态
        if (user.Status == UserStatus.Inactive)
        {
            return ApiResponse<LoginResponse>.Fail(403, "用户已被禁用");
        }

        if (user.Status == UserStatus.Locked)
        {
            return ApiResponse<LoginResponse>.Fail(403, "用户已被锁定");
        }

        // 生成 Token
        var roleList = user.GetRoleList();
        var roles = roleList.Select(r => r.ToString()).ToList();
        var roleDescriptions = roleList.Select(r => r.GetDescription()).ToList();
        var token = _jwtService.GenerateToken(user.Id, user.Username, roles, user.HospitalId, user.IsCommissionUser);

        // 更新最后登录时间
        user.LastLoginAt = DateTime.Now;
        await _fsql.Update<User>()
            .Set(u => u.LastLoginAt, user.LastLoginAt)
            .Where(u => u.Id == user.Id)
            .ExecuteAffrowsAsync();

        var response = new LoginResponse
        {
            Token = token,
            UserInfo = new UserInfo
            {
                Id = user.Id,
                Username = user.Username,
                RealName = user.RealName,
                Phone = user.Phone,
                Department = user.Department,
                RoleStrings = roles,
                RoleDescriptions = roleDescriptions,
                HospitalId = user.HospitalId,
                HospitalName = user.Hospital?.HospitalName,
                SystemHospitalName = _systemConfig.HospitalName,
                SystemLevel = _systemConfig.SystemLevel,
                IsCommissionUser = user.IsCommissionUser
            }
        };

        return ApiResponse<LoginResponse>.Ok(response, "登录成功");
    }

    public async Task<ApiResponse<string>> RefreshTokenAsync(string token)
    {
        try
        {
            var newToken = _jwtService.RefreshToken(token);
            return ApiResponse<string>.Ok(newToken, "Token刷新成功");
        }
        catch (Exception ex)
        {
            return ApiResponse<string>.Fail(401, "Token刷新失败: " + ex.Message);
        }
    }

    public async Task<ApiResponse> LogoutAsync()
    {
        // 如果使用 Redis 存储 Token 黑名单，可以在这里处理
        return ApiResponse.Ok("登出成功");
    }

    public async Task<ApiResponse> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await _fsql.Select<User>().Where(u => u.Id == userId).FirstAsync();
        if (user == null)
        {
            return ApiResponse.Fail(404, "用户不存在");
        }

        // 验证旧密码
        if (!_passwordService.VerifyPassword(request.OldPassword, user.Password))
        {
            return ApiResponse.Fail(400, "旧密码不正确");
        }

        // 更新密码
        user.Password = _passwordService.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.Now;

        await _fsql.Update<User>()
            .SetSource(user)
            .ExecuteAffrowsAsync();

        return ApiResponse.Ok("密码修改成功");
    }

    public async Task<ApiResponse> ResetAdminPasswordAsync(string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword))
        {
            return ApiResponse.Fail(400, "密码不能为空");
        }

        var adminUser = await _fsql.Select<User>()
            .Where(u => u.Username == "admin")
            .FirstAsync();

        if (adminUser == null)
        {
            // 创建默认管理员
            adminUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                Password = _passwordService.HashPassword(newPassword),
                RealName = "系统管理员",
                Phone = "13800000000",
                Department = "信息科",
                Status = UserStatus.Active,
                IsCommissionUser = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            adminUser.SetRoleList(new List<UserRole> { UserRole.SYSTEM_ADMIN });

            await _fsql.Insert(adminUser).ExecuteAffrowsAsync();
            _logger.LogInformation("管理员账户已创建");
        }
        else
        {
            // 重置密码
            adminUser.Password = _passwordService.HashPassword(newPassword);
            adminUser.UpdatedAt = DateTime.Now;

            await _fsql.Update<User>()
                .SetSource(adminUser)
                .ExecuteAffrowsAsync();

            _logger.LogInformation("管理员密码已重置");
        }

        return ApiResponse.Ok("管理员密码已设置成功");
    }
}
