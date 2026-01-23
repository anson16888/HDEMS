using FreeSql;
using HDEMS.Application.Interfaces;
using HDEMS.Domain.Entities;
using HDEMS.Domain.Enums;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation;

namespace HDEMS.Application.Services;

/// <summary>
/// 导入导出服务实现
/// </summary>
public class ImportExportService : IImportExportService
{
    private readonly IFreeSql _fsql;

    public ImportExportService(IFreeSql fsql)
    {
        _fsql = fsql;
    }

    public async Task<byte[]> GetMaterialTemplateAsync()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("物资导入模板");

        // 从数据库获取物资类型列表
        var materialTypes = await _fsql.Select<MaterialTypeDict>()
            .Where(t => t.IsEnabled)
            .OrderBy(t => t.SortOrder)
            .ToListAsync();

        var materialTypeNames = materialTypes.Select(t => t.TypeName).ToList();

        // 第1行：标题和说明合并成一行（显示为两行，通过设置行高实现）
        worksheet.Cells[1, 1, 1, 10].Merge = true;
        worksheet.Cells[1, 1].Style.WrapText = true;
        worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        worksheet.Cells[1, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        worksheet.Row(1).Height = 36; // 设置行高以容纳两行文字

        // 使用 RichText 设置不同样式
        var titleText = worksheet.Cells[1, 1].RichText.Add("物资导入模板\n");
        titleText.Bold = true;
        titleText.Size = 12;

        var descText = worksheet.Cells[1, 1].RichText.Add("说明：物资编码不填则自动生成（格式为EM-YYMMDDHHMMSS-XXXXX）；带*号为必填项；物资类型可输入任意值，不存在时会自动创建。");
        descText.Color = System.Drawing.Color.Red;
        descText.Size = 10;

        // 第2行：表头
        worksheet.Cells[2, 1].Value = "物资编码";
        worksheet.Cells[2, 2].Value = "物资名称*";
        worksheet.Cells[2, 3].Value = "物资类型*";
        worksheet.Cells[2, 4].Value = "库存数量*";
        worksheet.Cells[2, 5].Value = "单位";
        worksheet.Cells[2, 6].Value = "存放位置*";
        worksheet.Cells[2, 7].Value = "规格";
        worksheet.Cells[2, 8].Value = "生产日期";
        worksheet.Cells[2, 9].Value = "质保期(月)";
        worksheet.Cells[2, 10].Value = "备注";

        // 第3行：示例数据
        worksheet.Cells[3, 1].Value = "";
        worksheet.Cells[3, 2].Value = "急救包（标准型）";
        worksheet.Cells[3, 3].Value = materialTypeNames.FirstOrDefault() ?? "医疗";
        worksheet.Cells[3, 4].Value = "100";
        worksheet.Cells[3, 5].Value = "个";
        worksheet.Cells[3, 6].Value = "A区-1排-3号";
        worksheet.Cells[3, 7].Value = "30x20x10cm";
        worksheet.Cells[3, 8].Value = DateTime.Now.ToString("yyyy-MM-dd");
        worksheet.Cells[3, 9].Value = "24";
        worksheet.Cells[3, 10].Value = "常规急救包";

        // 设置列宽
        worksheet.Column(1).Width = 20;  // 物资编码
        worksheet.Column(2).Width = 25;  // 物资名称
        worksheet.Column(3).Width = 12;  // 物资类型
        worksheet.Column(4).Width = 12;  // 库存数量
        worksheet.Column(5).Width = 10;  // 单位
        worksheet.Column(6).Width = 15;  // 存放位置
        worksheet.Column(7).Width = 15;  // 规格
        worksheet.Column(8).Width = 15;  // 生产日期
        worksheet.Column(9).Width = 12;  // 质保期
        worksheet.Column(10).Width = 30; // 备注

        // 标题行边框
        using (var range = worksheet.Cells[1, 1, 1, 10])
        {
            range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        }

        // 表头样式（加粗，带边框）
        using (var range = worksheet.Cells[2, 1, 2, 10])
        {
            range.Style.Font.Bold = true;
            range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        }

        // 添加物资类型下拉列表验证（从第3行开始，最多允许1000行数据）
        var materialTypeValidation = worksheet.DataValidations.AddListValidation("C3:C1002");
        materialTypeValidation.Formula.ExcelFormula = $"\"{string.Join(",", materialTypeNames)}\"";
        materialTypeValidation.ShowErrorMessage = true;
        materialTypeValidation.ErrorTitle = "物资类型错误";
        materialTypeValidation.Error = "请从下拉列表中选择有效的物资类型";

        return await Task.FromResult(package.GetAsByteArray());
    }

    public async Task<byte[]> GetScheduleTemplateAsync(ScheduleType scheduleType)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();

        // 获取排班类型中文名称
        var scheduleTypeName = scheduleType switch
        {
            ScheduleType.Bureau => "局级行政",
            ScheduleType.Hospital => "院级行政",
            ScheduleType.Director => "院内主任",
            _ => scheduleType.ToString()
        };

        var worksheet = package.Workbook.Worksheets.Add($"{scheduleTypeName}排班导入模板");

        // 获取字典数据（去掉医院）
        var shifts = await _fsql.Select<Shift>().OrderBy(s => s.SortOrder).ToListAsync();
        var shiftNames = shifts.Select(s => s.ShiftName).ToList();

