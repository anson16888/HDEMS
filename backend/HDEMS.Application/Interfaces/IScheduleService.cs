using HDEMS.Application.DTOs;
using HDEMS.Domain.Enums;

namespace HDEMS.Application.Interfaces;

/// <summary>
/// 排班服务接口
/// </summary>
public interface IScheduleService
{
    /// <summary>
    /// 获取排班列表（分页）
    /// </summary>
    Task<ApiResponse<PagedResult<ScheduleDto>>> GetPagedAsync(ScheduleQueryRequest request);

    /// <summary>
    /// 根据ID获取排班详情
    /// </summary>
    Task<ApiResponse<ScheduleDto>> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建排班
    /// </summary>
    Task<ApiResponse<ScheduleDto>> CreateAsync(ScheduleCreateRequest request);

    /// <summary>
    /// 批量创建排班
    /// </summary>
    Task<ApiResponse<List<ScheduleDto>>> BatchCreateAsync(List<ScheduleCreateRequest> requests);

    /// <summary>
    /// 更新排班
    /// </summary>
    Task<ApiResponse<ScheduleDto>> UpdateAsync(Guid id, ScheduleCreateRequest request);

    /// <summary>
    /// 删除排班
    /// </summary>
    Task<ApiResponse> DeleteAsync(Guid id);

    /// <summary>
    /// 批量删除排班
    /// </summary>
    Task<ApiResponse> BatchDeleteAsync(List<Guid> ids);

    /// <summary>
    /// 获取排班一览表（汇总数据）
    /// </summary>
    Task<ApiResponse<PagedResult<ScheduleOverviewItem>>> GetOverviewAsync(ScheduleQueryRequest request);

    /// <summary>
    /// 获取排班统计数据
    /// </summary>
    /// <param name="startDate">开始日期（可选）</param>
    /// <param name="endDate">结束日期（可选）</param>
    Task<ApiResponse<ScheduleStatistics>> GetStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// 导出排班数据
    /// </summary>
    Task<ApiResponse<byte[]>> ExportAsync(ScheduleQueryRequest request);

    /// <summary>
    /// 导入排班数据
    /// </summary>
    Task<ApiResponse<MaterialImportResult>> ImportAsync(Stream fileStream, string fileName, Domain.Enums.ScheduleType scheduleType);
}
