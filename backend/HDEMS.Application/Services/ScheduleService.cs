using FreeSql;
using HDEMS.Application.DTOs;
using HDEMS.Application.Interfaces;
using HDEMS.Domain.Entities;
using HDEMS.Domain.Enums;
using HDEMS.Infrastructure.Services;
using HDEMS.Infrastructure.Contexts;
using HDEMS.Infrastructure.Configuration;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OfficeOpenXml;

namespace HDEMS.Application.Services;

/// <summary>
/// 排班服务实现
/// </summary>
public class ScheduleService : IScheduleService
{
    private readonly IFreeSql _fsql;
    private readonly IMapper _mapper;
    private readonly ILogger<ScheduleService> _logger;
    private readonly AuditContext _auditContext;
    private readonly SystemConfig _systemConfig;

    public ScheduleService(IFreeSql fsql, IMapper mapper, ILogger<ScheduleService> logger, AuditContext auditContext, IOptions<SystemConfig> systemConfig)
    {
        _fsql = fsql;
        _mapper = mapper;
        _logger = logger;
        _auditContext = auditContext;
        _systemConfig = systemConfig.Value;
    }

    public async Task<ApiResponse<PagedResult<ScheduleDto>>> GetPagedAsync(ScheduleQueryRequest request)
    {
        var query = _fsql.Select<Schedule>()
            .Include(s => s.Shift)
            .Include(s => s.Rank)
            .Include(s => s.Department)
            .Include(s => s.Title)
            .Include(s => s.Hospital);

        // 条件过滤
        if (request.StartDate.HasValue)
        {
            query = query.Where(s => s.ScheduleDate >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(s => s.ScheduleDate <= request.EndDate.Value);
        }

        if (request.ScheduleType.HasValue)
        {
            query = query.Where(s => s.ScheduleType == request.ScheduleType.Value);
        }

        if (request.DepartmentId.HasValue)
        {
            query = query.Where(s => s.DepartmentId == request.DepartmentId.Value);
        }

        if (request.ShiftId.HasValue)
        {
            query = query.Where(s => s.ShiftId == request.ShiftId.Value);
        }

        if (request.HospitalId.HasValue)
        {
            query = query.Where(s => s.HospitalId == request.HospitalId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            query = query.Where(s => s.PersonName.Contains(request.Keyword));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(s => s.ScheduleDate)
            .Page(request.Page, request.PageSize)
            .ToListAsync();

        var dtos = _mapper.Map<List<ScheduleDto>>(items);

        // 填充医院名称
        for (int i = 0; i < dtos.Count; i++)
        {
            // 如果 hospitalId 是 Guid.Empty，则使用配置中的系统组织名称
            if (items[i].HospitalId == Guid.Empty || items[i].HospitalId == null)
            {
                dtos[i].HospitalName = _systemConfig.SystemOrgName ?? "宝安卫健局";
            }
            else
            {
                dtos[i].HospitalName = items[i].Hospital?.HospitalName;
            }
        }

        var result = new PagedResult<ScheduleDto>
        {
            Total = (int)total,
            Page = request.Page,
            PageSize = request.PageSize,
            Items = dtos
        };

        return ApiResponse.OkPaged(result);
    }

    public async Task<ApiResponse<ScheduleDto>> GetByIdAsync(Guid id)
    {
        var schedule = await _fsql.Select<Schedule>()
            .Include(s => s.Shift)
            .Include(s => s.Rank)
            .Include(s => s.Department)
            .Include(s => s.Title)
            .Include(s => s.Hospital)
            .Where(s => s.Id == id)
            .FirstAsync();

        if (schedule == null)
        {
            return ApiResponse<ScheduleDto>.Fail(404, "排班记录不存在");
        }

        var dto = _mapper.Map<ScheduleDto>(schedule);
        // 如果 hospitalId 是 Guid.Empty，则使用配置中的系统组织名称
        if (schedule.HospitalId == Guid.Empty || schedule.HospitalId == null)
        {
            dto.HospitalName = _systemConfig.SystemOrgName ?? "宝安卫健局";
        }
        else
        {
            dto.HospitalName = schedule.Hospital?.HospitalName;
        }
        return ApiResponse<ScheduleDto>.Ok(dto);
    }

    public async Task<ApiResponse<ScheduleDto>> CreateAsync(ScheduleCreateRequest request)
    {
        // 检查是否已存在相同排班
        var exists = await _fsql.Select<Schedule>()
            .Where(s => s.ScheduleDate == request.ScheduleDate
                && s.ScheduleType == request.ScheduleType
                && s.ShiftId == request.ShiftId
                && s.PersonName == request.PersonName)
            .AnyAsync();

        if (exists)
        {
            return ApiResponse<ScheduleDto>.Fail(400, "该时间段已存在排班记录");
        }

        var schedule = _mapper.Map<Schedule>(request);
        schedule.Id = Guid.NewGuid();
        // 如果传了医院ID则使用传参的医院，否则使用当前用户所属医院
        schedule.HospitalId = request.HospitalId ?? _auditContext.CurrentHospitalId;
        schedule.CreatedBy = _auditContext.CurrentUserDisplayName;
        schedule.CreatedAt = DateTime.Now;

        await _fsql.Insert(schedule).ExecuteAffrowsAsync();

        var result = await GetByIdAsync(schedule.Id);
        return result;
    }

    public async Task<ApiResponse<List<ScheduleDto>>> BatchCreateAsync(List<ScheduleCreateRequest> requests)
    {
        var results = new List<ScheduleDto>();

        foreach (var request in requests)
        {
            var result = await CreateAsync(request);
            if (result.Success && result.Data != null)
            {
                results.Add(result.Data);
            }
        }

        return ApiResponse<List<ScheduleDto>>.Ok(results, $"成功创建 {results.Count} 条排班记录");
    }

    public async Task<ApiResponse<ScheduleDto>> UpdateAsync(Guid id, ScheduleCreateRequest request)
    {
        var schedule = await _fsql.Select<Schedule>().Where(s => s.Id == id).FirstAsync();
        if (schedule == null)
        {
            return ApiResponse<ScheduleDto>.Fail(404, "排班记录不存在");
        }

        _mapper.Map(request, schedule);
        // 如果传了医院ID则使用传参的医院，否则使用当前用户所属医院
        schedule.HospitalId = request.HospitalId ?? _auditContext.CurrentHospitalId;
        schedule.UpdatedBy = _auditContext.CurrentUserDisplayName;
        schedule.UpdatedAt = DateTime.Now;

        await _fsql.Update<Schedule>()
            .SetSource(schedule)
            .ExecuteAffrowsAsync();

        var result = await GetByIdAsync(schedule.Id);
        return result;
    }

    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        var schedule = await _fsql.Select<Schedule>().Where(s => s.Id == id).FirstAsync();
        if (schedule == null)
        {
            return ApiResponse.Fail(404, "排班记录不存在");
        }

        // 软删除
        await _fsql.Update<Schedule>()
            .Set(s => s.IsDeleted, true)
            .Set(s => s.DeletedBy, _auditContext.CurrentUserDisplayName)
            .Set(s => s.DeletedAt, DateTime.Now)
            .Where(s => s.Id == id)
            .ExecuteAffrowsAsync();
        return ApiResponse.Ok("删除成功");
    }

    public async Task<ApiResponse> BatchDeleteAsync(List<Guid> ids)
    {
        // 软删除
        await _fsql.Update<Schedule>()
            .Set(s => s.IsDeleted, true)
            .Set(s => s.DeletedBy, _auditContext.CurrentUserDisplayName)
            .Set(s => s.DeletedAt, DateTime.Now)
            .Where(s => ids.Contains(s.Id))
            .ExecuteAffrowsAsync();
        return ApiResponse.Ok($"成功删除 {ids.Count} 条记录");
    }

    public async Task<ApiResponse<PagedResult<ScheduleOverviewItem>>> GetOverviewAsync(ScheduleQueryRequest request)
    {
        var query = _fsql.Select<Schedule>()
            .Include(s => s.Shift)
            .Include(s => s.Rank)
            .Include(s => s.Department)
            .Include(s => s.Hospital)
            .Include(s => s.Title);

        // 条件过滤
        if (request.StartDate.HasValue)
        {
            query = query.Where(s => s.ScheduleDate >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(s => s.ScheduleDate <= request.EndDate.Value);
        }

        if (request.ScheduleType.HasValue)
        {
            query = query.Where(s => s.ScheduleType == request.ScheduleType.Value);
        }

        if (request.DepartmentId.HasValue)
        {
            query = query.Where(s => s.DepartmentId == request.DepartmentId.Value);
        }

        if (request.HospitalId.HasValue)
        {
            query = query.Where(s => s.HospitalId == request.HospitalId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            query = query.Where(s => s.PersonName.Contains(request.Keyword));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(s => s.ScheduleDate)
            .Page(request.Page, request.PageSize)
            .ToListAsync();

        // 获取排班类型中文名称映射
        var overviewItems = items.Select(s => new ScheduleOverviewItem
        {
            ScheduleDate = s.ScheduleDate,
            DepartmentName = s.Department?.DepartmentName ?? "",
            HospitalName = s.Hospital?.HospitalName ?? "",
            ScheduleType = s.ScheduleType,
            ScheduleTypeName = s.ScheduleType switch
            {
                ScheduleType.Bureau => "局级行政",
                ScheduleType.Hospital => "院级行政",
                ScheduleType.Director => "院内主任",
                _ => s.ScheduleType.ToString()
            },
            ShiftName = s.Shift?.ShiftName ?? "",
            PersonName = s.PersonName,
            RankName = s.Rank?.RankName,
            TitleName = s.Title?.TitleName,
            Phone = s.Phone,
            Remark = s.Remark
        }).ToList();

        var result = new PagedResult<ScheduleOverviewItem>
        {
            Total = (int)total,
            Page = request.Page,
            PageSize = request.PageSize,
            Items = overviewItems
        };

        return ApiResponse.OkPaged(result);
    }

    public async Task<ApiResponse<ScheduleStatistics>> GetStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null, Guid? hospitalId = null)
    {
        // 辅助函数：创建带有日期和医院过滤条件的查询
        ISelect<Schedule> CreateQuery()
        {
            var query = _fsql.Select<Schedule>();
            if (startDate.HasValue)
            {
                query = query.Where(s => s.ScheduleDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(s => s.ScheduleDate <= endDate.Value);
            }
            if (hospitalId.HasValue)
            {
                query = query.Where(s => s.HospitalId == hospitalId.Value);
            }
            return query;
        }

        var total = await CreateQuery().CountAsync();
        var bureauCount = await CreateQuery().Where(s => s.ScheduleType == ScheduleType.Bureau).CountAsync();
        var hospitalCount = await CreateQuery().Where(s => s.ScheduleType == ScheduleType.Hospital).CountAsync();
        var directorCount = await CreateQuery().Where(s => s.ScheduleType == ScheduleType.Director).CountAsync();

        var stats = new ScheduleStatistics
        {
            TotalCount = (int)total,
            BureauCount = (int)bureauCount,
            HospitalCount = (int)hospitalCount,
            DirectorCount = (int)directorCount
        };

        return ApiResponse<ScheduleStatistics>.Ok(stats);
    }

    public async Task<ApiResponse<byte[]>> ExportAsync(ScheduleQueryRequest request)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("排班数据");

        // 表头
        worksheet.Cells[1, 1].Value = "日期";
        worksheet.Cells[1, 2].Value = "排班类型";
        worksheet.Cells[1, 3].Value = "医院";
        worksheet.Cells[1, 4].Value = "班次";
        worksheet.Cells[1, 5].Value = "人员姓名";
        worksheet.Cells[1, 6].Value = "联系电话";
        worksheet.Cells[1, 7].Value = "职级";
        worksheet.Cells[1, 8].Value = "科室";
        worksheet.Cells[1, 9].Value = "职称";
        worksheet.Cells[1, 10].Value = "备注";

        // 设置表头样式
        using (var range = worksheet.Cells[1, 1, 1, 10])
        {
            range.Style.Font.Bold = true;
        }

        // 获取数据
        var query = _fsql.Select<Schedule>()
            .Include(s => s.Shift)
            .Include(s => s.Rank)
            .Include(s => s.Department)
            .Include(s => s.Title)
            .Include(s => s.Hospital);

        if (request.StartDate.HasValue)
        {
            query = query.Where(s => s.ScheduleDate >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(s => s.ScheduleDate <= request.EndDate.Value);
        }

        if (request.ScheduleType.HasValue)
        {
            query = query.Where(s => s.ScheduleType == request.ScheduleType.Value);
        }

        if (request.HospitalId.HasValue)
        {
            query = query.Where(s => s.HospitalId == request.HospitalId.Value);
        }

        var schedules = await query
            .OrderByDescending(s => s.ScheduleDate)
            .Take(10000)
            .ToListAsync();

        // 填充数据
        int row = 2;
        foreach (var schedule in schedules)
        {
            // 获取排班类型中文名称
            var scheduleTypeName = schedule.ScheduleType switch
            {
                ScheduleType.Bureau => "局级行政",
                ScheduleType.Hospital => "院级行政",
                ScheduleType.Director => "院内主任",
                _ => schedule.ScheduleType.ToString()
            };

            worksheet.Cells[row, 1].Value = schedule.ScheduleDate.ToString("yyyy-MM-dd");
            worksheet.Cells[row, 2].Value = scheduleTypeName;
            // 如果 hospitalId 是 Guid.Empty，则使用配置中的系统组织名称
            worksheet.Cells[row, 3].Value = (schedule.HospitalId == Guid.Empty || schedule.HospitalId == null)
                ? (_systemConfig.SystemOrgName ?? "宝安卫健局")
                : (schedule.Hospital?.HospitalName ?? "");
            worksheet.Cells[row, 4].Value = schedule.Shift?.ShiftName;
            worksheet.Cells[row, 5].Value = schedule.PersonName;
            worksheet.Cells[row, 6].Value = schedule.Phone;
            worksheet.Cells[row, 7].Value = schedule.Rank?.RankName;
            worksheet.Cells[row, 8].Value = schedule.Department?.DepartmentName;
            worksheet.Cells[row, 9].Value = schedule.Title?.TitleName;
            worksheet.Cells[row, 10].Value = schedule.Remark;
            row++;
        }

        worksheet.Cells.AutoFitColumns();

        return ApiResponse<byte[]>.Ok(package.GetAsByteArray(), "导出成功");
    }

    public async Task<ApiResponse<MaterialImportResult>> ImportAsync(Stream fileStream, string fileName, ScheduleType scheduleType)
    {
        var result = new MaterialImportResult();

        using var package = new ExcelPackage(fileStream);
        var worksheet = package.Workbook.Worksheets[0];

        if (worksheet == null)
        {
            return ApiResponse<MaterialImportResult>.Fail(400, "无效的Excel文件");
        }

        var rowCount = worksheet.Dimension.Rows;
        result.TotalCount = rowCount - 2; // 减去标题+说明行和表头行

        var shifts = await _fsql.Select<Shift>().ToListAsync();
        var shiftDict = shifts.ToDictionary(s => s.ShiftName, s => s.Id);

        var departments = await _fsql.Select<Department>().ToListAsync();
        var departmentDict = departments.ToDictionary(d => d.DepartmentName, d => d.Id);

        var ranks = await _fsql.Select<PersonRank>().ToListAsync();
        var rankDict = ranks.ToDictionary(r => r.RankName, r => r.Id);

        var titles = await _fsql.Select<PersonTitle>().ToListAsync();
        var titleDict = titles.ToDictionary(t => t.TitleName, t => t.Id);

        // 辅助方法：获取或创建班次
        async Task<Guid> GetOrCreateShift(string name)
        {
            if (shiftDict.TryGetValue(name, out var id))
                return id;

            // 自动创建新班次
            var newShift = new Shift
            {
                Id = Guid.NewGuid(),
                ShiftCode = name, // 使用名称作为编码
                ShiftName = name,
                SortOrder = shifts.Count + 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CreatedBy = _auditContext.CurrentUserDisplayName,
                UpdatedBy = _auditContext.CurrentUserDisplayName
            };
            await _fsql.Insert(newShift).ExecuteAffrowsAsync();
            shiftDict[name] = newShift.Id;
            shifts.Add(newShift);
            return newShift.Id;
        }

        // 辅助方法：获取或创建职级
        async Task<Guid?> GetOrCreateRank(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            if (rankDict.TryGetValue(name, out var id))
                return id;

            // 自动创建新职级
            var newRank = new PersonRank
            {
                Id = Guid.NewGuid(),
                RankCode = name, // 使用名称作为编码
                RankName = name,
                SortOrder = ranks.Count + 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CreatedBy = _auditContext.CurrentUserDisplayName,
                UpdatedBy = _auditContext.CurrentUserDisplayName
            };
            await _fsql.Insert(newRank).ExecuteAffrowsAsync();
            rankDict[name] = newRank.Id;
            ranks.Add(newRank);
            return newRank.Id;
        }

        // 辅助方法：获取或创建科室
        async Task<Guid?> GetOrCreateDepartment(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            if (departmentDict.TryGetValue(name, out var id))
                return id;

            // 自动创建新科室
            var newDept = new Department
            {
                Id = Guid.NewGuid(),
                DepartmentCode = name, // 使用名称作为编码
                DepartmentName = name,
                SortOrder = departments.Count + 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CreatedBy = _auditContext.CurrentUserDisplayName,
                UpdatedBy = _auditContext.CurrentUserDisplayName
            };
            await _fsql.Insert(newDept).ExecuteAffrowsAsync();
            departmentDict[name] = newDept.Id;
            departments.Add(newDept);
            return newDept.Id;
        }

        // 辅助方法：获取或创建职称
        async Task<Guid?> GetOrCreateTitle(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            if (titleDict.TryGetValue(name, out var id))
                return id;

            // 自动创建新职称
            var newTitle = new PersonTitle
            {
                Id = Guid.NewGuid(),
                TitleCode = name, // 使用名称作为编码
                TitleName = name,
                SortOrder = titles.Count + 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CreatedBy = _auditContext.CurrentUserDisplayName,
                UpdatedBy = _auditContext.CurrentUserDisplayName
            };
            await _fsql.Insert(newTitle).ExecuteAffrowsAsync();
            titleDict[name] = newTitle.Id;
            titles.Add(newTitle);
            return newTitle.Id;
        }

        for (int row = 3; row <= rowCount; row++)
        {
            try
            {
                // 去掉医院列，列号调整：1-日期,2-班次,3-姓名,4-电话,5-职级,6-科室,7-职称,8-备注
                var dateStr = worksheet.Cells[row, 1].Text;
                var shiftName = worksheet.Cells[row, 2].Text;
                var personName = worksheet.Cells[row, 3].Text;
                var phone = worksheet.Cells[row, 4].Text;
                var rankName = worksheet.Cells[row, 5].Text;
                var departmentName = worksheet.Cells[row, 6].Text;
                var titleName = worksheet.Cells[row, 7].Text;
                var remark = worksheet.Cells[row, 8].Text;

                if (!DateTime.TryParse(dateStr, out var scheduleDate))
                {
                    result.Errors.Add(new MaterialImportError { RowNumber = row, ErrorMessage = "日期格式不正确" });
                    result.FailedCount++;
                    continue;
                }

                // 原来的班次验证代码已注释，改为自动创建
                //if (!shiftDict.ContainsKey(shiftName))
                //{
                //    result.Errors.Add(new MaterialImportError { RowNumber = row, ErrorMessage = $"班次 '{shiftName}' 不存在" });
                //    result.FailedCount++;
                //    continue;
                //}

                var schedule = new Schedule
                {
                    Id = Guid.NewGuid(),
                    ScheduleDate = scheduleDate,
                    ScheduleType = scheduleType,
                    ShiftId = await GetOrCreateShift(shiftName),
                    PersonName = personName,
                    Phone = phone,
                    RankId = await GetOrCreateRank(rankName),
                    DepartmentId = await GetOrCreateDepartment(departmentName),
                    TitleId = await GetOrCreateTitle(titleName),
                    Remark = remark,
                    HospitalId = _auditContext.CurrentHospitalId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _auditContext.CurrentUserDisplayName
                };

                await _fsql.Insert(schedule).ExecuteAffrowsAsync();
                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                result.Errors.Add(new MaterialImportError { RowNumber = row, ErrorMessage = ex.Message });
                result.FailedCount++;
            }
        }

        return ApiResponse<MaterialImportResult>.Ok(result, $"导入完成，成功 {result.SuccessCount} 条，失败 {result.FailedCount} 条");
    }
}
