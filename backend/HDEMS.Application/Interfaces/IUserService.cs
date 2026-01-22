using HDEMS.Application.DTOs;

namespace HDEMS.Application.Interfaces;

/// <summary>
/// 用户服务接口
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 获取用户列表（分页）
    /// </summary>
    Task<ApiResponse<PagedResult<UserDto>>> GetPagedAsync(int page = 1, int pageSize = 20, string? keyword = null);

    /// <summary>
    /// 根据ID获取用户详情
    /// </summary>
    Task<ApiResponse<UserDto>> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建用户
    /// </summary>
    Task<ApiResponse<UserDto>> CreateAsync(UserCreateRequest request, string defaultPassword = "123456");

    /// <summary>
    /// 更新用户
    /// </summary>
    Task<ApiResponse<UserDto>> UpdateAsync(Guid id, UserUpdateRequest request);

    /// <summary>
    /// 删除用户
    /// </summary>
    Task<ApiResponse> DeleteAsync(Guid id);

    /// <summary>
    /// 重置用户密码
    /// </summary>
    Task<ApiResponse> ResetPasswordAsync(Guid id, string newPassword);

    /// <summary>
    /// 设置用户权限
    /// </summary>
    Task<ApiResponse> SetUserRolesAsync(Guid userId, List<Domain.Enums.UserRole> roles);

    /// <summary>
    /// 获取管理员列表
    /// </summary>
    Task<ApiResponse<List<UserDto>>> GetAdminsAsync();

    /// <summary>
    /// 修改用户权限
    /// </summary>
    Task<ApiResponse> UpdateUserRolesAsync(Guid userId, List<Domain.Enums.UserRole> roles);
}
