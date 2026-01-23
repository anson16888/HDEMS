using FreeSql;
using HDEMS.Application.DTOs;
using HDEMS.Application.Interfaces;
using HDEMS.Domain.Entities;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace HDEMS.Application.Services;

/// <summary>
/// 物资库存阈值服务实现
/// </summary>
public class MaterialThresholdService : IMaterialThresholdService
{
    private readonly IFreeSql _fsql;
    private readonly IMapper _mapper;
    private readonly ILogger<MaterialThresholdService> _logger;

    public MaterialThresholdService(IFreeSql fsql, IMapper mapper, ILogger<MaterialThresholdService> logger)
    {
        _fsql = fsql;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<MaterialThresholdDto>>> GetPagedAsync(MaterialThresholdQueryRequest request)
    {
        var query = _fsql.Select<MaterialThreshold>()
            .Include(t => t.MaterialType);

        // 条件过滤
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            query = query.Where(t => t.MaterialType!.TypeName.Contains(request.Keyword));
        }

        if (request.MaterialTypeId.HasValue)
        {
            query = query.Where(t => t.MaterialTypeId == request.MaterialTypeId.Value);
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

        var dtos = _mapper.Map<List<MaterialThresholdDto>>(items);

        var result = new PagedResult<MaterialThresholdDto>
        {
            Total = (int)total,
            Page = request.Page,
            PageSize = request.PageSize,
            Items = dtos
        };

        return ApiResponse.OkPaged(result);
    }

    public async Task<ApiResponse<MaterialThresholdDto>> GetByIdAsync(Guid id)
    {
        var threshold = await _fsql.Select<MaterialThreshold>()
            .Include(t => t.MaterialType)
            .Where(t => t.Id == id)
            .FirstAsync();

        if (threshold == null)
        {
            return ApiResponse<MaterialThresholdDto>.Fail(404, "阈值配置不存在");
        }

        var dto = _mapper.Map<MaterialThresholdDto>(threshold);
        return ApiResponse<MaterialThresholdDto>.Ok(dto);
    }

    public async Task<ApiResponse<MaterialThresholdDto>> CreateAsync(MaterialThresholdCreateRequest request)
    {
        // 验证物资类型是否存在
        var materialType = await _fsql.Select<MaterialTypeDict>()
            .Where(t => t.Id == request.MaterialTypeId && t.IsEnabled)
            .FirstAsync();

        if (materialType == null)
        {
            return ApiResponse<MaterialThresholdDto>.Fail(400, "物资类型不存在或已禁用");
        }

        // 检查该物资类型是否已存在阈值配置
        var exists = await _fsql.Select<MaterialThreshold>()
            .Where(t => t.MaterialTypeId == request.MaterialTypeId)
            .AnyAsync();

        if (exists)
        {
            return ApiResponse<MaterialThresholdDto>.Fail(400, "该物资类型已存在阈值配置");
        }

        var threshold = _mapper.Map<MaterialThreshold>(request);
        threshold.Id = Guid.NewGuid();
        threshold.CreatedAt = DateTime.Now;
        threshold.UpdatedAt = DateTime.Now;

        await _fsql.Insert(threshold).ExecuteAffrowsAsync();

        // 通过 MaterialTypeId 查询刚插入的记录
        var inserted = await _fsql.Select<MaterialThreshold>()
            .Include(t => t.MaterialType)
            .Where(t => t.MaterialTypeId == request.MaterialTypeId)
            .FirstAsync();

        if (inserted == null)
        {
            return ApiResponse<MaterialThresholdDto>.Fail(500, "创建失败");
        }

        var dto = _mapper.Map<MaterialThresholdDto>(inserted);
        return ApiResponse<MaterialThresholdDto>.Ok(dto, "创建成功");
    }

    public async Task<ApiResponse<MaterialThresholdDto>> UpdateAsync(Guid id, MaterialThresholdUpdateRequest request)
    {
        var threshold = await _fsql.Select<MaterialThreshold>().Where(t => t.Id == id).FirstAsync();
        if (threshold == null)
        {
            return ApiResponse<MaterialThresholdDto>.Fail(404, "阈值配置不存在");
        }

        // 验证物资类型是否存在
        var materialType = await _fsql.Select<MaterialTypeDict>()
            .Where(t => t.Id == request.MaterialTypeId && t.IsEnabled)
            .FirstAsync();

        if (materialType == null)
        {
            return ApiResponse<MaterialThresholdDto>.Fail(400, "物资类型不存在或已禁用");
        }

        // 如果修改了物资类型，检查是否冲突
        if (threshold.MaterialTypeId != request.MaterialTypeId)
        {
            var exists = await _fsql.Select<MaterialThreshold>()
                .Where(t => t.MaterialTypeId == request.MaterialTypeId && t.Id != id)
                .AnyAsync();

            if (exists)
            {
                return ApiResponse<MaterialThresholdDto>.Fail(400, "该物资类型已存在阈值配置");
            }
        }

        _mapper.Map(request, threshold);
        threshold.Id = id;
        threshold.UpdatedAt = DateTime.Now;

        await _fsql.Update<MaterialThreshold>()
            .SetSource(threshold)
            .ExecuteAffrowsAsync();

        var result = await GetByIdAsync(threshold.Id);
        return result;
    }

    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        var threshold = await _fsql.Select<MaterialThreshold>().Where(t => t.Id == id).FirstAsync();
        if (threshold == null)
        {
            return ApiResponse.Fail(404, "阈值配置不存在");
        }

        await _fsql.Delete<MaterialThreshold>(id).ExecuteAffrowsAsync();

        return ApiResponse.Ok("删除成功");
    }

    public async Task<ApiResponse> BatchDeleteAsync(List<Guid> ids)
    {
        await _fsql.Delete<MaterialThreshold>()
            .Where(t => ids.Contains(t.Id))
            .ExecuteAffrowsAsync();

        return ApiResponse.Ok($"成功删除 {ids.Count} 条记录");
    }

    public async Task<ApiResponse> ToggleEnabledAsync(Guid id)
    {
        var threshold = await _fsql.Select<MaterialThreshold>().Where(t => t.Id == id).FirstAsync();
        if (threshold == null)
        {
            return ApiResponse.Fail(404, "阈值配置不存在");
        }

        threshold.IsEnabled = !threshold.IsEnabled;
        threshold.UpdatedAt = DateTime.Now;

        await _fsql.Update<MaterialThreshold>()
            .SetSource(threshold)
            .ExecuteAffrowsAsync();

        return ApiResponse.Ok(threshold.IsEnabled ? "已启用" : "已禁用");
    }

    public async Task<ApiResponse<List<MaterialThresholdDto>>> GetEnabledThresholdsAsync()
    {
        var thresholds = await _fsql.Select<MaterialThreshold>()
            .Include(t => t.MaterialType)
            .Where(t => t.IsEnabled)
            .OrderBy(t => t.SortOrder)
            .ToListAsync();

        var dtos = _mapper.Map<List<MaterialThresholdDto>>(thresholds);

        return ApiResponse<List<MaterialThresholdDto>>.Ok(dtos);
    }
}
