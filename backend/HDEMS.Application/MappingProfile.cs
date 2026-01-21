using AutoMapper;
using HDEMS.Application.DTOs;
using HDEMS.Domain.Entities;

namespace HDEMS.Application;

/// <summary>
/// AutoMapper 配置文件
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Material 映射
        CreateMap<Material, MaterialDto>()
            .ForMember(d => d.MaterialTypeName, opt => opt.MapFrom(s => s.MaterialType.ToString()))
            .ForMember(d => d.HospitalName, opt => opt.MapFrom(s => s.Hospital != null ? s.Hospital.HospitalName : null))
            .ForMember(d => d.StatusName, opt => opt.MapFrom(s => GetStatusName(s.Status)));

        CreateMap<MaterialCreateRequest, Material>();

        // Schedule 映射
        CreateMap<Schedule, ScheduleDto>()
            .ForMember(d => d.HospitalName, opt => opt.MapFrom(s => s.Hospital != null ? s.Hospital.HospitalName : null))
            .ForMember(d => d.ScheduleTypeName, opt => opt.MapFrom(s => s.ScheduleType.ToString()))
            .ForMember(d => d.ShiftName, opt => opt.MapFrom(s => s.Shift != null ? s.Shift.ShiftName : null))
            .ForMember(d => d.RankName, opt => opt.MapFrom(s => s.Rank != null ? s.Rank.RankName : null))
            .ForMember(d => d.DepartmentName, opt => opt.MapFrom(s => s.Department != null ? s.Department.DepartmentName : null))
            .ForMember(d => d.TitleName, opt => opt.MapFrom(s => s.Title != null ? s.Title.TitleName : null));

        CreateMap<ScheduleCreateRequest, Schedule>();

        // User 映射
        CreateMap<User, UserDto>()
            .ForMember(d => d.HospitalName, opt => opt.MapFrom(s => s.Hospital != null ? s.Hospital.HospitalName : null))
            .ForMember(d => d.Roles, opt => opt.MapFrom(s => s.GetRoleList()));

        CreateMap<UserCreateRequest, User>();
        CreateMap<UserUpdateRequest, User>();

        // Hospital 映射
        CreateMap<Hospital, HospitalDto>()
            .ReverseMap();

        // Department 映射
        CreateMap<Department, DepartmentDto>()
            .ReverseMap();

        // Shift 映射
        CreateMap<Shift, ShiftDto>()
            .ReverseMap();

        // PersonRank 映射
        CreateMap<PersonRank, PersonRankDto>()
            .ReverseMap();

        // PersonTitle 映射
        CreateMap<PersonTitle, PersonTitleDto>()
            .ReverseMap();

        // Person 映射
        CreateMap<Person, PersonDto>()
            .ForMember(d => d.DepartmentName, opt => opt.MapFrom(s => s.Department != null ? s.Department.DepartmentName : null))
            .ForMember(d => d.RankName, opt => opt.MapFrom(s => s.Rank != null ? s.Rank.RankName : null))
            .ForMember(d => d.TitleName, opt => opt.MapFrom(s => s.Title != null ? s.Title.TitleName : null))
            .ForMember(d => d.HospitalName, opt => opt.MapFrom(s => s.Hospital != null ? s.Hospital.HospitalName : null));
    }

    private static string GetStatusName(Domain.Enums.MaterialStatus status)
    {
        return status switch
        {
            Domain.Enums.MaterialStatus.Normal => "正常",
            Domain.Enums.MaterialStatus.Low => "库存偏低",
            Domain.Enums.MaterialStatus.Out => "已耗尽",
            Domain.Enums.MaterialStatus.Expired => "已过期",
            Domain.Enums.MaterialStatus.ExpiringSoon => "即将过期",
            _ => "未知"
        };
    }
}
