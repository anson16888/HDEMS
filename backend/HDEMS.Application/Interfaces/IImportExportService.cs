namespace HDEMS.Application.Interfaces;

/// <summary>
/// 导入导出服务接口
/// </summary>
public interface IImportExportService
{
    /// <summary>
    /// 下载物资导入模板
    /// </summary>
    Task<byte[]> GetMaterialTemplateAsync();

    /// <summary>
    /// 下载排班导入模板
    /// </summary>
    Task<byte[]> GetScheduleTemplateAsync(Domain.Enums.ScheduleType scheduleType);
}
