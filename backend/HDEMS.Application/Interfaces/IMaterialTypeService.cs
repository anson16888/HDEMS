using HDEMS.Application.DTOs;

namespace HDEMS.Application.Interfaces;

/// <summary>
/// 物资类型服务接口
/// </summary>
public interface IMaterialTypeService
{
    /// <summary>
    /// 获取物资类型分页列表
    /// </summary>
    Task<ApiResponse<PagedResult<MaterialTypeDto>>> GetPagedAsync(MaterialTypeQueryRequest request);

    /// <summary>
    /// 获取所有启用的物资类型（用于下拉选择）
    /// </summary>
    Task<ApiResponse<List<MaterialTypeOptionDto>>> GetEnabledTypesAsync();

    /// <summary>
    /// 根据ID获取物资类型
    /// </summary>
    Task<ApiResponse<MaterialTypeDto>> GetByIdAsync(Guid id);

    /// <summary>
    /// 根据编码获取物资类型
    /// </summary>
    Task<ApiResponse<MaterialTypeDto>> GetByCodeAsync(string code);

    /// <summary>
    /// 创建物资类型
    /// </summary>
    Task<ApiResponse<MaterialTypeDto>> CreateAsync(MaterialTypeCreateRequest request);

    /// <summary>
    /// 更新物资类型
    /// </summary>
    Task<ApiResponse<MaterialTypeDto>> UpdateAsync(Guid id, MaterialTypeUpdateRequest request);

    /// <summary>
    /// 删除物资类型
    /// </summary>
    Task<ApiResponse> DeleteAsync(Guid id);

    /// <summary>
    /// 批量删除物资类型
    /// </summary>
    Task<ApiResponse> BatchDeleteAsync(List<Guid> ids);

    /// <summary>
    /// 启用/禁用物资类型
    /// </summary>
    Task<ApiResponse> ToggleEnabledAsync(Guid id);
}
