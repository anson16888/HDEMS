using FreeSql;
using HDEMS.Application.DTOs;
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

        _mapper.Map(request, user);
        user.UpdatedAt = DateTime.Now;

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

        // 软删除
        user.DeletedAt = DateTime.Now;
        await _fsql.Update<User>()
            .SetSource(user)
            .ExecuteAffrowsAsync();

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
}
