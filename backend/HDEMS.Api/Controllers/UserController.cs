using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HDEMS.Application.DTOs;
using HDEMS.Application.Interfaces;

namespace HDEMS.Api.Controllers;

/// <summary>
/// 用户管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// 获取用户列表（分页）
    /// </summary>
    /// <param name="page">页码</param>
    /// <param name="pageSize">每页条数</param>
    /// <param name="keyword">搜索关键词</param>
    /// <param name="role">角色筛选</param>
    /// <param name="status">状态筛选</param>
    /// <param name="hospitalId">医院ID筛选</param>
    /// <returns>用户列表</returns>
    [HttpGet]
    public async Task<ApiResponse<PagedResult<UserDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? keyword = null,
        [FromQuery] int? role = null,
        [FromQuery] int? status = null,
        [FromQuery] Guid? hospitalId = null)
    {
        return await _userService.GetPagedAsync(page, pageSize, keyword, role, status, hospitalId);
    }

    /// <summary>
    /// 根据ID获取用户详情
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>用户详情</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<UserDto>> GetById(Guid id)
    {
        return await _userService.GetByIdAsync(id);
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="request">创建请求</param>
    /// <returns>创建的用户</returns>
    [HttpPost]
    public async Task<ApiResponse<UserDto>> Create([FromBody] UserCreateRequest request)
    {
        return await _userService.CreateAsync(request);
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="request">更新请求</param>
    /// <returns>更新后的用户</returns>
    [HttpPut("{id}")]
    public async Task<ApiResponse<UserDto>> Update(Guid id, [FromBody] UserUpdateRequest request)
    {
        return await _userService.UpdateAsync(id, request);
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>操作结果</returns>
    [HttpDelete("{id}")]
    public async Task<ApiResponse> Delete(Guid id)
    {
        return await _userService.DeleteAsync(id);
    }

    /// <summary>
    /// 重置用户密码
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="newPassword">新密码</param>
    /// <returns>操作结果</returns>
    [HttpPost("{id}/reset-password")]
    public async Task<ApiResponse> ResetPassword(Guid id, [FromBody] string newPassword)
    {
        return await _userService.ResetPasswordAsync(id, newPassword);
    }

    /// <summary>
    /// 设置用户权限
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="roles">角色编码列表，如 ["SYSTEM_ADMIN", "MATERIAL_ADMIN"]</param>
    /// <returns>操作结果</returns>
    [HttpPost("{id}/set-roles")]
    public async Task<ApiResponse> SetUserRoles(Guid id, [FromBody] List<string> roles)
    {
        var roleList = roles.Select(r => Enum.Parse<Domain.Enums.UserRole>(r)).ToList();
        return await _userService.SetUserRolesAsync(id, roleList);
    }

    /// <summary>
    /// 获取管理员列表
    /// </summary>
    /// <returns>管理员列表</returns>
    [HttpGet("admins")]
    public async Task<ApiResponse<List<UserDto>>> GetAdmins()
    {
        return await _userService.GetAdminsAsync();
    }

    /// <summary>
    /// 修改用户权限
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="roles">角色编码列表，如 ["SYSTEM_ADMIN", "MATERIAL_ADMIN"]</param>
    /// <returns>操作结果</returns>
    [HttpPut("{id}/roles")]
    public async Task<ApiResponse> UpdateUserRoles(Guid id, [FromBody] List<string> roles)
    {
        var roleList = roles.Select(r => Enum.Parse<Domain.Enums.UserRole>(r)).ToList();
        return await _userService.UpdateUserRolesAsync(id, roleList);
    }
}
