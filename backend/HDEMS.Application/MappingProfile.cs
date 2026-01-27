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
            .ForMember(d => d.MaterialTypeName, opt => opt.MapFrom(s => s.MaterialType != null ? s.MaterialType.TypeName : ""))
            .ForMember(d => d.MaterialTypeColor, opt => opt.MapFrom(s => s.MaterialType != null ? s.MaterialType.Color : null))
            .ForMember(d => d.StatusName, opt => opt.MapFrom(s => GetStatusName(s.Status)))
            .ForMember(d => d.HospitalName, opt => opt.MapFrom(s => s.Hospital != null ? s.Hospital.HospitalName : null));

        CreateMap<MaterialCreateRequest, Material>();

        // MaterialTypeDict 映射
        CreateMap<MaterialTypeDict, MaterialTypeDto>()
            .ReverseMap();

        CreateMap<MaterialTypeDict, MaterialTypeOptionDto>()
            .ReverseMap();

        CreateMap<MaterialTypeCreateRequest, MaterialTypeDict>();

        CreateMap<MaterialTypeUpdateRequest, MaterialTypeDict>();

        // MaterialThreshold 映射
        CreateMap<MaterialThreshold, MaterialThresholdDto>()
            .ForMember(d => d.MaterialTypeName, opt => opt.MapFrom(s => s.MaterialType != null ? s.MaterialType.TypeName : ""))
            .ForMember(d => d.MaterialTypeColor, opt => opt.MapFrom(s => s.MaterialType != null ? s.MaterialType.Color : null))
            .ReverseMap();

        CreateMap<MaterialThresholdCreateRequest, MaterialThreshold>()
            .ForMember(d => d.Id, opt => opt.Ignore());

        CreateMap<MaterialThresholdUpdateRequest, MaterialThreshold>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore());

        // Hospital 映射
        CreateMap<Hospital, HospitalDto>()
            .ReverseMap();

        // Schedule 映射
        CreateMap<Schedule, ScheduleDto>()
            .ForMember(d => d.ScheduleTypeName, opt => opt.MapFrom(s => s.ScheduleType.ToString()))
            .ForMember(d => d.ShiftName, opt => opt.MapFrom(s => s.Shift != null ? s.Shift.ShiftName : null))
            .ForMember(d => d.RankName, opt => opt.MapFrom(s => s.Rank != null ? s.Rank.RankName : null))
            .ForMember(d => d.DepartmentName, opt => opt.MapFrom(s => s.Department != null ? s.Department.DepartmentName : null))
            .ForMember(d => d.TitleName, opt => opt.MapFrom(s => s.Title != null ? s.Title.TitleName : null))
            .ForMember(d => d.HospitalName, opt => opt.MapFrom(s => s.Hospital != null ? s.Hospital.HospitalName : null));

        CreateMap<ScheduleCreateRequest, Schedule>();

        // User 映射
        CreateMap<User, UserDto>()
            .ForMember(d => d.Roles, opt => opt.Ignore())           // 手动填充
            .ForMember(d => d.RoleDescriptions, opt => opt.Ignore()) // 手动填充
            .ForMember(d => d.HospitalName, opt => opt.MapFrom(s => s.Hospital != null ? s.Hospital.HospitalName : null));

        CreateMap<UserCreateRequest, User>()
            .ForMember(d => d.Roles, opt => opt.Ignore());  // Roles 通过 SetRoleList 设置

        CreateMap<UserUpdateRequest, User>()
            .ForMember(d => d.Roles, opt => opt.Ignore())  // Roles 通过 SetRoleList 设置
            .ForMember(d => d.Username, opt => opt.Ignore());  // 用户名不允许修改

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

    private static string GetStatusName(int status)
    {
        return status switch
        {
            0 => "正常",
            1 => "库存偏低",
            2 => "已耗尽",
            3 => "已过期",
            4 => "即将过期",
            _ => "未知"
        };
    }
}
