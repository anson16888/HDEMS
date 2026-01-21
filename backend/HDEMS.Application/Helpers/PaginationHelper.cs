namespace HDEMS.Application.Helpers;

/// <summary>
/// 分页辅助类
/// </summary>
public static class PaginationHelper
{
    /// <summary>
    /// 计算总页数
    /// </summary>
    public static int CalculateTotalPages(int total, int pageSize)
    {
        return (int)Math.Ceiling((double)total / pageSize);
    }

    /// <summary>
    /// 验证分页参数
    /// </summary>
    public static (int Page, int PageSize) ValidatePagination(int page, int pageSize, int maxPageSize = 100)
    {
        if (page < 1)
            page = 1;

        if (pageSize < 1)
            pageSize = 20;

        if (pageSize > maxPageSize)
            pageSize = maxPageSize;

        return (page, pageSize);
    }

    /// <summary>
    /// 计算跳过的记录数
    /// </summary>
    public static int CalculateSkip(int page, int pageSize)
    {
        return (page - 1) * pageSize;
    }
}
