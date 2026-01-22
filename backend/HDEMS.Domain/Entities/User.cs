using FreeSql.DataAnnotations;
using HDEMS.Domain.Enums;

namespace HDEMS.Domain.Entities;

/// <summary>
/// 用户实体
/// </summary>
[Table(Name = "t_user")]
[Index("idx_username", "Username", true)]
public class User : BaseEntity
{
    /// <summary>
    /// 账号/工号
    /// </summary>
    [Column(StringLength = 50, IsNullable = false)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密码（SHA256加密）
    /// </summary>
    [Column(StringLength = 64, IsNullable = false)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 真实姓名
    /// </summary>
    [Column(StringLength = 50, IsNullable = false)]
    public string RealName { get; set; } = string.Empty;

    /// <summary>
    /// 手机号码
    /// </summary>
    [Column(StringLength = 20, IsNullable = false)]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 科室
    /// </summary>
    [Column(StringLength = 100)]
    public string? Department { get; set; }

    /// <summary>
    /// 用户角色（JSON数组格式存储）
    /// </summary>
    [Column(StringLength = -1)]
    public string Roles { get; set; } = "[]";

    /// <summary>
    /// 用户状态
    /// </summary>
    [Column(IsNullable = false)]
    public UserStatus Status { get; set; } = UserStatus.Active;

    /// <summary>
    /// 最后登录时间
    /// </summary>
    [Column(Position = 8)]
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// 获取角色列表
    /// </summary>
    public List<UserRole> GetRoleList()
    {
        if (string.IsNullOrWhiteSpace(Roles))
            return new List<UserRole>();

        try
        {
            // 从字符串数组解析为枚举
            var roleStrings = System.Text.Json.JsonSerializer.Deserialize<List<string>>(Roles);
            if (roleStrings == null || roleStrings.Count == 0)
                return new List<UserRole>();

            return roleStrings.Select(r => Enum.Parse<UserRole>(r)).ToList();
        }
        catch
        {
            return new List<UserRole>();
        }
    }

    /// <summary>
    /// 设置角色列表
    /// </summary>
    public void SetRoleList(List<UserRole> roles)
    {
        // 存储为字符串数组
        var roleStrings = roles.Select(r => r.ToString()).ToList();
        Roles = System.Text.Json.JsonSerializer.Serialize(roleStrings);
    }

    /// <summary>
    /// 是否包含指定角色
    /// </summary>
    public bool HasRole(UserRole role)
    {
        return GetRoleList().Contains(role);
    }
}
