using HDEMS.Domain.Enums;

namespace HDEMS.Application.DTOs;

/// <summary>
/// 排班DTO
/// </summary>
public class ScheduleDto
{
    public Guid Id { get; set; }
    public DateTime ScheduleDate { get; set; }
    public ScheduleType ScheduleType { get; set; }
    public string ScheduleTypeName { get; set; } = string.Empty;
    public Guid ShiftId { get; set; }
    public string? ShiftName { get; set; }
    public Guid? PersonId { get; set; }
    public string PersonName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public Guid? RankId { get; set; }
    public string? RankName { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public Guid? TitleId { get; set; }
    public string? TitleName { get; set; }
    public string? Remark { get; set; }
    public Guid? HospitalId { get; set; }
    public string? HospitalName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 排班创建/更新请求
/// </summary>
public class ScheduleCreateRequest
{
    public DateTime ScheduleDate { get; set; }
    public ScheduleType ScheduleType { get; set; }
    public Guid ShiftId { get; set; }
    public string PersonName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public Guid? RankId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? TitleId { get; set; }
    public string? Remark { get; set; }
}

/// <summary>
/// 排班查询请求
/// </summary>
public class ScheduleQueryRequest
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ScheduleType? ScheduleType { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? ShiftId { get; set; }
    public Guid? HospitalId { get; set; }
    public string? Keyword { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// 排班统计
/// </summary>
public class ScheduleStatistics
{
    public int TotalCount { get; set; }
    public int BureauCount { get; set; }
    public int HospitalCount { get; set; }
    public int DirectorCount { get; set; }
}

/// <summary>
/// 排班一览表汇总项
/// </summary>
public class ScheduleOverviewItem
{
    public DateTime ScheduleDate { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public ScheduleType ScheduleType { get; set; }
    public string ScheduleTypeName { get; set; } = string.Empty;
    public string ShiftName { get; set; } = string.Empty;
    public string PersonName { get; set; } = string.Empty;
    public string? RankName { get; set; }
    public string? TitleName { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Remark { get; set; }
}
