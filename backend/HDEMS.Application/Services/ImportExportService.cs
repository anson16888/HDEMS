using HDEMS.Application.Interfaces;
using HDEMS.Domain.Enums;
using OfficeOpenXml;

namespace HDEMS.Application.Services;

/// <summary>
/// 导入导出服务实现
/// </summary>
public class ImportExportService : IImportExportService
{
    public async Task<byte[]> GetMaterialTemplateAsync()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("物资导入模板");

        // 表头
        worksheet.Cells[1, 1].Value = "物资编码*";
        worksheet.Cells[1, 2].Value = "物资名称*";
        worksheet.Cells[1, 3].Value = "物资类型*";
        worksheet.Cells[1, 4].Value = "库存数量*";
        worksheet.Cells[1, 5].Value = "单位";
        worksheet.Cells[1, 6].Value = "存放位置*";
        worksheet.Cells[1, 7].Value = "医院*";
        worksheet.Cells[1, 8].Value = "规格";
        worksheet.Cells[1, 9].Value = "生产日期";
        worksheet.Cells[1, 10].Value = "质保期(月)";
        worksheet.Cells[1, 11].Value = "备注";

        // 示例数据
        worksheet.Cells[2, 1].Value = "EM-2023001";
        worksheet.Cells[2, 2].Value = "急救包（标准型）";
        worksheet.Cells[2, 3].Value = "Medical";
        worksheet.Cells[2, 4].Value = "100";
        worksheet.Cells[2, 5].Value = "个";
        worksheet.Cells[2, 6].Value = "A区-1排-3号";
        worksheet.Cells[2, 7].Value = "宝安人民医院";
        worksheet.Cells[2, 8].Value = "30x20x10cm";
        worksheet.Cells[2, 9].Value = "2023-01-01";
        worksheet.Cells[2, 10].Value = "36";
        worksheet.Cells[2, 11].Value = "常规急救包";

        // 设置列宽
        for (int col = 1; col <= 11; col++)
        {
            worksheet.Column(col).Width = 15;
        }

        // 表头样式
        using (var range = worksheet.Cells[1, 1, 1, 11])
        {
            range.Style.Font.Bold = true;
            //range.Style.Fill.PatternType = OfficeOpenXml.Style.FillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
        }

        return await Task.FromResult(package.GetAsByteArray());
    }

    public async Task<byte[]> GetScheduleTemplateAsync(ScheduleType scheduleType)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add($"{scheduleType}排班导入模板");

        // 表头
        worksheet.Cells[1, 1].Value = "日期*";
        worksheet.Cells[1, 2].Value = "医院*";
        worksheet.Cells[1, 3].Value = "班次*";
        worksheet.Cells[1, 4].Value = "人员姓名*";
        worksheet.Cells[1, 5].Value = "联系电话*";
        worksheet.Cells[1, 6].Value = "职级";
        worksheet.Cells[1, 7].Value = "科室";
        worksheet.Cells[1, 8].Value = "职称";
        worksheet.Cells[1, 9].Value = "备注";

        // 示例数据
        worksheet.Cells[2, 1].Value = "2023-11-01";
        worksheet.Cells[2, 2].Value = "宝安人民医院";
        worksheet.Cells[2, 3].Value = "早班";
        worksheet.Cells[2, 4].Value = "张三";
        worksheet.Cells[2, 5].Value = "13800138000";
        worksheet.Cells[2, 6].Value = "主任医师";
        worksheet.Cells[2, 7].Value = "内科";
        worksheet.Cells[2, 8].Value = "教授";
        worksheet.Cells[2, 9].Value = "常规值班";

        // 设置列宽
        for (int col = 1; col <= 9; col++)
        {
            worksheet.Column(col).Width = 15;
        }

        // 表头样式
        using (var range = worksheet.Cells[1, 1, 1, 9])
        {
            range.Style.Font.Bold = true;
           // range.Style.Fill.PatternType = OfficeOpenXml.Style.FillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
        }

        return await Task.FromResult(package.GetAsByteArray());
    }
}
