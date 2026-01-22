using FreeSql;
using HDEMS.Application.DTOs;
using HDEMS.Application.Extensions;
using HDEMS.Application.Interfaces;
using HDEMS.Domain.Entities;
using HDEMS.Domain.Enums;
using HDEMS.Infrastructure.Services;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace HDEMS.Application.Services;

/// <summary>
/// 用户服务实现
/// </summary>
public class UserService : IUserService
{
    private readonly IFreeSql _fsql;
    private readonly IMapper _mapper;
    private readonly PasswordService _passwordService;

    public UserService(IFreeSql fsql, IMapper mapper, PasswordService passwordService)
    {
        _fsql = fsql;
        _mapper = mapper;
        _passwordService = passwordService;
    }

    public async Task<ApiResponse<PagedResult<UserDto>>> GetPagedAsync(int page = 1, int pageSize = 20, string? keyword = null)
    {
        var query = _fsql.Select<User>().Include(u => u.Hospital);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(u => u.RealName.Contains(keyword) || u.Username.Contains(keyword));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Page(page, pageSize)
            .ToListAsync();

        var dtos = _mapper.Map<List<UserDto>>(items);

        // 填充角色相关字段
        for (int i = 0; i < dtos.Count; i++)
        {
            PopulateRoleFields(dtos[i], items[i]);
        }

        var result = new PagedResult<UserDto>
        {
            Total = (int)total,
            Page = page,
            PageSize = pageSize,
            Items = dtos
        };

        return ApiResponse.OkPaged(result);
    }

    public async Task<ApiResponse<UserDto>> GetByIdAsync(Guid id)
    {
        var user = await _fsql.Select<User>()
            .Include(u => u.Hospital)
            .Where(u => u.Id == id)
            .FirstAsync();

        if (user == null)
        {
            return ApiResponse<UserDto>.Fail(404, "用户不存在");
        }

        var dto = _mapper.Map<UserDto>(user);
        PopulateRoleFields(dto, user);
        return ApiResponse<UserDto>.Ok(dto);
    }

    public async Task<ApiResponse<UserDto>> CreateAsync(UserCreateRequest request, string defaultPassword = "123456")
    {
        // 检查用户名是否已存在
        var exists = await _fsql.Select<User>()
            .Where(u => u.Username == request.Username)
            .AnyAsync();

        if (exists)
        {
            return ApiResponse<UserDto>.Fail(400, "用户名已存在");
        }

        var user = _mapper.Map<User>(request);
        user.Id = Guid.NewGuid();
        user.Password = _passwordService.HashPassword(defaultPassword);
        user.Status = UserStatus.Active;
        user.CreatedAt = DateTime.Now;
        user.UpdatedAt = DateTime.Now;

        // 手动设置角色（从字符串转换为枚举）
        user.SetRoleList(request.Roles);

        await _fsql.Insert(user).ExecuteAffrowsAsync();

        var result = await GetByIdAsync(user.Id);
        return result;
    }

    public async Task<ApiResponse<UserDto>> UpdateAsync(Guid id, UserUpdateRequest request)
    {
        var user = await _fsql.Select<User>().Where(u => u.Id == id).FirstAsync();
        if (user == null)
        {
            return ApiResponse<UserDto>.Fail(404, "用户不存在");
        }

        // 手动映射字段
        user.RealName = request.RealName;
        user.Phone = request.Phone;
        user.Department = request.Department;
        user.Status = request.Status;
        user.HospitalId = request.HospitalId;
        user.IsCommissionUser = request.IsCommissionUser;
        user.UpdatedAt = DateTime.Now;

        // 手动设置角色（从字符串转换为枚举）
        user.SetRoleList(request.Roles);

        await _fsql.Update<User>()
            .SetSource(user)
            .ExecuteAffrowsAsync();

        var result = await GetByIdAsync(user.Id);
        return result;
    }

    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        var user = await _fsql.Select<User>().Where(u => u.Id == id).FirstAsync();
        if (user == null)
        {
            return ApiResponse.Fail(404, "用户不存在");
        }

        // 硬删除
        await _fsql.Delete<User>().Where(u => u.Id == id).ExecuteAffrowsAsync();

        return ApiResponse.Ok("删除成功");
    }

    public async Task<ApiResponse> ResetPasswordAsync(Guid id, string newPassword)
    {
        var user = await _fsql.Select<User>().Where(u => u.Id == id).FirstAsync();
        if (user == null)
        {
            return ApiResponse.Fail(404, "用户不存在");
        }

        user.Password = _passwordService.HashPassword(newPassword);
        user.UpdatedAt = DateTime.Now;

        await _fsql.Update<User>()
            .SetSource(user)
            .ExecuteAffrowsAsync();

        return ApiResponse.Ok("密码重置成功");
    }

    public async Task<ApiResponse> SetUserRolesAsync(Guid userId, List<UserRole> roles)
    {
        var user = await _fsql.Select<User>().Where(u => u.Id == userId).FirstAsync();
        if (user == null)
        {
            return ApiResponse.Fail(404, "用户不存在");
        }

        user.SetRoleList(roles);
        user.UpdatedAt = DateTime.Now;

        await _fsql.Update<User>()
            .Set(u => u.Roles, user.Roles)
            .Set(u => u.UpdatedAt, user.UpdatedAt)
            .Where(u => u.Id == userId)
            .ExecuteAffrowsAsync();

        return ApiResponse.Ok("权限设置成功");
    }

    public async Task<ApiResponse<List<UserDto>>> GetAdminsAsync()
    {
        var users = await _fsql.Select<User>()
            .Include(u => u.Hospital)
            .Where(u => u.DeletedAt == null)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        var dtos = _mapper.Map<List<UserDto>>(users);

        // 填充角色相关字段
        for (int i = 0; i < dtos.Count; i++)
        {
            PopulateRoleFields(dtos[i], users[i]);
        }

        // 过滤出有角色的用户（管理员）
        var admins = dtos.Where(u => u.Roles != null && u.Roles.Count > 0).ToList();

        return DTOs.ApiResponse<List<UserDto>>.Ok(admins);
    }

    public async Task<ApiResponse> UpdateUserRolesAsync(Guid userId, List<Domain.Enums.UserRole> roles)
    {
        var user = await _fsql.Select<User>().Where(u => u.Id == userId).FirstAsync();
        if (user == null)
        {
            return ApiResponse.Fail(404, "用户不存在");
        }

        user.SetRoleList(roles);
        user.UpdatedAt = DateTime.Now;

        await _fsql.Update<User>()
            .Set(u => u.Roles, user.Roles)
            .Set(u => u.UpdatedAt, user.UpdatedAt)
            .Where(u => u.Id == userId)
            .ExecuteAffrowsAsync();

        return ApiResponse.Ok("用户权限修改成功");
    }

    /// <summary>
    /// 填充用户DTO的角色相关字段
    /// </summary>
    private void PopulateRoleFields(UserDto dto, User user)
    {
        var roleList = user.GetRoleList();
        dto.Roles = roleList.Select(r => r.ToString()).ToList();
        dto.RoleDescriptions = roleList.Select(r => r.GetDescription()).ToList();
    }
}
