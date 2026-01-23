using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HDEMS.Application.DTOs;
using HDEMS.Application.Interfaces;

namespace HDEMS.Api.Controllers;

/// <summary>
/// 物资库存阈值管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MaterialThresholdController : ControllerBase
{
    private readonly IMaterialThresholdService _materialThresholdService;
    private readonly ILogger<MaterialThresholdController> _logger;

    public MaterialThresholdController(IMaterialThresholdService materialThresholdService, ILogger<MaterialThresholdController> logger)
    {
        _materialThresholdService = materialThresholdService;
        _logger = logger;
    }

    /// <summary>
    /// 获取阈值配置列表（分页）
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <returns>阈值配置列表</returns>
    [HttpGet]
    public async Task<ApiResponse<PagedResult<MaterialThresholdDto>>> GetPaged([FromQuery] MaterialThresholdQueryRequest request)
    {
        return await _materialThresholdService.GetPagedAsync(request);
    }

    /// <summary>
    /// 获取启用的阈值配置列表
    /// </summary>
    /// <returns>阈值配置列表</returns>
    [HttpGet("enabled")]
    public async Task<ApiResponse<List<MaterialThresholdDto>>> GetEnabledThresholds()
    {
        return await _materialThresholdService.GetEnabledThresholdsAsync();
    }

    /// <summary>
    /// 根据ID获取阈值配置
    /// </summary>
    /// <param name="id">阈值配置ID</param>
    /// <returns>阈值配置详情</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<MaterialThresholdDto>> GetById(Guid id)
    {
        return await _materialThresholdService.GetByIdAsync(id);
    }

    /// <summary>
    /// 创建阈值配置
    /// </summary>
    /// <param name="request">创建请求</param>
    /// <returns>创建的阈值配置</returns>
    [HttpPost]
    public async Task<ApiResponse<MaterialThresholdDto>> Create([FromBody] MaterialThresholdCreateRequest request)
    {
        return await _materialThresholdService.CreateAsync(request);
    }

    /// <summary>
    /// 更新阈值配置
    /// </summary>
    /// <param name="id">阈值配置ID</param>
    /// <param name="request">更新请求</param>
    /// <returns>更新后的阈值配置</returns>
    [HttpPut("{id}")]
    public async Task<ApiResponse<MaterialThresholdDto>> Update(Guid id, [FromBody] MaterialThresholdUpdateRequest request)
    {
        return await _materialThresholdService.UpdateAsync(id, request);
    }

    /// <summary>
    /// 删除阈值配置
    /// </summary>
    /// <param name="id">阈值配置ID</param>
    /// <returns>操作结果</returns>
    [HttpDelete("{id}")]
    public async Task<ApiResponse> Delete(Guid id)
    {
        return await _materialThresholdService.DeleteAsync(id);
    }

    /// <summary>
    /// 批量删除阈值配置
    /// </summary>
    /// <param name="ids">阈值配置ID列表</param>
    /// <returns>操作结果</returns>
    [HttpPost("batch-delete")]
    public async Task<ApiResponse> BatchDelete([FromBody] List<Guid> ids)
    {
        return await _materialThresholdService.BatchDeleteAsync(ids);
    }

    /// <summary>
    /// 切换启用状态
    /// </summary>
    /// <param name="id">阈值配置ID</param>
    /// <returns>操作结果</returns>
    [HttpPatch("{id}/toggle")]
    public async Task<ApiResponse> ToggleEnabled(Guid id)
    {
        return await _materialThresholdService.ToggleEnabledAsync(id);
    }
}
