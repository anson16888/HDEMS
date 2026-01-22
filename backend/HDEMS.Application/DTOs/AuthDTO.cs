using HDEMS.Domain.Enums;
using System.ComponentModel;
using System.Text.Json.Serialization;

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
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("userInfo")]
    public UserInfo UserInfo { get; set; } = new UserInfo();
}

/// <summary>
/// 用户信息
/// </summary>
public class UserInfo
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("realName")]
    public string RealName { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;

    [JsonPropertyName("department")]
    public string? Department { get; set; }

    [JsonPropertyName("roles")]
    public List<string> RoleStrings { get; set; } = new List<string>();

    [JsonIgnore]
    public List<UserRole> Roles
    {
        get => RoleStrings.Select(r => Enum.Parse<UserRole>(r)).ToList();
        set => RoleStrings = value.Select(r => r.ToString()).ToList();
    }

    [JsonPropertyName("roleDescriptions")]
    public List<string> RoleDescriptions { get; set; } = new List<string>();

    [JsonPropertyName("hospitalId")]
    public Guid? HospitalId { get; set; }

    [JsonPropertyName("hospitalName")]
    public string? HospitalName { get; set; }

    [JsonPropertyName("systemHospitalName")]
    public string? SystemHospitalName { get; set; }

    [JsonPropertyName("systemLevel")]
    public string? SystemLevel { get; set; }

    [JsonPropertyName("isCommissionUser")]
    public bool IsCommissionUser { get; set; }
}

/// <summary>
/// 设置用户权限请求
/// </summary>
public class SetUserRolesRequest
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 角色列表
    /// </summary>
    public List<UserRole> Roles { get; set; } = new List<UserRole>();
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
    public List<string> Roles { get; set; } = new List<string>();
    public List<string> RoleDescriptions { get; set; } = new List<string>();
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

    [JsonPropertyName("roles")]
    public List<string> RoleStrings { get; set; } = new List<string>();

    [JsonIgnore]
    public List<UserRole> Roles
    {
        get => RoleStrings.Select(r => Enum.Parse<UserRole>(r)).ToList();
        set => RoleStrings = value.Select(r => r.ToString()).ToList();
    }

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

    [JsonPropertyName("roles")]
    public List<string> RoleStrings { get; set; } = new List<string>();

    [JsonIgnore]
    public List<UserRole> Roles
    {
        get => RoleStrings.Select(r => Enum.Parse<UserRole>(r)).ToList();
        set => RoleStrings = value.Select(r => r.ToString()).ToList();
    }

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
