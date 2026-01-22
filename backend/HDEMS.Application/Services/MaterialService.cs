using FreeSql;
using HDEMS.Application.DTOs;
using HDEMS.Application.Interfaces;
using HDEMS.Domain.Entities;
using HDEMS.Domain.Enums;
using HDEMS.Infrastructure.Configuration;
using HDEMS.Infrastructure.Services;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OfficeOpenXml;

namespace HDEMS.Application.Services;

/// <summary>
/// 物资服务实现
/// </summary>
public class MaterialService : IMaterialService
{
    private readonly IFreeSql _fsql;
    private readonly IMapper _mapper;
    private readonly PasswordService _passwordService;
    private readonly ILogger<MaterialService> _logger;
    private readonly SystemConfig _systemConfig;

    public MaterialService(IFreeSql fsql, IMapper mapper, PasswordService passwordService, ILogger<MaterialService> logger, IOptions<SystemConfig> systemConfig)
    {
        _fsql = fsql;
        _mapper = mapper;
        _passwordService = passwordService;
        _logger = logger;
        _systemConfig = systemConfig.Value;
    }

    public async Task<ApiResponse<PagedResult<MaterialDto>>> GetPagedAsync(MaterialQueryRequest request)
    {
        var query = _fsql.Select<Material>()
            .Include(m => m.Hospital);

        // 条件过滤
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            query = query.Where(m => m.MaterialName.Contains(request.Keyword) || m.MaterialCode.Contains(request.Keyword));
        }

