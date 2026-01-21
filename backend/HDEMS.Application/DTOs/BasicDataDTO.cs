namespace HDEMS.Application.DTOs;

/// <summary>
/// 医院DTO
/// </summary>
public class HospitalDto
{
    public Guid Id { get; set; }
    public string HospitalCode { get; set; } = string.Empty;
    public string HospitalName { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public string? Address { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactPerson { get; set; }
    public int SortOrder { get; set; }
    public int Status { get; set; }
}

/// <summary>
/// 科室DTO
/// </summary>
public class DepartmentDto
{
    public Guid Id { get; set; }
    public string DepartmentCode { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string? DepartmentType { get; set; }
    public int SortOrder { get; set; }
}

/// <summary>
/// 班次DTO
/// </summary>
public class ShiftDto
{
    public Guid Id { get; set; }
    public string ShiftCode { get; set; } = string.Empty;
    public string ShiftName { get; set; } = string.Empty;
    public string? TimeRange { get; set; }
    public int SortOrder { get; set; }
}

/// <summary>
/// 人员职级DTO
/// </summary>
public class PersonRankDto
{
    public Guid Id { get; set; }
    public string RankCode { get; set; } = string.Empty;
    public string RankName { get; set; } = string.Empty;
    public string? RankCategory { get; set; }
    public int SortOrder { get; set; }
}

/// <summary>
/// 人员职称DTO
/// </summary>
public class PersonTitleDto
{
    public Guid Id { get; set; }
    public string TitleCode { get; set; } = string.Empty;
    public string TitleName { get; set; } = string.Empty;
    public string? TitleLevel { get; set; }
    public int SortOrder { get; set; }
}

/// <summary>
/// 人员DTO
/// </summary>
public class PersonDto
{
    public Guid Id { get; set; }
    public string PersonCode { get; set; } = string.Empty;
    public string PersonName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public Guid? RankId { get; set; }
    public string? RankName { get; set; }
    public Guid? TitleId { get; set; }
    public string? TitleName { get; set; }
    public Guid? HospitalId { get; set; }
    public string? HospitalName { get; set; }
}
