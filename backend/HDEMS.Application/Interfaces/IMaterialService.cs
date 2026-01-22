using HDEMS.Application.DTOs;

namespace HDEMS.Application.Interfaces;

/// <summary>
/// 物资服务接口
/// </summary>
public interface IMaterialService
{
    /// <summary>
    /// 获取物资列表（分页）
    /// </summary>
    Task<ApiResponse<PagedResult<MaterialDto>>> GetPagedAsync(MaterialQueryRequest request);

    /// <summary>
    /// 根据ID获取物资详情
    /// </summary>
    Task<ApiResponse<MaterialDto>> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建物资
    /// </summary>
    Task<ApiResponse<MaterialDto>> CreateAsync(MaterialCreateRequest request);

    /// <summary>
    /// 更新物资
    /// </summary>
    Task<ApiResponse<MaterialDto>> UpdateAsync(Guid id, MaterialCreateRequest request);

    /// <summary>
    /// 删除物资
    /// </summary>
    Task<ApiResponse> DeleteAsync(Guid id);

    /// <summary>
    /// 批量删除物资
    /// </summary>
    Task<ApiResponse> BatchDeleteAsync(List<Guid> ids);

    /// <summary>
    /// 导入物资
    /// </summary>
    Task<ApiResponse<MaterialImportResult>> ImportAsync(Stream fileStream, string fileName);

    /// <summary>
    /// 导出物资
    /// </summary>
    Task<ApiResponse<byte[]>> ExportAsync(MaterialQueryRequest request);

    /// <summary>
    /// 获取物资统计数据
    /// </summary>
    Task<ApiResponse<object>> GetStatisticsAsync();
}