        if (request.MaterialType.HasValue)
        {
            query = query.Where(m => m.MaterialType == request.MaterialType.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(m => m.Status == request.Status.Value);
        }

        if (request.HospitalId.HasValue)
        {
            query = query.Where(m => m.HospitalId == request.HospitalId.Value);
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(m => m.UpdatedAt)
            .Page(request.Page, request.PageSize)
            .ToListAsync();

        var dtos = _mapper.Map<List<MaterialDto>>(items);

        var result = new PagedResult<MaterialDto>
        {
            Total = (int)total,
            Page = request.Page,
            PageSize = request.PageSize,
            Items = dtos
        };

        return ApiResponse.OkPaged(result);
    }

    public async Task<ApiResponse<MaterialDto>> GetByIdAsync(Guid id)
    {
        var material = await _fsql.Select<Material>()
            .Include(m => m.Hospital)
            .Where(m => m.Id == id)
            .FirstAsync();

        if (material == null)
        {
            return ApiResponse<MaterialDto>.Fail(404, "物资不存在");
        }

        var dto = _mapper.Map<MaterialDto>(material);
        return ApiResponse<MaterialDto>.Ok(dto);
    }

    public async Task<ApiResponse<MaterialDto>> CreateAsync(MaterialCreateRequest request)
    {
        // 如果物资编码为空，自动生成
        string materialCode = request.MaterialCode;
        if (string.IsNullOrWhiteSpace(materialCode))
        {
            materialCode = await GenerateMaterialCodeAsync();
        }
        else
        {
            // 检查编码是否已存在
            var exists = await _fsql.Select<Material>()
                .Where(m => m.MaterialCode == materialCode)
                .AnyAsync();

            if (exists)
            {
                return ApiResponse<MaterialDto>.Fail(400, "物资编码已存在");
            }
        }

        var material = _mapper.Map<Material>(request);
        material.MaterialCode = materialCode;

        // 计算过期日期
        if (request.ProductionDate.HasValue && request.ShelfLife.HasValue)
        {
            material.ExpiryDate = request.ProductionDate.Value.AddMonths(request.ShelfLife.Value);
        }

        // 更新库存状态
        material.Status = GetMaterialStatus(material.Quantity);

        material.Id = Guid.NewGuid();
        material.CreatedAt = DateTime.Now;
        material.UpdatedAt = DateTime.Now;

        await _fsql.Insert(material).ExecuteAffrowsAsync();

        var result = await GetByIdAsync(material.Id);
        return result;
    }

    /// <summary>
    /// 生成物资编码：EM-YYMMDDHHMMSS-随机数
    /// </summary>
    private async Task<string> GenerateMaterialCodeAsync()
    {
        var now = DateTime.Now;
        var datePrefix = $"EM-{now:yyMMddHHmmss}";

        // 生成5位随机数（大写字母+数字）
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        string randomCode;

        // 确保生成的编码不重复
        do
        {
            randomCode = new string(Enumerable.Repeat(chars, 5)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        while (await _fsql.Select<Material>()
            .Where(m => m.MaterialCode == $"{datePrefix}-{randomCode}")
            .AnyAsync());

        return $"{datePrefix}-{randomCode}";
    }

    public async Task<ApiResponse<MaterialDto>> UpdateAsync(Guid id, MaterialCreateRequest request)
    {
        var material = await _fsql.Select<Material>().Where(m => m.Id == id).FirstAsync();
        if (material == null)
        {
            return ApiResponse<MaterialDto>.Fail(404, "物资不存在");
        }

        // 如果传入了新的物资编码，检查是否冲突
        if (!string.IsNullOrWhiteSpace(request.MaterialCode))
        {
            var codeExists = await _fsql.Select<Material>()
                .Where(m => m.MaterialCode == request.MaterialCode && m.Id != id)
                .AnyAsync();

            if (codeExists)
            {
                return ApiResponse<MaterialDto>.Fail(400, "物资编码已被其他物资使用");
            }
        }

        // 保存原编码，如果请求中的编码为空则保留原编码
        var originalCode = material.MaterialCode;
        _mapper.Map(request, material);
        if (string.IsNullOrWhiteSpace(request.MaterialCode))
        {
            material.MaterialCode = originalCode;
        }

        // 计算过期日期
        if (request.ProductionDate.HasValue && request.ShelfLife.HasValue)
        {
            material.ExpiryDate = request.ProductionDate.Value.AddMonths(request.ShelfLife.Value);
        }

        // 更新库存状态
        material.Status = GetMaterialStatus(material.Quantity);
        material.UpdatedAt = DateTime.Now;

        await _fsql.Update<Material>()
            .SetSource(material)
            .ExecuteAffrowsAsync();

        var result = await GetByIdAsync(material.Id);
        return result;
    }

    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        var material = await _fsql.Select<Material>().Where(m => m.Id == id).FirstAsync();
        if (material == null)
        {
            return ApiResponse.Fail(404, "物资不存在");
        }

        // 软删除
        material.DeletedAt = DateTime.Now;
        await _fsql.Update<Material>()
            .SetSource(material)
            .ExecuteAffrowsAsync();

        return ApiResponse.Ok("删除成功");
    }

    public async Task<ApiResponse> BatchDeleteAsync(List<Guid> ids)
    {
        await _fsql.Update<Material>()
            .Set(m => m.DeletedAt, DateTime.Now)
            .Where(m => ids.Contains(m.Id))
            .ExecuteAffrowsAsync();

        return ApiResponse.Ok($"成功删除 {ids.Count} 条记录");
    }

    public async Task<ApiResponse<MaterialImportResult>> ImportAsync(Stream fileStream, string fileName)
    {
        var result = new MaterialImportResult();

        using var package = new ExcelPackage(fileStream);
        var worksheet = package.Workbook.Worksheets[0];

        if (worksheet == null)
        {
            return ApiResponse<MaterialImportResult>.Fail(400, "无效的Excel文件");
        }

        var rowCount = worksheet.Dimension.Rows;
        result.TotalCount = rowCount - 3; // 减去标题和说明行

        // 从配置获取医院名称
        var hospitalName = _systemConfig.HospitalName ?? "宝安人民医院";
        var hospital = await _fsql.Select<Hospital>().Where(h => h.HospitalName == hospitalName).FirstAsync();

        if (hospital == null)
        {
            return ApiResponse<MaterialImportResult>.Fail(400, $"系统配置的医院 '{hospitalName}' 不存在，请先在系统中创建该医院");
        }

        for (int row = 4; row <= rowCount; row++)
        {
            try
            {
                // 去掉医院列，列号调整：1-编码,2-名称,3-类型,4-数量,5-单位,6-位置,7-规格,8-生产日期,9-质保期,10-备注
                var materialCode = worksheet.Cells[row, 1].Text?.Trim();
                var materialName = worksheet.Cells[row, 2].Text?.Trim();
                var materialTypeName = worksheet.Cells[row, 3].Text?.Trim();
                var quantityText = worksheet.Cells[row, 4].Text?.Trim();
                var unit = worksheet.Cells[row, 5].Text?.Trim();
                var location = worksheet.Cells[row, 6].Text?.Trim();
                var specification = worksheet.Cells[row, 7].Text?.Trim();
                var productionDateStr = worksheet.Cells[row, 8].Text?.Trim();
                var shelfLifeStr = worksheet.Cells[row, 9].Text?.Trim();
                var remark = worksheet.Cells[row, 10].Text?.Trim();

                // 验证必填项
                if (string.IsNullOrWhiteSpace(materialName))
                {
                    result.Errors.Add(new MaterialImportError { RowNumber = row, ErrorMessage = "物资名称不能为空" });
                    result.FailedCount++;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(quantityText) || !decimal.TryParse(quantityText, out var quantity))
                {
                    result.Errors.Add(new MaterialImportError { RowNumber = row, ErrorMessage = "库存数量格式不正确" });
                    result.FailedCount++;
                    continue;
                }

                // 解析物资类型（中文转枚举）
                MaterialType materialType = materialTypeName switch
                {
                    "食品" => MaterialType.Food,
                    "医疗" => MaterialType.Medical,
                    "设备" => MaterialType.Equipment,
                    "衣物" => MaterialType.Clothing,
                    "其他" => MaterialType.Other,
                    _ => Enum.TryParse<MaterialType>(materialTypeName, out var parsedType) ? parsedType : MaterialType.Other
                };

                // 如果物资编码为空，自动生成
                if (string.IsNullOrWhiteSpace(materialCode))
                {
                    materialCode = await GenerateMaterialCodeAsync();
                }

                // 检查是否已存在
                var exists = await _fsql.Select<Material>()
                    .Where(m => m.MaterialCode == materialCode)
                    .AnyAsync();

                if (exists)
                {
                    result.Errors.Add(new MaterialImportError { RowNumber = row, ErrorMessage = $"物资编码 '{materialCode}' 已存在" });
                    result.FailedCount++;
                    continue;
                }

                var material = new Material
                {
                    Id = Guid.NewGuid(),
                    MaterialCode = materialCode,
                    MaterialName = materialName,
                    MaterialType = materialType,
                    Quantity = quantity,
                    Unit = unit,
                    Location = location,
                    HospitalId = hospital.Id,
                    Specification = specification,
                    Remark = remark,
                    Status = GetMaterialStatus(quantity),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                // 解析生产日期和质保期
                if (DateTime.TryParse(productionDateStr, out var productionDate))
                {
                    material.ProductionDate = productionDate;
                    if (int.TryParse(shelfLifeStr, out var shelfLife))
                    {
                        material.ShelfLife = shelfLife;
                        material.ExpiryDate = productionDate.AddMonths(shelfLife);
                    }
                }

                await _fsql.Insert(material).ExecuteAffrowsAsync();
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

    public async Task<ApiResponse<byte[]>> ExportAsync(MaterialQueryRequest request)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("物资数据");

        // 表头
        worksheet.Cells[1, 1].Value = "物资编码";
        worksheet.Cells[1, 2].Value = "物资名称";
        worksheet.Cells[1, 3].Value = "物资类型";
        worksheet.Cells[1, 4].Value = "规格";
        worksheet.Cells[1, 5].Value = "库存数量";
        worksheet.Cells[1, 6].Value = "单位";
        worksheet.Cells[1, 7].Value = "生产日期";
        worksheet.Cells[1, 8].Value = "质保期(月)";
        worksheet.Cells[1, 9].Value = "存放位置";
        worksheet.Cells[1, 10].Value = "医院";
        worksheet.Cells[1, 11].Value = "状态";
        worksheet.Cells[1, 12].Value = "备注";

        // 设置表头样式
        using (var range = worksheet.Cells[1, 1, 1, 12])
        {
            range.Style.Font.Bold = true;
        }

        // 获取数据
        var query = _fsql.Select<Material>().Include(m => m.Hospital);
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            query = query.Where(m => m.MaterialName.Contains(request.Keyword) || m.MaterialCode.Contains(request.Keyword));
        }
        if (request.MaterialType.HasValue)
        {
            query = query.Where(m => m.MaterialType == request.MaterialType.Value);
        }
        if (request.HospitalId.HasValue)
        {
            query = query.Where(m => m.HospitalId == request.HospitalId.Value);
        }

        var materials = await query.OrderByDescending(m => m.UpdatedAt).Take(10000).ToListAsync();

        // 填充数据
        int row = 2;
        foreach (var material in materials)
        {
            worksheet.Cells[row, 1].Value = material.MaterialCode;
            worksheet.Cells[row, 2].Value = material.MaterialName;
            worksheet.Cells[row, 3].Value = material.MaterialType.ToString();
            worksheet.Cells[row, 4].Value = material.Specification;
            worksheet.Cells[row, 5].Value = material.Quantity.ToString();
            worksheet.Cells[row, 6].Value = material.Unit;
            worksheet.Cells[row, 7].Value = material.ProductionDate?.ToString("yyyy-MM-dd");
            worksheet.Cells[row, 8].Value = material.ShelfLife?.ToString();
            worksheet.Cells[row, 9].Value = material.Location;
            worksheet.Cells[row, 10].Value = material.Hospital?.HospitalName;
            worksheet.Cells[row, 11].Value = material.Status.ToString();
            worksheet.Cells[row, 12].Value = material.Remark;
            row++;
        }

        // 自动调整列宽
        worksheet.Cells.AutoFitColumns();

        return ApiResponse<byte[]>.Ok(package.GetAsByteArray(), "导出成功");
    }

    public async Task<ApiResponse<object>> GetStatisticsAsync(Guid? hospitalId = null)
    {
        var query = _fsql.Select<Material>();

        if (hospitalId.HasValue)
        {
            query = query.Where(m => m.HospitalId == hospitalId.Value);
        }

        var total = await query.CountAsync();
        var normal = await query.Where(m => m.Status == MaterialStatus.Normal).CountAsync();
        var low = await query.Where(m => m.Status == MaterialStatus.Low).CountAsync();
        var outStock = await query.Where(m => m.Status == MaterialStatus.Out).CountAsync();

        // 获取所有物资然后内存分组
        var allMaterials = await query.ToListAsync();
        var typeStats = allMaterials
            .GroupBy(m => m.MaterialType)
            .Select(g => new
            {
                Type = g.Key,
                Count = g.Count(),
                Quantity = g.Sum(x => x.Quantity)
            })
            .ToList();

        return ApiResponse<object>.Ok(new
        {
            Total = total,
            Normal = normal,
            Low = low,
            Out = outStock,
            TypeStatistics = typeStats
        });
    }

    private MaterialStatus GetMaterialStatus(decimal quantity)
    {
        return quantity switch
        {
            0 => MaterialStatus.Out,
            <= 5 => MaterialStatus.Low,
            _ => MaterialStatus.Normal
        };
    }
}
