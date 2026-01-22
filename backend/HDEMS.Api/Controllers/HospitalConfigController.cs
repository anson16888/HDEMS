using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HDEMS.Application.DTOs;
using HDEMS.Application.Interfaces;

namespace HDEMS.Api.Controllers;

/// <summary>
/// 医院配置管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HospitalConfigController : ControllerBase
{
    private readonly IHospitalConfigService _hospitalConfigService;
    private readonly ILogger<HospitalConfigController> _logger;

    public HospitalConfigController(IHospitalConfigService hospitalConfigService, ILogger<HospitalConfigController> logger)
    {
        _hospitalConfigService = hospitalConfigService;
        _logger = logger;
    }

    /// <summary>
    /// 获取医院配置
    /// </summary>
    [HttpGet]
    public async Task<ApiResponse<HospitalConfigDto>> GetConfig()
    {
        return await _hospitalConfigService.GetConfigAsync();
    }

    /// <summary>
    /// 更新医院配置
    /// </summary>
    [HttpPut]
    public async Task<ApiResponse<HospitalConfigDto>> UpdateConfig([FromBody] HospitalConfigUpdateRequest request)
    {
        return await _hospitalConfigService.UpdateConfigAsync(request);
    }
}
