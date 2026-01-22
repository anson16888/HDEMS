using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HDEMS.Application.DTOs;
using HDEMS.Application.Interfaces;

namespace HDEMS.Api.Controllers;

/// <summary>
/// 物资管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MaterialController : ControllerBase
{
    private readonly IMaterialService _materialService;
    private readonly ILogger<MaterialController> _logger;

    public MaterialController(IMaterialService materialService, ILogger<MaterialController> logger)
    {
        _materialService = materialService;
        _logger = logger;
    }

    /// <summary>
    /// 获取物资列表（分页）
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <returns>物资列表</returns>
    [HttpGet]
    public async Task<ApiResponse<PagedResult<MaterialDto>>> GetPaged([FromQuery] MaterialQueryRequest request)
    {
        return await _materialService.GetPagedAsync(request);
    }

    /// <summary>
    /// 根据ID获取物资详情
    /// </summary>
    /// <param name="id">物资ID</param>
    /// <returns>物资详情</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<MaterialDto>> GetById(Guid id)
    {
        return await _materialService.GetByIdAsync(id);
    }

    /// <summary>
    /// 创建物资
    /// </summary>
    /// <param name="request">创建请求</param>
    /// <returns>创建的物资</returns>
    [HttpPost]
    public async Task<ApiResponse<MaterialDto>> Create([FromBody] MaterialCreateRequest request)
    {
        return await _materialService.CreateAsync(request);
    }

    /// <summary>
    /// 更新物资
    /// </summary>
    /// <param name="id">物资ID</param>
    /// <param name="request">更新请求</param>
    /// <returns>更新后的物资</returns>
    [HttpPut("{id}")]
    public async Task<ApiResponse<MaterialDto>> Update(Guid id, [FromBody] MaterialCreateRequest request)
    {
        return await _materialService.UpdateAsync(id, request);
    }

    /// <summary>
    /// 删除物资
    /// </summary>
    /// <param name="id">物资ID</param>
    /// <returns>操作结果</returns>
    [HttpDelete("{id}")]
    public async Task<ApiResponse> Delete(Guid id)
    {
        return await _materialService.DeleteAsync(id);
    }

    /// <summary>
    /// 批量删除物资
    /// </summary>
    /// <param name="ids">物资ID列表</param>
    /// <returns>操作结果</returns>
    [HttpPost("batch-delete")]
    public async Task<ApiResponse> BatchDelete([FromBody] List<Guid> ids)
    {
        return await _materialService.BatchDeleteAsync(ids);
    }

    /// <summary>
    /// 导入物资
    /// </summary>
    /// <param name="file">Excel文件</param>
    /// <returns>导入结果</returns>
    [HttpPost("import")]
    public async Task<ApiResponse<MaterialImportResult>> Import(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return ApiResponse<MaterialImportResult>.Fail(400, "请选择有效的Excel文件");
        }

        using var stream = file.OpenReadStream();
        return await _materialService.ImportAsync(stream, file.FileName);
    }

    /// <summary>
    /// 导出物资
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <returns>Excel文件</returns>
    [HttpPost("export")]
    public async Task<IActionResult> Export([FromBody] MaterialQueryRequest request)
    {
        var result = await _materialService.ExportAsync(request);
        if (!result.Success || result.Data == null)
        {
            return BadRequest(result);
        }

        return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"物资数据_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }

    /// <summary>
    /// 获取物资统计数据
    /// </summary>
    /// <param name="hospitalId">医院ID（可选）</param>
    /// <returns>统计数据</returns>
    [HttpGet("statistics")]
    public async Task<ApiResponse<object>> GetStatistics()
    {
        return await _materialService.GetStatisticsAsync();
    }
}
