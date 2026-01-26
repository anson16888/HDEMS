using FreeSql;
using HDEMS.Application.DTOs;
using HDEMS.Application.Interfaces;
using HDEMS.Domain.Entities;
using HDEMS.Infrastructure.Contexts;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace HDEMS.Application.Services;

/// <summary>
/// 物资类型服务实现
/// </summary>
public class MaterialTypeService : IMaterialTypeService
{
    private readonly IFreeSql _fsql;
    private readonly IMapper _mapper;
    private readonly ILogger<MaterialTypeService> _logger;
    private readonly AuditContext _auditContext;

    public MaterialTypeService(IFreeSql fsql, IMapper mapper, ILogger<MaterialTypeService> logger, AuditContext auditContext)
    {
        _fsql = fsql;
        _mapper = mapper;
        _logger = logger;
        _auditContext = auditContext;
    }

    public async Task<ApiResponse<PagedResult<MaterialTypeDto>>> GetPagedAsync(MaterialTypeQueryRequest request)
    {
        var query = _fsql.Select<MaterialTypeDict>();

        // 条件过滤
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            query = query.Where(t => t.TypeName.Contains(request.Keyword) || t.TypeCode.Contains(request.Keyword));
        }

        if (request.IsEnabled.HasValue)
        {
            query = query.Where(t => t.IsEnabled == request.IsEnabled.Value);
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(t => t.SortOrder)
            .Page(request.Page, request.PageSize)
            .ToListAsync();

        var dtos = _mapper.Map<List<MaterialTypeDto>>(items);

        var result = new PagedResult<MaterialTypeDto>
        {
            Total = (int)total,
            Page = request.Page,
            PageSize = request.PageSize,
            Items = dtos
        };

        return ApiResponse.OkPaged(result);
    }

    public async Task<ApiResponse<List<MaterialTypeOptionDto>>> GetEnabledTypesAsync()
    {
        var types = await _fsql.Select<MaterialTypeDict>()
            .Where(t => t.IsEnabled)
            .OrderBy(t => t.SortOrder)
            .ToListAsync();

        var options = _mapper.Map<List<MaterialTypeOptionDto>>(types);

        return ApiResponse<List<MaterialTypeOptionDto>>.Ok(options);
    }

    public async Task<ApiResponse<MaterialTypeDto>> GetByIdAsync(Guid id)
    {
        var type = await _fsql.Select<MaterialTypeDict>()
            .Where(t => t.Id == id)
            .FirstAsync();

        if (type == null)
        {
            return ApiResponse<MaterialTypeDto>.Fail(404, "物资类型不存在");
        }

        var dto = _mapper.Map<MaterialTypeDto>(type);
        return ApiResponse<MaterialTypeDto>.Ok(dto);
    }

    public async Task<ApiResponse<MaterialTypeDto>> GetByCodeAsync(string code)
    {
        var type = await _fsql.Select<MaterialTypeDict>()
            .Where(t => t.TypeCode == code)
            .FirstAsync();

        if (type == null)
        {
            return ApiResponse<MaterialTypeDto>.Fail(404, "物资类型不存在");
        }

        var dto = _mapper.Map<MaterialTypeDto>(type);
        return ApiResponse<MaterialTypeDto>.Ok(dto);
    }

    public async Task<ApiResponse<MaterialTypeDto>> CreateAsync(MaterialTypeCreateRequest request)
    {
        // 检查编码是否已存在
        var exists = await _fsql.Select<MaterialTypeDict>()
            .Where(t => t.TypeCode == request.TypeCode)
            .AnyAsync();

        if (exists)
        {
            return ApiResponse<MaterialTypeDto>.Fail(400, "类型编码已存在");
        }

        var type = _mapper.Map<MaterialTypeDict>(request);
        type.Id = Guid.NewGuid();
        type.CreatedBy = _auditContext.CurrentUserDisplayName;
        type.CreatedAt = DateTime.Now;

        // 插入数据
        await _fsql.Insert(type).ExecuteAffrowsAsync();

        // 通过 TypeCode 查询刚插入的记录
        var inserted = await _fsql.Select<MaterialTypeDict>()
            .Where(t => t.TypeCode == request.TypeCode)
            .FirstAsync();

        if (inserted == null)
        {
            return ApiResponse<MaterialTypeDto>.Fail(500, "创建失败");
        }

        var dto = _mapper.Map<MaterialTypeDto>(inserted);
        return ApiResponse<MaterialTypeDto>.Ok(dto, "创建成功");
    }

