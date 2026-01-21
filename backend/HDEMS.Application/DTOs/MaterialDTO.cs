using HDEMS.Domain.Enums;

namespace HDEMS.Application.DTOs;

/// <summary>
/// 物资DTO
/// </summary>
public class MaterialDto
{
    public Guid Id { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public MaterialType MaterialType { get; set; }
    public string MaterialTypeName { get; set; } = string.Empty;
    public string? Specification { get; set; }
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
    public DateTime? ProductionDate { get; set; }
    public int? ShelfLife { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public Guid HospitalId { get; set; }
    public string? HospitalName { get; set; }
    public string? Remark { get; set; }
    public MaterialStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 物资创建/更新请求
/// </summary>
public class MaterialCreateRequest
{
    public string MaterialCode { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public MaterialType MaterialType { get; set; }
    public string? Specification { get; set; }
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
    public DateTime? ProductionDate { get; set; }
    public int? ShelfLife { get; set; }
    public string Location { get; set; } = string.Empty;
    public Guid HospitalId { get; set; }
    public string? Remark { get; set; }
}

/// <summary>
/// 物资查询请求
/// </summary>
public class MaterialQueryRequest
{
    public string? Keyword { get; set; }
    public MaterialType? MaterialType { get; set; }
    public MaterialStatus? Status { get; set; }
    public Guid? HospitalId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// 物资导入结果
/// </summary>
public class MaterialImportResult
{
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public List<MaterialImportError> Errors { get; set; } = new List<MaterialImportError>();
}

public class MaterialImportError
{
    public int RowNumber { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
