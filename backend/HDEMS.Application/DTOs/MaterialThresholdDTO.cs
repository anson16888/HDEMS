namespace HDEMS.Application.DTOs;

/// <summary>
/// 物资库存阈值 DTO
/// </summary>
public class MaterialThresholdDto
{
    public Guid Id { get; set; }
    public Guid MaterialTypeId { get; set; }
    public string MaterialTypeName { get; set; } = string.Empty;
    public string? MaterialTypeColor { get; set; }
    public int Threshold { get; set; }
    public bool IsEnabled { get; set; }
    public int SortOrder { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 物资库存阈值创建请求
/// </summary>
public class MaterialThresholdCreateRequest
{
    /// <summary>
    /// 物资类型ID
    /// </summary>
    public Guid MaterialTypeId { get; set; }

    /// <summary>
    /// 预警阈值
    /// </summary>
    public int Threshold { get; set; } = 5;

    /// <summary>
    /// 启用状态
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 物资库存阈值更新请求
/// </summary>
public class MaterialThresholdUpdateRequest
{
    /// <summary>
    /// 物资类型ID
    /// </summary>
    public Guid MaterialTypeId { get; set; }

    /// <summary>
    /// 预警阈值
    /// </summary>
    public int Threshold { get; set; } = 5;

    /// <summary>
    /// 启用状态
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 物资库存阈值查询请求
/// </summary>
public class MaterialThresholdQueryRequest
{
    /// <summary>
    /// 页码
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 关键字搜索
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 物资类型ID
    /// </summary>
    public Guid? MaterialTypeId { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool? IsEnabled { get; set; }
}