    public async Task<ApiResponse<MaterialTypeDto>> UpdateAsync(Guid id, MaterialTypeUpdateRequest request)
    {
        var type = await _fsql.Select<MaterialTypeDict>().Where(t => t.Id == id).FirstAsync();
        if (type == null)
        {
            return ApiResponse<MaterialTypeDto>.Fail(404, "物资类型不存在");
        }

        // 如果修改了编码，检查是否冲突
        if (type.TypeCode != request.TypeCode)
        {
            var codeExists = await _fsql.Select<MaterialTypeDict>()
                .Where(t => t.TypeCode == request.TypeCode && t.Id != id)
                .AnyAsync();

            if (codeExists)
            {
                return ApiResponse<MaterialTypeDto>.Fail(400, "类型编码已被其他类型使用");
            }
        }

        _mapper.Map(request, type);
        type.Id = id;
        type.UpdatedBy = _auditContext.CurrentUserDisplayName;
        type.UpdatedAt = DateTime.Now;

        await _fsql.Update<MaterialTypeDict>()
            .SetSource(type)
            .ExecuteAffrowsAsync();

        var result = await GetByIdAsync(type.Id);
        return result;
    }

    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        var type = await _fsql.Select<MaterialTypeDict>().Where(t => t.Id == id).FirstAsync();
        if (type == null)
        {
            return ApiResponse.Fail(404, "物资类型不存在");
        }

        // 检查是否有物资使用此类型
        var hasMaterials = await _fsql.Select<Domain.Entities.Material>()
            .Where(m => m.MaterialTypeId == id)
            .AnyAsync();

        if (hasMaterials)
        {
            return ApiResponse.Fail(400, "该类型下已有物资，无法删除");
        }

        // 软删除
        await _fsql.Update<MaterialTypeDict>()
            .Set(t => t.IsDeleted, true)
            .Set(t => t.DeletedBy, _auditContext.CurrentUserDisplayName)
            .Set(t => t.DeletedAt, DateTime.Now)
            .Where(t => t.Id == id)
            .ExecuteAffrowsAsync();

        return ApiResponse.Ok("删除成功");
    }

    public async Task<ApiResponse> BatchDeleteAsync(List<Guid> ids)
    {
        // 检查是否有物资使用这些类型
        var hasMaterials = await _fsql.Select<Domain.Entities.Material>()
            .Where(m => ids.Contains(m.MaterialTypeId))
            .AnyAsync();

        if (hasMaterials)
        {
            return ApiResponse.Fail(400, "部分类型下已有物资，无法删除");
        }

        // 软删除
        await _fsql.Update<MaterialTypeDict>()
            .Set(t => t.IsDeleted, true)
            .Set(t => t.DeletedBy, _auditContext.CurrentUserDisplayName)
            .Set(t => t.DeletedAt, DateTime.Now)
            .Where(t => ids.Contains(t.Id))
            .ExecuteAffrowsAsync();

        return ApiResponse.Ok($"成功删除 {ids.Count} 条记录");
    }

    public async Task<ApiResponse> ToggleEnabledAsync(Guid id)
    {
        var type = await _fsql.Select<MaterialTypeDict>().Where(t => t.Id == id).FirstAsync();
        if (type == null)
        {
            return ApiResponse.Fail(404, "物资类型不存在");
        }

        type.IsEnabled = !type.IsEnabled;
        type.UpdatedAt = DateTime.Now;

        await _fsql.Update<MaterialTypeDict>()
            .SetSource(type)
            .ExecuteAffrowsAsync();

        return ApiResponse.Ok(type.IsEnabled ? "已启用" : "已禁用");
    }
}
