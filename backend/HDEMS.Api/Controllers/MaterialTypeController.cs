using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HDEMS.Application.DTOs;
using HDEMS.Application.Interfaces;

namespace HDEMS.Api.Controllers;

/// <summary>
/// 物资类型管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MaterialTypeController : ControllerBase
{
    private readonly IMaterialTypeService _materialTypeService;
    private readonly ILogger<MaterialTypeController> _logger;

    public MaterialTypeController(IMaterialTypeService materialTypeService, ILogger<MaterialTypeController> logger)
    {
        _materialTypeService = materialTypeService;
        _logger = logger;
    }

    /// <summary>
    /// 获取物资类型分页列表
    /// </summary>
    [HttpGet]
    public async Task<ApiResponse<PagedResult<MaterialTypeDto>>> GetPaged([FromQuery] MaterialTypeQueryRequest request)
    {
        return await _materialTypeService.GetPagedAsync(request);
    }

    /// <summary>
    /// 获取所有启用的物资类型（用于下拉选择）
    /// </summary>
    [HttpGet("enabled")]
    public async Task<ApiResponse<List<MaterialTypeOptionDto>>> GetEnabledTypes()
    {
        return await _materialTypeService.GetEnabledTypesAsync();
    }

    /// <summary>
    /// 根据ID获取物资类型
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ApiResponse<MaterialTypeDto>> GetById(int id)
    {
        return await _materialTypeService.GetByIdAsync(id);
    }

    /// <summary>
    /// 根据编码获取物资类型
    /// </summary>
    [HttpGet("code/{code}")]
    public async Task<ApiResponse<MaterialTypeDto>> GetByCode(string code)
    {
        return await _materialTypeService.GetByCodeAsync(code);
    }

    /// <summary>
    /// 创建物资类型
    /// </summary>
    [HttpPost]
    public async Task<ApiResponse<MaterialTypeDto>> Create([FromBody] MaterialTypeCreateRequest request)
    {
        return await _materialTypeService.CreateAsync(request);
    }

    /// <summary>
    /// 更新物资类型
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResponse<MaterialTypeDto>> Update(int id, [FromBody] MaterialTypeUpdateRequest request)
    {
        return await _materialTypeService.UpdateAsync(id, request);
    }

    /// <summary>
    /// 删除物资类型
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResponse> Delete(int id)
    {
        return await _materialTypeService.DeleteAsync(id);
    }

    /// <summary>
    /// 批量删除物资类型
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ApiResponse> BatchDelete([FromBody] List<int> ids)
    {
        return await _materialTypeService.BatchDeleteAsync(ids);
    }

    /// <summary>
    /// 启用/禁用物资类型
    /// </summary>
    [HttpPatch("{id}/toggle")]
    public async Task<ApiResponse> ToggleEnabled(int id)
    {
        return await _materialTypeService.ToggleEnabledAsync(id);
    }
}
