using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HDEMS.Application.DTOs;
using HDEMS.Application.Interfaces;

namespace HDEMS.Api.Controllers;

/// <summary>
/// 排班管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _scheduleService;
    private readonly ILogger<ScheduleController> _logger;

    public ScheduleController(IScheduleService scheduleService, ILogger<ScheduleController> logger)
    {
        _scheduleService = scheduleService;
        _logger = logger;
    }

    /// <summary>
    /// 获取排班列表（分页）
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <returns>排班列表</returns>
    [HttpGet]
    public async Task<ApiResponse<PagedResult<ScheduleDto>>> GetPaged([FromQuery] ScheduleQueryRequest request)
    {
        return await _scheduleService.GetPagedAsync(request);
    }

    /// <summary>
    /// 根据ID获取排班详情
    /// </summary>
    /// <param name="id">排班ID</param>
    /// <returns>排班详情</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<ScheduleDto>> GetById(Guid id)
    {
        return await _scheduleService.GetByIdAsync(id);
    }

    /// <summary>
    /// 创建排班
    /// </summary>
    /// <param name="request">创建请求</param>
    /// <returns>创建的排班</returns>
    [HttpPost]
    public async Task<ApiResponse<ScheduleDto>> Create([FromBody] ScheduleCreateRequest request)
    {
        return await _scheduleService.CreateAsync(request);
    }

    /// <summary>
    /// 批量创建排班
    /// </summary>
    /// <param name="requests">创建请求列表</param>
    /// <returns>创建的排班列表</returns>
    [HttpPost("batch")]
    public async Task<ApiResponse<List<ScheduleDto>>> BatchCreate([FromBody] List<ScheduleCreateRequest> requests)
    {
        return await _scheduleService.BatchCreateAsync(requests);
    }

    /// <summary>
    /// 更新排班
    /// </summary>
    /// <param name="id">排班ID</param>
    /// <param name="request">更新请求</param>
    /// <returns>更新后的排班</returns>
    [HttpPut("{id}")]
    public async Task<ApiResponse<ScheduleDto>> Update(Guid id, [FromBody] ScheduleCreateRequest request)
    {
        return await _scheduleService.UpdateAsync(id, request);
    }

    /// <summary>
    /// 删除排班
    /// </summary>
    /// <param name="id">排班ID</param>
    /// <returns>操作结果</returns>
    [HttpDelete("{id}")]
    public async Task<ApiResponse> Delete(Guid id)
    {
        return await _scheduleService.DeleteAsync(id);
    }

    /// <summary>
    /// 批量删除排班
    /// </summary>
    /// <param name="ids">排班ID列表</param>
    /// <returns>操作结果</returns>
    [HttpPost("batch-delete")]
    public async Task<ApiResponse> BatchDelete([FromBody] List<Guid> ids)
    {
        return await _scheduleService.BatchDeleteAsync(ids);
    }

    /// <summary>
    /// 获取排班一览表
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <returns>排班一览数据</returns>
    [HttpGet("overview")]
    public async Task<ApiResponse<PagedResult<ScheduleOverviewItem>>> GetOverview([FromQuery] ScheduleQueryRequest request)
    {
        return await _scheduleService.GetOverviewAsync(request);
    }

    /// <summary>
    /// 获取排班统计数据
    /// </summary>
    /// <param name="startDate">开始日期（可选）</param>
    /// <param name="endDate">结束日期（可选）</param>
    /// <returns>统计数据</returns>
    [HttpGet("statistics")]
    public async Task<ApiResponse<ScheduleStatistics>> GetStatistics([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        return await _scheduleService.GetStatisticsAsync(startDate, endDate);
    }

    /// <summary>
    /// 导出排班数据
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <returns>Excel文件</returns>
    [HttpPost("export")]
    public async Task<IActionResult> Export([FromBody] ScheduleQueryRequest request)
    {
        var result = await _scheduleService.ExportAsync(request);
        if (!result.Success || result.Data == null)
        {
            return BadRequest(result);
        }

        return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"排班数据_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }

    /// <summary>
    /// 导入排班数据
    /// </summary>
    /// <param name="scheduleType">排班类型</param>
    /// <param name="file">Excel文件</param>
    /// <returns>导入结果</returns>
    [HttpPost("import/{scheduleType}")]
    public async Task<ApiResponse<MaterialImportResult>> Import(Domain.Enums.ScheduleType scheduleType, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return ApiResponse<MaterialImportResult>.Fail(400, "请选择有效的Excel文件");
        }

        using var stream = file.OpenReadStream();
        return await _scheduleService.ImportAsync(stream, file.FileName, scheduleType);
    }
}
