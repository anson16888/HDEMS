using HDEMS.Domain.Enums;

namespace HDEMS.Application.DTOs;

/// <summary>
/// 登录请求
/// </summary>
public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// 登录响应
/// </summary>
public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public UserInfo UserInfo { get; set; } = new UserInfo();
}

/// <summary>
/// 用户信息
/// </summary>
public class UserInfo
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Department { get; set; }
    public List<UserRole> Roles { get; set; } = new List<UserRole>();
    public Guid? HospitalId { get; set; }
    public string? HospitalName { get; set; }
    public bool IsCommissionUser { get; set; }
}

/// <summary>
/// 用户DTO
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Department { get; set; }
    public List<UserRole> Roles { get; set; } = new List<UserRole>();
    public UserStatus Status { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public Guid? HospitalId { get; set; }
    public string? HospitalName { get; set; }
    public bool IsCommissionUser { get; set; }
}

/// <summary>
/// 用户创建请求
/// </summary>
public class UserCreateRequest
{
    public string Username { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Department { get; set; }
    public List<UserRole> Roles { get; set; } = new List<UserRole>();
    public Guid? HospitalId { get; set; }
    public bool IsCommissionUser { get; set; }
}

/// <summary>
/// 用户更新请求
/// </summary>
public class UserUpdateRequest
{
    public string RealName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Department { get; set; }
    public List<UserRole> Roles { get; set; } = new List<UserRole>();
    public UserStatus Status { get; set; }
    public Guid? HospitalId { get; set; }
    public bool IsCommissionUser { get; set; }
}

/// <summary>
/// 修改密码请求
/// </summary>
public class ChangePasswordRequest
{
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