        var departments = await _fsql.Select<Department>().OrderBy(d => d.SortOrder).ToListAsync();
        var departmentNames = departments.Select(d => d.DepartmentName).ToList();

        var ranks = await _fsql.Select<PersonRank>().OrderBy(r => r.SortOrder).ToListAsync();
        var rankNames = ranks.Select(r => r.RankName).ToList();

        var titles = await _fsql.Select<PersonTitle>().OrderBy(t => t.SortOrder).ToListAsync();
        var titleNames = titles.Select(t => t.TitleName).ToList();

        // 第1行：标题和说明合并成一行（显示为两行，通过设置行高实现）
        worksheet.Cells[1, 1, 1, 8].Merge = true;
        worksheet.Cells[1, 1].Style.WrapText = true;
        worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        worksheet.Cells[1, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        worksheet.Row(1).Height = 36; // 设置行高以容纳两行文字

        // 使用 RichText 设置不同样式
        var richText = worksheet.Cells[1, 1].RichText.Add($"{scheduleTypeName}排班导入模板\n");
        richText.Bold = true;
        richText.Size = 12;

        var descText = worksheet.Cells[1, 1].RichText.Add("说明：带*号为必填项；班次、职级、科室、职称请从下拉列表中选择。");
        descText.Color = System.Drawing.Color.Red;
        descText.Size = 10;

        // 第2行：表头
        worksheet.Cells[2, 1].Value = "日期*";
        worksheet.Cells[2, 2].Value = "班次*";
        worksheet.Cells[2, 3].Value = "人员姓名*";
        worksheet.Cells[2, 4].Value = "联系电话*";
        worksheet.Cells[2, 5].Value = "职级*";
        worksheet.Cells[2, 6].Value = "科室*";
        worksheet.Cells[2, 7].Value = "职称*";
        worksheet.Cells[2, 8].Value = "备注";

        // 第3行：示例数据
        worksheet.Cells[3, 1].Value = DateTime.Now.ToString("yyyy-MM-dd");
        worksheet.Cells[3, 2].Value = shiftNames.FirstOrDefault() ?? "早班";
        worksheet.Cells[3, 3].Value = "张三";
        worksheet.Cells[3, 4].Value = "13800138000";
        worksheet.Cells[3, 5].Value = rankNames.FirstOrDefault();
        worksheet.Cells[3, 6].Value = departmentNames.FirstOrDefault();
        worksheet.Cells[3, 7].Value = titleNames.FirstOrDefault();
        worksheet.Cells[3, 8].Value = "常规值班";

        // 设置列宽
        worksheet.Column(1).Width = 15;  // 日期
        worksheet.Column(2).Width = 12;  // 班次
        worksheet.Column(3).Width = 15;  // 人员姓名
        worksheet.Column(4).Width = 18;  // 联系电话
        worksheet.Column(5).Width = 15;  // 职级
        worksheet.Column(6).Width = 15;  // 科室
        worksheet.Column(7).Width = 15;  // 职称
        worksheet.Column(8).Width = 40;  // 备注

        // 标题行边框
        using (var range = worksheet.Cells[1, 1, 1, 8])
        {
            range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        }

        // 表头样式（加粗，带边框）
        using (var range = worksheet.Cells[2, 1, 2, 8])
        {
            range.Style.Font.Bold = true;
            range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        }

        // 添加班次下拉列表验证（B列-第2列，从第3行开始）
        var shiftValidation = worksheet.DataValidations.AddListValidation("B3:B1002");
        shiftValidation.Formula.ExcelFormula = $"\"{string.Join(",", shiftNames)}\"";
        shiftValidation.ShowErrorMessage = true;
        shiftValidation.ErrorTitle = "输入错误";
        shiftValidation.Error = "请从下拉列表中选择有效的班次";

        // 添加职级下拉列表验证（E列-第5列，从第3行开始）
        var rankValidation = worksheet.DataValidations.AddListValidation("E3:E1002");
        if (rankNames.Any())
        {
            rankValidation.Formula.ExcelFormula = $"\"{string.Join(",", rankNames)}\"";
        }
        rankValidation.ShowErrorMessage = true;
        rankValidation.ErrorTitle = "输入错误";
        rankValidation.Error = "请从下拉列表中选择有效的职级";

        // 添加科室下拉列表验证（F列-第6列，从第3行开始）
        var departmentValidation = worksheet.DataValidations.AddListValidation("F3:F1002");
        if (departmentNames.Any())
        {
            departmentValidation.Formula.ExcelFormula = $"\"{string.Join(",", departmentNames)}\"";
        }
        departmentValidation.ShowErrorMessage = true;
        departmentValidation.ErrorTitle = "输入错误";
        departmentValidation.Error = "请从下拉列表中选择有效的科室";

        // 添加职称下拉列表验证（G列-第7列，从第3行开始）
        var titleValidation = worksheet.DataValidations.AddListValidation("G3:G1002");
        if (titleNames.Any())
        {
            titleValidation.Formula.ExcelFormula = $"\"{string.Join(",", titleNames)}\"";
        }
        titleValidation.ShowErrorMessage = true;
        titleValidation.ErrorTitle = "输入错误";
        titleValidation.Error = "请从下拉列表中选择有效的职称";

        return await Task.FromResult(package.GetAsByteArray());
    }
}
