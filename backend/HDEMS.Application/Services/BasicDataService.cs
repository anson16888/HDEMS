using FreeSql;
using HDEMS.Application.DTOs;
using HDEMS.Application.Interfaces;
using HDEMS.Domain.Entities;
using HDEMS.Infrastructure.Services;
using HDEMS.Infrastructure.Contexts;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace HDEMS.Application.Services;

/// <summary>
/// 基础数据服务实现
/// </summary>
public class BasicDataService : IBasicDataService
{
    private readonly IFreeSql _fsql;
    private readonly IMapper _mapper;
    private readonly AuditContext _auditContext;

    public BasicDataService(IFreeSql fsql, IMapper mapper, AuditContext auditContext)
    {
        _fsql = fsql;
        _mapper = mapper;
        _auditContext = auditContext;
    }

    #region Hospital

    public async Task<ApiResponse<List<HospitalDto>>> GetHospitalsAsync()
    {
        var items = await _fsql.Select<Hospital>()
            .OrderBy(h => h.SortOrder)
            .ToListAsync();

        var dtos = _mapper.Map<List<HospitalDto>>(items);
        return ApiResponse<List<HospitalDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<HospitalDto>> GetHospitalByIdAsync(Guid id)
    {
        var hospital = await _fsql.Select<Hospital>().Where(h => h.Id == id).FirstAsync();
        if (hospital == null)
        {
            return ApiResponse<HospitalDto>.Fail(404, "医院不存在");
        }
        var dto = _mapper.Map<HospitalDto>(hospital);
        return ApiResponse<HospitalDto>.Ok(dto);
    }

    public async Task<ApiResponse<HospitalDto>> CreateHospitalAsync(HospitalDto dto)
    {
        var hospital = _mapper.Map<Hospital>(dto);
        hospital.Id = Guid.NewGuid();
        hospital.CreatedBy = _auditContext.CurrentUserDisplayName;
        hospital.CreatedAt = DateTime.Now;
        hospital.UpdatedBy = _auditContext.CurrentUserDisplayName;
        hospital.UpdatedAt = DateTime.Now;

        await _fsql.Insert(hospital).ExecuteAffrowsAsync();
        return await GetHospitalByIdAsync(hospital.Id);
    }

    public async Task<ApiResponse> UpdateHospitalAsync(Guid id, HospitalDto dto)
    {
        var hospital = await _fsql.Select<Hospital>().Where(h => h.Id == id).FirstAsync();
        if (hospital == null)
        {
            return ApiResponse.Fail(404, "医院不存在");
        }

        _mapper.Map(dto, hospital);
        hospital.Id = id; // 确保Id不被覆盖
        hospital.UpdatedBy = _auditContext.CurrentUserDisplayName;
        hospital.UpdatedAt = DateTime.Now;

        await _fsql.Update<Hospital>().SetSource(hospital).ExecuteAffrowsAsync();
        return ApiResponse.Ok("更新成功");
    }

    public async Task<ApiResponse> DeleteHospitalAsync(Guid id)
    {
        var exists = await _fsql.Select<Hospital>().Where(h => h.Id == id).AnyAsync();
        if (!exists)
        {
            return ApiResponse.Fail(404, "医院不存在");
        }

        // 软删除
        await _fsql.Update<Hospital>()
            .Set(h => h.IsDeleted, true)
            .Set(h => h.DeletedBy, _auditContext.CurrentUserDisplayName)
            .Set(h => h.DeletedAt, DateTime.Now)
            .Where(h => h.Id == id)
            .ExecuteAffrowsAsync();
        return ApiResponse.Ok("删除成功");
    }

    #endregion

    #region Department

    public async Task<ApiResponse<List<DepartmentDto>>> GetDepartmentsAsync()
    {
        var items = await _fsql.Select<Department>()
            .OrderBy(d => d.SortOrder)
            .ToListAsync();

        var dtos = _mapper.Map<List<DepartmentDto>>(items);
        return ApiResponse<List<DepartmentDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<DepartmentDto>> GetDepartmentByIdAsync(Guid id)
    {
        var department = await _fsql.Select<Department>().Where(d => d.Id == id).FirstAsync();
        if (department == null)
        {
            return ApiResponse<DepartmentDto>.Fail(404, "科室不存在");
        }
        var dto = _mapper.Map<DepartmentDto>(department);
        return ApiResponse<DepartmentDto>.Ok(dto);
    }

    public async Task<ApiResponse<DepartmentDto>> CreateDepartmentAsync(DepartmentDto dto)
    {
        var department = _mapper.Map<Department>(dto);
        department.Id = Guid.NewGuid();
        department.CreatedBy = _auditContext.CurrentUserDisplayName;
        department.CreatedAt = DateTime.Now;
        department.UpdatedBy = _auditContext.CurrentUserDisplayName;
        department.UpdatedAt = DateTime.Now;

        await _fsql.Insert(department).ExecuteAffrowsAsync();
        return await GetDepartmentByIdAsync(department.Id);
    }

    public async Task<ApiResponse> UpdateDepartmentAsync(Guid id, DepartmentDto dto)
    {
        var department = await _fsql.Select<Department>().Where(d => d.Id == id).FirstAsync();
        if (department == null)
        {
            return ApiResponse.Fail(404, "科室不存在");
        }

        _mapper.Map(dto, department);
        department.Id = id; // 确保Id不被覆盖
        department.UpdatedBy = _auditContext.CurrentUserDisplayName;
        department.UpdatedAt = DateTime.Now;

        await _fsql.Update<Department>().SetSource(department).ExecuteAffrowsAsync();
        return ApiResponse.Ok("更新成功");
    }

    public async Task<ApiResponse> DeleteDepartmentAsync(Guid id)
    {
        var exists = await _fsql.Select<Department>().Where(d => d.Id == id).AnyAsync();
        if (!exists)
        {
            return ApiResponse.Fail(404, "科室不存在");
        }

        // 软删除
        await _fsql.Update<Department>()
            .Set(d => d.IsDeleted, true)
            .Set(d => d.DeletedBy, _auditContext.CurrentUserDisplayName)
            .Set(d => d.DeletedAt, DateTime.Now)
            .Where(d => d.Id == id)
            .ExecuteAffrowsAsync();
        return ApiResponse.Ok("删除成功");
    }

    #endregion

    #region Shift

    public async Task<ApiResponse<List<ShiftDto>>> GetShiftsAsync()
    {
        var items = await _fsql.Select<Shift>()
            .OrderBy(s => s.SortOrder)
            .ToListAsync();

        var dtos = _mapper.Map<List<ShiftDto>>(items);
        return ApiResponse<List<ShiftDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<ShiftDto>> GetShiftByIdAsync(Guid id)
    {
        var shift = await _fsql.Select<Shift>().Where(s => s.Id == id).FirstAsync();
        if (shift == null)
        {
            return ApiResponse<ShiftDto>.Fail(404, "班次不存在");
        }
        var dto = _mapper.Map<ShiftDto>(shift);
        return ApiResponse<ShiftDto>.Ok(dto);
    }

    public async Task<ApiResponse<ShiftDto>> CreateShiftAsync(ShiftDto dto)
    {
        var shift = _mapper.Map<Shift>(dto);
        shift.Id = Guid.NewGuid();
        shift.CreatedBy = _auditContext.CurrentUserDisplayName;
        shift.CreatedAt = DateTime.Now;
        shift.UpdatedBy = _auditContext.CurrentUserDisplayName;
        shift.UpdatedAt = DateTime.Now;

        await _fsql.Insert(shift).ExecuteAffrowsAsync();
        return await GetShiftByIdAsync(shift.Id);
    }

    public async Task<ApiResponse> UpdateShiftAsync(Guid id, ShiftDto dto)
    {
        var shift = await _fsql.Select<Shift>().Where(s => s.Id == id).FirstAsync();
        if (shift == null)
        {
            return ApiResponse.Fail(404, "班次不存在");
        }

        _mapper.Map(dto, shift);
        shift.Id = id; // 确保Id不被覆盖
        shift.UpdatedBy = _auditContext.CurrentUserDisplayName;
        shift.UpdatedAt = DateTime.Now;

        await _fsql.Update<Shift>().SetSource(shift).ExecuteAffrowsAsync();
        return ApiResponse.Ok("更新成功");
    }

    public async Task<ApiResponse> DeleteShiftAsync(Guid id)
    {
        var exists = await _fsql.Select<Shift>().Where(s => s.Id == id).AnyAsync();
        if (!exists)
        {
            return ApiResponse.Fail(404, "班次不存在");
        }

        // 软删除
        await _fsql.Update<Shift>()
            .Set(s => s.IsDeleted, true)
            .Set(s => s.DeletedBy, _auditContext.CurrentUserDisplayName)
            .Set(s => s.DeletedAt, DateTime.Now)
            .Where(s => s.Id == id)
            .ExecuteAffrowsAsync();
        return ApiResponse.Ok("删除成功");
    }

    #endregion

    #region PersonRank

    public async Task<ApiResponse<List<PersonRankDto>>> GetPersonRanksAsync()
    {
        var items = await _fsql.Select<PersonRank>()
            .OrderBy(r => r.SortOrder)
            .ToListAsync();

        var dtos = _mapper.Map<List<PersonRankDto>>(items);
        return ApiResponse<List<PersonRankDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<List<PersonRankDto>>> GetPersonRanksByCategoryAsync(string category)
    {
        var items = await _fsql.Select<PersonRank>()
            .Where(r => r.RankCategory == category)
            .OrderBy(r => r.SortOrder)
            .ToListAsync();

        var dtos = _mapper.Map<List<PersonRankDto>>(items);
        return ApiResponse<List<PersonRankDto>>.Ok(dtos);
    }

    #endregion

    #region PersonTitle

    public async Task<ApiResponse<List<PersonTitleDto>>> GetPersonTitlesAsync()
    {
        var items = await _fsql.Select<PersonTitle>()
            .OrderBy(t => t.SortOrder)
            .ToListAsync();

        var dtos = _mapper.Map<List<PersonTitleDto>>(items);
        return ApiResponse<List<PersonTitleDto>>.Ok(dtos);
    }

    #endregion

    #region Person

    public async Task<ApiResponse<PagedResult<PersonDto>>> GetPersonsPagedAsync(int page = 1, int pageSize = 20, string? keyword = null)
    {
        var query = _fsql.Select<Person>()
            .Include(p => p.Hospital)
            .Include(p => p.Department)
            .Include(p => p.Rank)
            .Include(p => p.Title);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(p => p.PersonName.Contains(keyword) || p.PersonCode.Contains(keyword));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Page(page, pageSize)
            .ToListAsync();

        var dtos = _mapper.Map<List<PersonDto>>(items);

        var result = new PagedResult<PersonDto>
        {
            Total = (int)total,
            Page = page,
            PageSize = pageSize,
            Items = dtos
        };

        return ApiResponse.OkPaged(result);
    }

    public async Task<ApiResponse<PersonDto>> GetPersonByIdAsync(Guid id)
    {
        var person = await _fsql.Select<Person>()
            .Include(p => p.Hospital)
            .Include(p => p.Department)
            .Include(p => p.Rank)
            .Include(p => p.Title)
            .Where(p => p.Id == id)
            .FirstAsync();

        if (person == null)
        {
            return ApiResponse<PersonDto>.Fail(404, "人员不存在");
        }

        var dto = _mapper.Map<PersonDto>(person);
        return ApiResponse<PersonDto>.Ok(dto);
    }

    public async Task<ApiResponse<List<PersonDto>>> GetPersonsByHospitalAsync(Guid hospitalId)
    {
        var items = await _fsql.Select<Person>()
            .Include(p => p.Department)
            .Include(p => p.Rank)
            .Include(p => p.Title)
            .Where(p => p.HospitalId == hospitalId)
            .ToListAsync();

        var dtos = _mapper.Map<List<PersonDto>>(items);
        return ApiResponse<List<PersonDto>>.Ok(dtos);
    }

    public async Task<ApiResponse> DeletePersonAsync(Guid id)
    {
        var exists = await _fsql.Select<Person>().Where(p => p.Id == id).AnyAsync();
        if (!exists)
        {
            return ApiResponse.Fail(404, "人员不存在");
        }

        // 软删除
        await _fsql.Update<Person>()
            .Set(p => p.IsDeleted, true)
            .Set(p => p.DeletedBy, _auditContext.CurrentUserDisplayName)
            .Set(p => p.DeletedAt, DateTime.Now)
            .Where(p => p.Id == id)
            .ExecuteAffrowsAsync();
        return ApiResponse.Ok("删除成功");
    }

    #endregion
}
