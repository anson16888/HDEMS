using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HDEMS.Application.DTOs;
using HDEMS.Application.Interfaces;

namespace HDEMS.Api.Controllers;

/// <summary>
/// 基础数据控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BasicDataController : ControllerBase
{
    private readonly IBasicDataService _basicDataService;
    private readonly ILogger<BasicDataController> _logger;

    public BasicDataController(IBasicDataService basicDataService, ILogger<BasicDataController> logger)
    {
        _basicDataService = basicDataService;
        _logger = logger;
    }

    #region Hospital

    /// <summary>
    /// 获取所有医院
    /// </summary>
    [HttpGet("hospitals")]
    public async Task<ApiResponse<List<HospitalDto>>> GetHospitals()
    {
        return await _basicDataService.GetHospitalsAsync();
    }

    /// <summary>
    /// 根据ID获取医院详情
    /// </summary>
    [HttpGet("hospitals/{id}")]
    public async Task<ApiResponse<HospitalDto>> GetHospitalById(Guid id)
    {
        return await _basicDataService.GetHospitalByIdAsync(id);
    }

    /// <summary>
    /// 创建医院
    /// </summary>
    [HttpPost("hospitals")]
    public async Task<ApiResponse<HospitalDto>> CreateHospital([FromBody] HospitalDto dto)
    {
        return await _basicDataService.CreateHospitalAsync(dto);
    }

    /// <summary>
    /// 更新医院
    /// </summary>
    [HttpPut("hospitals/{id}")]
    public async Task<ApiResponse> UpdateHospital(Guid id, [FromBody] HospitalDto dto)
    {
        return await _basicDataService.UpdateHospitalAsync(id, dto);
    }

    /// <summary>
    /// 删除医院
    /// </summary>
    [HttpDelete("hospitals/{id}")]
    public async Task<ApiResponse> DeleteHospital(Guid id)
    {
        return await _basicDataService.DeleteHospitalAsync(id);
    }

    #endregion

    #region Department

    /// <summary>
    /// 获取所有科室
    /// </summary>
    [HttpGet("departments")]
    public async Task<ApiResponse<List<DepartmentDto>>> GetDepartments()
    {
        return await _basicDataService.GetDepartmentsAsync();
    }

    /// <summary>
    /// 根据ID获取科室详情
    /// </summary>
    [HttpGet("departments/{id}")]
    public async Task<ApiResponse<DepartmentDto>> GetDepartmentById(Guid id)
    {
        return await _basicDataService.GetDepartmentByIdAsync(id);
    }

    /// <summary>
    /// 创建科室
    /// </summary>
    [HttpPost("departments")]
    public async Task<ApiResponse<DepartmentDto>> CreateDepartment([FromBody] DepartmentDto dto)
    {
        return await _basicDataService.CreateDepartmentAsync(dto);
    }

    /// <summary>
    /// 更新科室
    /// </summary>
    [HttpPut("departments/{id}")]
    public async Task<ApiResponse> UpdateDepartment(Guid id, [FromBody] DepartmentDto dto)
    {
        return await _basicDataService.UpdateDepartmentAsync(id, dto);
    }

    /// <summary>
    /// 删除科室
    /// </summary>
    [HttpDelete("departments/{id}")]
    public async Task<ApiResponse> DeleteDepartment(Guid id)
    {
        return await _basicDataService.DeleteDepartmentAsync(id);
    }

    #endregion

    #region Shift

    /// <summary>
    /// 获取所有班次
    /// </summary>
    [HttpGet("shifts")]
    public async Task<ApiResponse<List<ShiftDto>>> GetShifts()
    {
        return await _basicDataService.GetShiftsAsync();
    }

    /// <summary>
    /// 根据ID获取班次详情
    /// </summary>
    [HttpGet("shifts/{id}")]
    public async Task<ApiResponse<ShiftDto>> GetShiftById(Guid id)
    {
        return await _basicDataService.GetShiftByIdAsync(id);
    }

    /// <summary>
    /// 创建班次
    /// </summary>
    [HttpPost("shifts")]
    public async Task<ApiResponse<ShiftDto>> CreateShift([FromBody] ShiftDto dto)
    {
        return await _basicDataService.CreateShiftAsync(dto);
    }

    /// <summary>
    /// 更新班次
    /// </summary>
    [HttpPut("shifts/{id}")]
    public async Task<ApiResponse> UpdateShift(Guid id, [FromBody] ShiftDto dto)
    {
        return await _basicDataService.UpdateShiftAsync(id, dto);
    }

    /// <summary>
    /// 删除班次
    /// </summary>
    [HttpDelete("shifts/{id}")]
    public async Task<ApiResponse> DeleteShift(Guid id)
    {
        return await _basicDataService.DeleteShiftAsync(id);
    }

    #endregion

    #region PersonRank

    /// <summary>
    /// 获取所有人员职级
    /// </summary>
    [HttpGet("person-ranks")]
    public async Task<ApiResponse<List<PersonRankDto>>> GetPersonRanks()
    {
        return await _basicDataService.GetPersonRanksAsync();
    }

    /// <summary>
    /// 根据分类获取人员职级
    /// </summary>
    [HttpGet("person-ranks/by-category/{category}")]
    public async Task<ApiResponse<List<PersonRankDto>>> GetPersonRanksByCategory(string category)
    {
        return await _basicDataService.GetPersonRanksByCategoryAsync(category);
    }

    #endregion

    #region PersonTitle

    /// <summary>
    /// 获取所有人员职称
    /// </summary>
    [HttpGet("person-titles")]
    public async Task<ApiResponse<List<PersonTitleDto>>> GetPersonTitles()
    {
        return await _basicDataService.GetPersonTitlesAsync();
    }

    #endregion

    #region Person

    /// <summary>
    /// 获取人员列表（分页）
    /// </summary>
    [HttpGet("persons")]
    public async Task<ApiResponse<PagedResult<PersonDto>>> GetPersonsPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? keyword = null)
    {
        return await _basicDataService.GetPersonsPagedAsync(page, pageSize, keyword);
    }

    /// <summary>
    /// 根据ID获取人员详情
    /// </summary>
    [HttpGet("persons/{id}")]
    public async Task<ApiResponse<PersonDto>> GetPersonById(Guid id)
    {
        return await _basicDataService.GetPersonByIdAsync(id);
    }

    /// <summary>
    /// 根据医院获取人员列表
    /// </summary>
    [HttpGet("persons/by-hospital/{hospitalId}")]
    public async Task<ApiResponse<List<PersonDto>>> GetPersonsByHospital(Guid hospitalId)
    {
        return await _basicDataService.GetPersonsByHospitalAsync(hospitalId);
    }

    /// <summary>
    /// 删除人员
    /// </summary>
    [HttpDelete("persons/{id}")]
    public async Task<ApiResponse> DeletePerson(Guid id)
    {
        return await _basicDataService.DeletePersonAsync(id);
    }

    #endregion
}
