using HDEMS.Application.DTOs;

namespace HDEMS.Application.Interfaces;

/// <summary>
/// 医院配置服务接口
/// </summary>
public interface IHospitalConfigService
{
    /// <summary>
    /// 获取医院配置
    /// </summary>
    Task<ApiResponse<HospitalConfigDto>> GetConfigAsync();

    /// <summary>
    /// 更新医院配置
    /// </summary>
    Task<ApiResponse<HospitalConfigDto>> UpdateConfigAsync(HospitalConfigUpdateRequest request);
}
