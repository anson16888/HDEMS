using OfficeOpenXml;
using OfficeOpenXml.Style;
using ExcelFillStyle = OfficeOpenXml.Style.ExcelFillStyle;
using ExcelHorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment;
using ExcelVerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment;

namespace HDEMS.Application.Helpers;

/// <summary>
/// Excel 辅助类
/// </summary>
public static class ExcelHelper
{
    /// <summary>
    /// 设置单元格样式
    /// </summary>
    public static void SetCellStyle(ExcelRange range, bool isBold = false, System.Drawing.Color? backgroundColor = null)
    {
        range.Style.Font.Bold = isBold;

        if (backgroundColor.HasValue)
        {
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(backgroundColor.Value);
        }
    }

    /// <summary>
    /// 设置表头样式
    /// </summary>
    public static void SetHeaderStyle(ExcelRange range)
    {
        SetCellStyle(range, true, System.Drawing.Color.LightBlue);
        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
    }

    /// <summary>
    /// 自动调整列宽
    /// </summary>
    public static void AutoFitColumns(ExcelWorksheet worksheet, int minWidth = 10, int maxWidth = 50)
    {
        worksheet.Cells.AutoFitColumns();

        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
        {
            if (worksheet.Column(col).Width < minWidth)
                worksheet.Column(col).Width = minWidth;
            else if (worksheet.Column(col).Width > maxWidth)
                worksheet.Column(col).Width = maxWidth;
        }
    }
}
