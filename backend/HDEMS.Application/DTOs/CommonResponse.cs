namespace HDEMS.Application.DTOs;

/// <summary>
/// 通用响应结果
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// 状态码
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// 响应消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 响应数据
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success => Code == 200;

    /// <summary>
    /// 创建成功响应
    /// </summary>
    public static ApiResponse<T> Ok(T data, string message = "操作成功")
    {
        return new ApiResponse<T> { Code = 200, Message = message, Data = data };
    }

    /// <summary>
    /// 创建失败响应
    /// </summary>
    public static ApiResponse<T> Fail(int code, string message)
    {
        return new ApiResponse<T> { Code = code, Message = message };
    }

    /// <summary>
    /// 创建分页响应
    /// </summary>
    public static ApiResponse<PagedResult<T>> OkPaged(PagedResult<T> data, string message = "查询成功")
    {
        return new ApiResponse<PagedResult<T>> { Code = 200, Message = message, Data = data };
    }
}

/// <summary>
/// 无数据响应结果
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    /// <summary>
    /// 创建成功响应
    /// </summary>
    public static ApiResponse Ok(string message = "操作成功")
    {
        return new ApiResponse { Code = 200, Message = message };
    }

    /// <summary>
    /// 创建失败响应
    /// </summary>
    public static new ApiResponse Fail(int code, string message)
    {
        return new ApiResponse { Code = code, Message = message };
    }

    /// <summary>
    /// 创建分页响应
    /// </summary>
    public static ApiResponse<PagedResult<T>> OkPaged<T>(PagedResult<T> data, string message = "查询成功")
    {
        return new ApiResponse<PagedResult<T>> { Code = 200, Message = message, Data = data };
    }
}

/// <summary>
/// 分页结果
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// 总记录数
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// 当前页码
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// 每页条数
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 总页数
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)Total / PageSize);

    /// <summary>
    /// 数据列表
    /// </summary>
    public List<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// 是否有上一页
    /// </summary>
    public bool HasPrevious => Page > 1;

    /// <summary>
    /// 是否有下一页
    /// </summary>
    public bool HasNext => Page < TotalPages;
}
