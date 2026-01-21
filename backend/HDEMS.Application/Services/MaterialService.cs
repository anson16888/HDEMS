using FreeSql;
using HDEMS.Application.DTOs;
using HDEMS.Application.Interfaces;
using HDEMS.Domain.Entities;
using HDEMS.Domain.Enums;
using HDEMS.Infrastructure.Services;
using AutoMapper;
using Microsoft.Extensions.Logging;
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

    public MaterialService(IFreeSql fsql, IMapper mapper, PasswordService passwordService, ILogger<MaterialService> logger)
    {
        _fsql = fsql;
        _mapper = mapper;
        _passwordService = passwordService;
        _logger = logger;
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
        // 检查编码是否已存在
        var exists = await _fsql.Select<Material>()
            .Where(m => m.MaterialCode == request.MaterialCode)
            .AnyAsync();

        if (exists)
        {
            return ApiResponse<MaterialDto>.Fail(400, "物资编码已存在");
        }

        var material = _mapper.Map<Material>(request);

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

    public async Task<ApiResponse<MaterialDto>> UpdateAsync(Guid id, MaterialCreateRequest request)
    {
        var material = await _fsql.Select<Material>().Where(m => m.Id == id).FirstAsync();
        if (material == null)
        {
            return ApiResponse<MaterialDto>.Fail(404, "物资不存在");
        }

        // 检查编码是否与其他物资冲突
        var codeExists = await _fsql.Select<Material>()
            .Where(m => m.MaterialCode == request.MaterialCode && m.Id != id)
            .AnyAsync();

        if (codeExists)
        {
            return ApiResponse<MaterialDto>.Fail(400, "物资编码已被其他物资使用");
        }

        _mapper.Map(request, material);

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
        result.TotalCount = rowCount - 1; // 减去表头

        var hospitals = await _fsql.Select<Hospital>().ToListAsync();
        var hospitalDict = hospitals.ToDictionary(h => h.HospitalName, h => h.Id);

        for (int row = 2; row <= rowCount; row++)
        {
            try
            {
                var materialCode = worksheet.Cells[row, 1].Text;
                var materialName = worksheet.Cells[row, 2].Text;
                var materialTypeName = worksheet.Cells[row, 3].Text;
                var quantity = decimal.Parse(worksheet.Cells[row, 4].Text);
                var unit = worksheet.Cells[row, 5].Text;
                var location = worksheet.Cells[row, 6].Text;
                var hospitalName = worksheet.Cells[row, 7].Text;
                var specification = worksheet.Cells[row, 8].Text;
                var productionDateStr = worksheet.Cells[row, 9].Text;
                var shelfLifeStr = worksheet.Cells[row, 10].Text;
                var remark = worksheet.Cells[row, 11].Text;

                // 验证必填项
                if (string.IsNullOrWhiteSpace(materialCode) || string.IsNullOrWhiteSpace(materialName))
                {
                    result.Errors.Add(new MaterialImportError { RowNumber = row, ErrorMessage = "物资编码和名称不能为空" });
                    result.FailedCount++;
                    continue;
                }

                // 检查医院
                if (!hospitalDict.ContainsKey(hospitalName))
                {
                    result.Errors.Add(new MaterialImportError { RowNumber = row, ErrorMessage = $"医院 '{hospitalName}' 不存在" });
                    result.FailedCount++;
                    continue;
                }

                // 解析物资类型
                if (!Enum.TryParse<MaterialType>(materialTypeName, out var materialType))
                {
                    materialType = MaterialType.Other;
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
                    HospitalId = hospitalDict[hospitalName],
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
            //range.Style.Fill.PatternType = OfficeOpenXml.Style.FillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            //range.Style.HorizontalAlignment = OfficeOpenXml.Style.HorizontalAlignment.Center;
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
