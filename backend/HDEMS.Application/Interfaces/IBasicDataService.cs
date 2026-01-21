using HDEMS.Application.DTOs;

namespace HDEMS.Application.Interfaces;

/// <summary>
/// 基础数据服务接口
/// </summary>
public interface IBasicDataService
{
    #region Hospital

    Task<ApiResponse<List<HospitalDto>>> GetHospitalsAsync();
    Task<ApiResponse<HospitalDto>> GetHospitalByIdAsync(Guid id);
    Task<ApiResponse<HospitalDto>> CreateHospitalAsync(HospitalDto dto);
    Task<ApiResponse> UpdateHospitalAsync(Guid id, HospitalDto dto);
    Task<ApiResponse> DeleteHospitalAsync(Guid id);

    #endregion

    #region Department

    Task<ApiResponse<List<DepartmentDto>>> GetDepartmentsAsync();
    Task<ApiResponse<DepartmentDto>> GetDepartmentByIdAsync(Guid id);
    Task<ApiResponse<DepartmentDto>> CreateDepartmentAsync(DepartmentDto dto);
    Task<ApiResponse> UpdateDepartmentAsync(Guid id, DepartmentDto dto);
    Task<ApiResponse> DeleteDepartmentAsync(Guid id);

    #endregion

    #region Shift

    Task<ApiResponse<List<ShiftDto>>> GetShiftsAsync();
    Task<ApiResponse<ShiftDto>> GetShiftByIdAsync(Guid id);
    Task<ApiResponse<ShiftDto>> CreateShiftAsync(ShiftDto dto);
    Task<ApiResponse> UpdateShiftAsync(Guid id, ShiftDto dto);
    Task<ApiResponse> DeleteShiftAsync(Guid id);

    #endregion

    #region PersonRank

    Task<ApiResponse<List<PersonRankDto>>> GetPersonRanksAsync();
    Task<ApiResponse<List<PersonRankDto>>> GetPersonRanksByCategoryAsync(string category);

    #endregion

    #region PersonTitle

    Task<ApiResponse<List<PersonTitleDto>>> GetPersonTitlesAsync();

    #endregion

    #region Person

    Task<ApiResponse<PagedResult<PersonDto>>> GetPersonsPagedAsync(int page = 1, int pageSize = 20, string? keyword = null);
    Task<ApiResponse<PersonDto>> GetPersonByIdAsync(Guid id);
    Task<ApiResponse<List<PersonDto>>> GetPersonsByHospitalAsync(Guid hospitalId);
    Task<ApiResponse> DeletePersonAsync(Guid id);

    #endregion
}
