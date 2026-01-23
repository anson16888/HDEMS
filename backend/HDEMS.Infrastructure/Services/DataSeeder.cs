using FreeSql;
using HDEMS.Domain.Entities;
using HDEMS.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HDEMS.Infrastructure.Services;

/// <summary>
/// 种子数据初始化服务
/// </summary>
public class DataSeeder
{
    private readonly IFreeSql _fsql;
    private readonly ILogger<DataSeeder> _logger;
    private readonly PasswordService _passwordService;

    public DataSeeder(IFreeSql fsql, ILogger<DataSeeder> logger, PasswordService passwordService)
    {
        _fsql = fsql;
        _logger = logger;
        _passwordService = passwordService;
    }

    /// <summary>
    /// 初始化种子数据
    /// </summary>
    public async Task SeedAsync()
    {
        _logger.LogInformation("开始初始化种子数据...");

        try
        {
            // 初始化医院配置数据
            await SeedHospitalConfigAsync();

            // 初始化物资类型数据
            await SeedMaterialTypesAsync();

            // 初始化物资阈值数据
            await SeedMaterialThresholdsAsync();

            // 初始化科室数据
            await SeedDepartmentsAsync();

            // 初始化班次数据
            await SeedShiftsAsync();

            // 初始化人员职级数据
            await SeedPersonRanksAsync();

            // 初始化人员职称数据
            await SeedPersonTitlesAsync();

            // 检查并创建系统初始化密钥
            await SeedSystemInitKeyAsync();

            // 初始化管理员账户
            await SeedAdminUserAsync();

            _logger.LogInformation("种子数据初始化完成");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "种子数据初始化失败");
            throw;
        }
    }

    private async Task SeedHospitalConfigAsync()
    {
        if (await _fsql.Select<HospitalConfig>().AnyAsync())
            return;

        var hospitalConfig = new HospitalConfig
        {
            Id = Guid.NewGuid(),
            HospitalName = "宝安区域急救应急物资及值班管理系统",
            HospitalPhone = "0755-12345678",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        await _fsql.Insert(hospitalConfig).ExecuteAffrowsAsync();
        _logger.LogInformation("初始化医院配置数据: 1 条");
    }

    private async Task SeedDepartmentsAsync()
    {
        if (await _fsql.Select<Department>().AnyAsync())
            return;

        var departments = new List<Department>
        {
            new() { DepartmentCode = "D001", DepartmentName = "急诊科", DepartmentType = "临床", SortOrder = 1 },
            new() { DepartmentCode = "D002", DepartmentName = "内科", DepartmentType = "临床", SortOrder = 2 },
            new() { DepartmentCode = "D003", DepartmentName = "外科", DepartmentType = "临床", SortOrder = 3 },
            new() { DepartmentCode = "D004", DepartmentName = "妇产科", DepartmentType = "临床", SortOrder = 4 },
            new() { DepartmentCode = "D005", DepartmentName = "儿科", DepartmentType = "临床", SortOrder = 5 },
            new() { DepartmentCode = "D006", DepartmentName = "骨科", DepartmentType = "临床", SortOrder = 6 },
            new() { DepartmentCode = "D007", DepartmentName = "眼科", DepartmentType = "临床", SortOrder = 7 },
            new() { DepartmentCode = "D008", DepartmentName = "耳鼻喉科", DepartmentType = "临床", SortOrder = 8 },
            new() { DepartmentCode = "D009", DepartmentName = "口腔科", DepartmentType = "临床", SortOrder = 9 },
            new() { DepartmentCode = "D010", DepartmentName = "皮肤科", DepartmentType = "临床", SortOrder = 10 },
            new() { DepartmentCode = "D011", DepartmentName = "麻醉科", DepartmentType = "临床", SortOrder = 11 },
            new() { DepartmentCode = "D012", DepartmentName = "影像科", DepartmentType = "医技", SortOrder = 12 },
            new() { DepartmentCode = "D013", DepartmentName = "检验科", DepartmentType = "医技", SortOrder = 13 },
            new() { DepartmentCode = "D014", DepartmentName = "药剂科", DepartmentType = "医技", SortOrder = 14 },
            new() { DepartmentCode = "D015", DepartmentName = "病理科", DepartmentType = "医技", SortOrder = 15 },
            new() { DepartmentCode = "D016", DepartmentName = "行政部", DepartmentType = "行政", SortOrder = 16 },
            new() { DepartmentCode = "D017", DepartmentName = "医务科", DepartmentType = "行政", SortOrder = 17 },
            new() { DepartmentCode = "D018", DepartmentName = "护理部", DepartmentType = "行政", SortOrder = 18 },
            new() { DepartmentCode = "D019", DepartmentName = "财务部", DepartmentType = "行政", SortOrder = 19 },
            new() { DepartmentCode = "D020", DepartmentName = "后勤部", DepartmentType = "行政", SortOrder = 20 }
        };

        await _fsql.Insert(departments).ExecuteAffrowsAsync();
        _logger.LogInformation("初始化科室数据: {Count} 条", departments.Count);
    }

    private async Task SeedShiftsAsync()
    {
        if (await _fsql.Select<Shift>().AnyAsync())
            return;

        var shifts = new List<Shift>
        {
            new() { ShiftCode = "S001", ShiftName = "早班", TimeRange = "08:00-12:00", SortOrder = 1 },
            new() { ShiftCode = "S002", ShiftName = "中班", TimeRange = "12:00-18:00", SortOrder = 2 },
            new() { ShiftCode = "S003", ShiftName = "晚班", TimeRange = "18:00-24:00", SortOrder = 3 },
            new() { ShiftCode = "S004", ShiftName = "夜班", TimeRange = "00:00-08:00", SortOrder = 4 },
            new() { ShiftCode = "S005", ShiftName = "全天", TimeRange = "00:00-24:00", SortOrder = 5 },
            new() { ShiftCode = "S006", ShiftName = "上午", TimeRange = "08:00-12:00", SortOrder = 6 },
            new() { ShiftCode = "S007", ShiftName = "下午", TimeRange = "12:00-18:00", SortOrder = 7 },
            new() { ShiftCode = "S008", ShiftName = "一线", TimeRange = "24小时值班", SortOrder = 8 },
            new() { ShiftCode = "S009", ShiftName = "二线", TimeRange = "24小时值班", SortOrder = 9 },
            new() { ShiftCode = "S010", ShiftName = "值班", TimeRange = "24小时值班", SortOrder = 10 }
        };

        await _fsql.Insert(shifts).ExecuteAffrowsAsync();
        _logger.LogInformation("初始化班次数据: {Count} 条", shifts.Count);
    }

    private async Task SeedPersonRanksAsync()
    {
        if (await _fsql.Select<PersonRank>().AnyAsync())
            return;

        var ranks = new List<PersonRank>
        {
            // 局级职级
            new() { RankCode = "R001", RankName = "局长", RankCategory = "局级", SortOrder = 1 },
            new() { RankCode = "R002", RankName = "副局长", RankCategory = "局级", SortOrder = 2 },
            new() { RankCode = "R003", RankName = "科长", RankCategory = "局级", SortOrder = 3 },
            new() { RankCode = "R004", RankName = "副科长", RankCategory = "局级", SortOrder = 4 },
            new() { RankCode = "R005", RankName = "科员", RankCategory = "局级", SortOrder = 5 },
            // 院级职级
            new() { RankCode = "R006", RankName = "院长", RankCategory = "院级", SortOrder = 6 },
            new() { RankCode = "R007", RankName = "副院长", RankCategory = "院级", SortOrder = 7 },
            new() { RankCode = "R008", RankName = "主任", RankCategory = "院级", SortOrder = 8 },
            new() { RankCode = "R009", RankName = "副主任", RankCategory = "院级", SortOrder = 9 },
            new() { RankCode = "R010", RankName = "主任医师", RankCategory = "院级", SortOrder = 10 },
            new() { RankCode = "R011", RankName = "副主任医师", RankCategory = "院级", SortOrder = 11 },
            new() { RankCode = "R012", RankName = "主治医师", RankCategory = "院级", SortOrder = 12 },
            new() { RankCode = "R013", RankName = "住院医师", RankCategory = "院级", SortOrder = 13 },
            new() { RankCode = "R014", RankName = "实习医师", RankCategory = "院级", SortOrder = 14 }
        };

        await _fsql.Insert(ranks).ExecuteAffrowsAsync();
        _logger.LogInformation("初始化人员职级数据: {Count} 条", ranks.Count);
    }

    private async Task SeedPersonTitlesAsync()
    {
        if (await _fsql.Select<PersonTitle>().AnyAsync())
            return;

        var titles = new List<PersonTitle>
        {
            new() { TitleCode = "T001", TitleName = "教授", TitleLevel = "高级", SortOrder = 1 },
            new() { TitleCode = "T002", TitleName = "副教授", TitleLevel = "副高级", SortOrder = 2 },
            new() { TitleCode = "T003", TitleName = "讲师", TitleLevel = "中级", SortOrder = 3 },
            new() { TitleCode = "T004", TitleName = "助教", TitleLevel = "初级", SortOrder = 4 },
            new() { TitleCode = "T005", TitleName = "主任医师", TitleLevel = "高级", SortOrder = 5 },
            new() { TitleCode = "T006", TitleName = "副主任医师", TitleLevel = "副高级", SortOrder = 6 },
            new() { TitleCode = "T007", TitleName = "主治医师", TitleLevel = "中级", SortOrder = 7 },
            new() { TitleCode = "T008", TitleName = "住院医师", TitleLevel = "初级", SortOrder = 8 },
            new() { TitleCode = "T009", TitleName = "主任护师", TitleLevel = "高级", SortOrder = 9 },
            new() { TitleCode = "T010", TitleName = "副主任护师", TitleLevel = "副高级", SortOrder = 10 },
            new() { TitleCode = "T011", TitleName = "主管护师", TitleLevel = "中级", SortOrder = 11 },
            new() { TitleCode = "T012", TitleName = "护师", TitleLevel = "初级", SortOrder = 12 }
        };

        await _fsql.Insert(titles).ExecuteAffrowsAsync();
        _logger.LogInformation("初始化人员职称数据: {Count} 条", titles.Count);
    }

    private async Task SeedMaterialTypesAsync()
    {
        if (await _fsql.Select<MaterialTypeDict>().AnyAsync())
            return;

        var materialTypes = new List<MaterialTypeDict>
        {
            new() { Id = Guid.NewGuid(), TypeCode = "FOOD", TypeName = "食品", Color = "#52c41a", SortOrder = 1, IsEnabled = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
            new() { Id = Guid.NewGuid(), TypeCode = "MEDICAL", TypeName = "医疗", Color = "#1890ff", SortOrder = 2, IsEnabled = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
            new() { Id = Guid.NewGuid(), TypeCode = "EQUIPMENT", TypeName = "设备", Color = "#fa8c16", SortOrder = 3, IsEnabled = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
            new() { Id = Guid.NewGuid(), TypeCode = "CLOTHING", TypeName = "衣物", Color = "#722ed1", SortOrder = 4, IsEnabled = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
            new() { Id = Guid.NewGuid(), TypeCode = "OTHER", TypeName = "其他", Color = "#8c8c8c", SortOrder = 5, IsEnabled = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
        };

        await _fsql.Insert(materialTypes).ExecuteAffrowsAsync();
        _logger.LogInformation("初始化物资类型数据: {Count} 条", materialTypes.Count);
    }

    private async Task SeedMaterialThresholdsAsync()
    {
        if (await _fsql.Select<MaterialThreshold>().AnyAsync())
            return;

        // 获取所有物资类型
        var materialTypes = await _fsql.Select<MaterialTypeDict>()
            .Where(t => t.IsEnabled)
            .OrderBy(t => t.SortOrder)
            .ToListAsync();

        if (materialTypes.Count == 0)
        {
            _logger.LogWarning("没有找到启用的物资类型，跳过阈值初始化");
            return;
        }

        // 为每个物资类型创建阈值配置（默认阈值为5）
        var thresholds = materialTypes.Select((mt, index) => new MaterialThreshold
        {
            Id = Guid.NewGuid(),
            MaterialTypeId = mt.Id,
            Threshold = 5,
            IsEnabled = true,
            SortOrder = index + 1,
            Remark = $"{mt.TypeName}库存低值预警阈值",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        }).ToList();

        await _fsql.Insert(thresholds).ExecuteAffrowsAsync();
        _logger.LogInformation("初始化物资阈值数据: {Count} 条", thresholds.Count);
    }

    private async Task SeedSystemInitKeyAsync()
    {
        // 生成系统初始化密钥并保存到配置
        var initKey = GenerateInitKey();
        _logger.LogInformation("系统初始化密钥已生成: {Key} (请妥善保管)", initKey);
        Console.WriteLine($"========================================");
        Console.WriteLine($"系统初始化密钥: {initKey}");
        Console.WriteLine($"========================================");
        Console.WriteLine($"请妥善保管此密钥，用于管理员密码重置验证");
    }

    private string GenerateInitKey()
    {
        // 生成32位随机密钥
        return Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N").Substring(0, 16);
    }

    private async Task SeedAdminUserAsync()
    {
        if (await _fsql.Select<User>().AnyAsync())
            return;

        // 创建管理员账户：admin / 123456
        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            Password = _passwordService.HashPassword("123456"),
            RealName = "系统管理员",
            Phone = "13800138000",
            Department = "信息科",
            Roles = JsonSerializer.Serialize(new List<string> { UserRole.SYSTEM_ADMIN.ToString() }),
            Status = UserStatus.Active,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        await _fsql.Insert(adminUser).ExecuteAffrowsAsync();
        _logger.LogInformation("初始化管理员账户: admin / 123456");
        Console.WriteLine("========================================");
        Console.WriteLine("管理员账户已创建");
        Console.WriteLine("用户名: admin");
        Console.WriteLine("密码: 123456");
        Console.WriteLine("========================================");
        Console.WriteLine("请登录后及时修改默认密码");
    }
}
