using HDEMS.Application.DTOs;

namespace HDEMS.Application.Interfaces;

/// <summary>
/// 物资库存阈值服务接口
/// </summary>
public interface IMaterialThresholdService
{
    /// <summary>
    /// 获取分页列表
    /// </summary>
    Task<ApiResponse<PagedResult<MaterialThresholdDto>>> GetPagedAsync(MaterialThresholdQueryRequest request);

    /// <summary>
    /// 根据ID获取
    /// </summary>
    Task<ApiResponse<MaterialThresholdDto>> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建
    /// </summary>
    Task<ApiResponse<MaterialThresholdDto>> CreateAsync(MaterialThresholdCreateRequest request);

    /// <summary>
    /// 更新
    /// </summary>
    Task<ApiResponse<MaterialThresholdDto>> UpdateAsync(Guid id, MaterialThresholdUpdateRequest request);

    /// <summary>
    /// 删除
    /// </summary>
    Task<ApiResponse> DeleteAsync(Guid id);

    /// <summary>
    /// 批量删除
    /// </summary>
    Task<ApiResponse> BatchDeleteAsync(List<Guid> ids);

    /// <summary>
    /// 切换启用状态
    /// </summary>
    Task<ApiResponse> ToggleEnabledAsync(Guid id);

    /// <summary>
    /// 获取所有启用的阈值配置
    /// </summary>
    Task<ApiResponse<List<MaterialThresholdDto>>> GetEnabledThresholdsAsync();
}
