namespace HDEMS.Application.DTOs;

/// <summary>
/// 物资类型DTO
/// </summary>
public class MaterialTypeDto
{
    public Guid Id { get; set; }
    public string TypeCode { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public string? Color { get; set; }
    public int SortOrder { get; set; }
    public bool IsEnabled { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 物资类型创建请求
/// </summary>
public class MaterialTypeCreateRequest
{
    public string TypeCode { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public string? Color { get; set; }
    public int SortOrder { get; set; }
    public bool IsEnabled { get; set; } = true;
    public string? Remark { get; set; }
}

/// <summary>
/// 物资类型更新请求
/// </summary>
public class MaterialTypeUpdateRequest
{
    public Guid Id { get; set; }
    public string TypeCode { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public string? Color { get; set; }
    public int SortOrder { get; set; }
    public bool IsEnabled { get; set; }
    public string? Remark { get; set; }
}

/// <summary>
/// 物资类型查询请求
/// </summary>
public class MaterialTypeQueryRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Keyword { get; set; }
    public bool? IsEnabled { get; set; }
}

/// <summary>
/// 物资类型简单DTO（用于下拉选择）
/// </summary>
public class MaterialTypeOptionDto
{
    public Guid Id { get; set; }
    public string TypeCode { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public string? Color { get; set; }
}
