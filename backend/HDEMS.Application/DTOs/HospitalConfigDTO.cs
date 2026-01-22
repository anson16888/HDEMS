namespace HDEMS.Application.DTOs;

/// <summary>
/// 医院配置DTO
/// </summary>
public class HospitalConfigDto
{
    public int Id { get; set; }
    public string HospitalName { get; set; } = string.Empty;
    public string? HospitalPhone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 医院配置更新请求
/// </summary>
public class HospitalConfigUpdateRequest
{
    /// <summary>
    /// 医院名称
    /// </summary>
    public string HospitalName { get; set; } = string.Empty;

    /// <summary>
    /// 医院电话
    /// </summary>
    public string? HospitalPhone { get; set; }
}
