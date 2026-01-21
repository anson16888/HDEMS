using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HDEMS.Application.Interfaces;

namespace HDEMS.Api.Controllers;

/// <summary>
/// 导入导出控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ImportExportController : ControllerBase
{
    private readonly IImportExportService _importExportService;

    public ImportExportController(IImportExportService importExportService)
    {
        _importExportService = importExportService;
    }

    /// <summary>
    /// 下载物资导入模板
    /// </summary>
    /// <returns>Excel模板文件</returns>
    [HttpGet("material-template")]
    public async Task<IActionResult> GetMaterialTemplate()
    {
        var data = await _importExportService.GetMaterialTemplateAsync();
        return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "物资导入模板.xlsx");
    }

    /// <summary>
    /// 下载排班导入模板
    /// </summary>
    /// <param name="scheduleType">排班类型</param>
    /// <returns>Excel模板文件</returns>
    [HttpGet("schedule-template/{scheduleType}")]
    public async Task<IActionResult> GetScheduleTemplate(Domain.Enums.ScheduleType scheduleType)
    {
        var data = await _importExportService.GetScheduleTemplateAsync(scheduleType);
        return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{scheduleType}排班导入模板.xlsx");
    }
}
