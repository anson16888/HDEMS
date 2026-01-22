using FreeSql;
using HDEMS.Application.DTOs;
using HDEMS.Application.Interfaces;
using HDEMS.Domain.Entities;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace HDEMS.Application.Services;

/// <summary>
/// 医院配置服务实现
/// </summary>
public class HospitalConfigService : IHospitalConfigService
{
    private readonly IFreeSql _fsql;
    private readonly IMapper _mapper;
    private readonly ILogger<HospitalConfigService> _logger;

    public HospitalConfigService(IFreeSql fsql, IMapper mapper, ILogger<HospitalConfigService> logger)
    {
        _fsql = fsql;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<HospitalConfigDto>> GetConfigAsync()
    {
        var config = await _fsql.Select<HospitalConfig>().FirstAsync();

        if (config == null)
        {
            // 如果没有配置，返回默认配置
            config = new HospitalConfig
            {
                HospitalName = "宝安区域急救应急物资及值班管理系统",
                HospitalPhone = "0755-12345678",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }

        var dto = _mapper.Map<HospitalConfigDto>(config);
        return ApiResponse<HospitalConfigDto>.Ok(dto);
    }

    public async Task<ApiResponse<HospitalConfigDto>> UpdateConfigAsync(HospitalConfigUpdateRequest request)
    {
        var config = await _fsql.Select<HospitalConfig>().FirstAsync();

        if (config == null)
        {
            // 如果没有配置，创建新配置
            config = new HospitalConfig
            {
                HospitalName = request.HospitalName,
                HospitalPhone = request.HospitalPhone,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _fsql.Insert(config).ExecuteAffrowsAsync();
        }
        else
        {
            // 更新现有配置
            config.HospitalName = request.HospitalName;
            config.HospitalPhone = request.HospitalPhone;
            config.UpdatedAt = DateTime.Now;

            await _fsql.Update<HospitalConfig>()
                .SetSource(config)
                .ExecuteAffrowsAsync();
        }

        var dto = _mapper.Map<HospitalConfigDto>(config);
        _logger.LogInformation("医院配置已更新: {HospitalName}", config.HospitalName);

        return ApiResponse<HospitalConfigDto>.Ok(dto, "更新成功");
    }
}
