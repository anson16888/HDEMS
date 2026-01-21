using FreeSql;
using HDEMS.Application.DTOs;
using HDEMS.Application.Interfaces;
using HDEMS.Domain.Entities;
using HDEMS.Domain.Enums;
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

    public AuthService(IFreeSql fsql, JwtService jwtService, PasswordService passwordService, ILogger<AuthService> logger)
    {
        _fsql = fsql;
        _jwtService = jwtService;
        _passwordService = passwordService;
        _logger = logger;
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
        var roles = user.GetRoleList().Select(r => r.ToString()).ToList();
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
                Roles = user.GetRoleList(),
                HospitalId = user.HospitalId,
                HospitalName = user.Hospital?.HospitalName,
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

    public async Task<ApiResponse> ResetAdminPasswordAsync(string initKey)
    {
        // 验证初始化密钥（在实际应用中，应该验证从配置文件中获取的密钥）
        // 这里简化处理，仅作示例

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
                Password = _passwordService.HashPassword("123456"),
                RealName = "系统管理员",
                Phone = "13800000000",
                Department = "信息科",
                Status = UserStatus.Active,
                IsCommissionUser = true
            };
            adminUser.SetRoleList(new List<UserRole> { UserRole.Admin });

            await _fsql.Insert(adminUser).ExecuteAffrowsAsync();
        }
        else
        {
            // 重置密码
            var newPassword = _passwordService.GenerateRandomPassword();
            adminUser.Password = _passwordService.HashPassword(newPassword);
            adminUser.UpdatedAt = DateTime.Now;

            await _fsql.Update<User>()
                .SetSource(adminUser)
                .ExecuteAffrowsAsync();

            _logger.LogWarning("管理员密码已重置为: {Password}", newPassword);
            return ApiResponse.Ok($"管理员密码已重置为: {newPassword}");
        }

        return ApiResponse.Ok("管理员账户已创建，默认密码: 123456");
    }
}
