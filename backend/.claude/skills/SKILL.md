# HDEMS 项目开发指南

## 项目概述

**HDEMS** (宝安区域急救应急物资及值班管理系统) 是一个基于 .NET 9 的企业级 Web API 应用，采用清洁架构 (Clean Architecture) 设计模式。

### 主要功能模块

- **用户管理**: 用户认证、权限管理、用户 CRUD
- **物资管理**: 物资信息管理、物资类型、物资阈值预警
- **排班管理**: 医院排班计划、排班统计
- **基础数据**: 医院、科室、人员、班次、职级、职称管理
- **导入导出**: Excel 数据导入导出功能

---

## 技术栈

| 技术 | 版本 | 用途 |
|------|------|------|
| .NET | 9.0 | 核心框架 |
| ASP.NET Core Web API | 9.0 | Web API 框架 |
| FreeSql | 3.2.833 | ORM 框架 |
| SQLite | - | 默认数据库 (可切换 SQL Server) |
| JWT Bearer | - | 身份认证 |
| BCrypt.Net-Next | - | 密码加密 |
| Swagger | 6.5.0 | API 文档 |
| EPPlus | 7.0.5 | Excel 处理 |
| Serilog | - | 结构化日志 |
| AutoMapper | - | 对象映射 |

---

## 项目架构

```
HDEMS.Backend/
├── HDEMS.Api/              # 表现层 - Web API 接口
├── HDEMS.Application/      # 应用层 - 业务逻辑、DTO、服务接口
├── HDEMS.Domain/           # 领域层 - 实体、枚举、领域模型
└── HDEMS.Infrastructure/   # 基础设施层 - 数据库、配置、外部服务
```

### 各层职责

| 层级 | 职责 | 关键组件 |
|------|------|----------|
| **HDEMS.Api** | HTTP 请求处理、API 路由、认证过滤 | Controllers, Filters, Program.cs |
| **HDEMS.Application** | 业务逻辑编排、DTO 转换、服务实现 | Services, DTOs, Interfaces, MappingProfile |
| **HDEMS.Domain** | 核心业务模型、实体定义 | Entities, Enums |
| **HDEMS.Infrastructure** | 数据访问、外部服务、配置 | FreeSql 配置, JWT Service, Password Service |

---

## 代码规范

### 命名约定

```csharp
// 命名空间 - PascalCase
namespace HDEMS.Api.Controllers
namespace HDEMS.Application.Services

// 类名 - PascalCase
public class UserController : ControllerBase
public class MaterialService : IMaterialService

// 方法名 - PascalCase
public async Task<ApiResponse<List<UserDTO>>> GetPagedAsync(...)
public async Task<ApiResponse<bool>> CreateMaterial(...)

// 属性名 - PascalCase
public Guid Id { get; set; }
public string MaterialName { get; set; }

// 局部变量/参数 - camelCase
var userId = user.Id;
async Task<List<Material>> GetActiveMaterials(Guid categoryId)
```

### 异步编程规范

```csharp
// 所有 I/O 操作使用 async/await
public async Task<ApiResponse<List<UserDTO>>> GetAllUsersAsync()
{
    var users = await _userRepository.Select().ToListAsync();
    return ApiResponse<List<UserDTO>>.Success(_mapper.Map<List<UserDTO>>(users));
}
```

### 服务注入规范

```csharp
// 构造函数注入依赖
private readonly IMaterialService _materialService;
private readonly IMapper _mapper;

public MaterialController(IMaterialService materialService, IMapper mapper)
{
    _materialService = materialService;
    _mapper = mapper;
}
```

### 统一响应格式

```csharp
// 成功响应
return ApiResponse<T>.Success(data);

// 失败响应
return ApiResponse<T>.Fail("错误信息");

// 分页响应
return ApiResponse<PagedResult<T>>.Success(pagedResult);
```

---

## 添加新功能的标准流程

### 1. 定义领域实体 (Domain/Entities/)

```csharp
/// <summary>
/// 功能描述
/// </summary>
public class YourEntity : BaseEntity
{
    /// <summary>
    /// 属性描述
    /// </summary>
    public string PropertyName { get; set; }
}
```

### 2. 创建 DTO (Application/DTOs/)

```csharp
public class YourEntityDTO
{
    public Guid Id { get; set; }
    public string PropertyName { get; set; }
}

public class CreateYourEntityDTO
{
    [Required]
    public string PropertyName { get; set; }
}

public class UpdateYourEntityDTO
{
    public Guid Id { get; set; }
    public string PropertyName { get; set; }
}
```

### 3. 定义服务接口 (Application/Interfaces/)

```csharp
public interface IYourEntityService
{
    Task<ApiResponse<List<YourEntityDTO>>> GetAllAsync();
    Task<ApiResponse<YourEntityDTO>> GetByIdAsync(Guid id);
    Task<ApiResponse<YourEntityDTO>> CreateAsync(CreateYourEntityDTO dto);
    Task<ApiResponse<YourEntityDTO>> UpdateAsync(UpdateYourEntityDTO dto);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);
}
```

### 4. 实现服务 (Application/Services/)

```csharp
public class YourEntityService : IYourEntityService
{
    private readonly IBaseRepository<YourEntity> _repository;
    private readonly IMapper _mapper;

    public YourEntityService(IBaseRepository<YourEntity> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<YourEntityDTO>>> GetAllAsync()
    {
        var entities = await _repository.Select().ToListAsync();
        return ApiResponse<List<YourEntityDTO>>.Success(_mapper.Map<List<YourEntityDTO>>(entities));
    }

    // ... 其他方法实现
}
```

### 5. 创建控制器 (Api/Controllers/)

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class YourEntityController : ControllerBase
{
    private readonly IYourEntityService _service;

    public YourEntityController(IYourEntityService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (!result.Success)
            return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateYourEntityDTO dto)
    {
        var result = await _service.CreateAsync(dto);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateYourEntityDTO dto)
    {
        var result = await _service.UpdateAsync(dto);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }
}
```

### 6. 配置 AutoMapper (Application/MappingProfile.cs)

```csharp
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // 添加映射配置
        CreateMap<YourEntity, YourEntityDTO>();
        CreateMap<CreateYourEntityDTO, YourEntity>();
        CreateMap<UpdateYourEntityDTO, YourEntity>();
    }
}
```

### 7. 注册服务 (Application/ServiceCollectionExtensions.cs)

```csharp
public static IServiceCollection AddApplicationServices(this IServiceCollection services)
{
    // 添加服务注册
    services.AddScoped<IYourEntityService, YourEntityService>();

    return services;
}
```

---

## 常用命令

### 构建和运行

```bash
# 还原依赖
dotnet restore

# 构建项目
dotnet build

# 运行项目
dotnet run

# 发布生产版本
dotnet publish -c Release
```

### 数据库迁移

项目使用 FreeSql 自动同步表结构，启动时自动创建/更新数据库。

### Swagger API 文档

运行项目后访问: `http://localhost:5000/swagger`

---

## 默认账户

```
用户名: admin
密码: admin123
角色: Admin
```

---

## 分页查询规范

```csharp
// 请求参数
public class PagedQueryDTO
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Keyword { get; set; }
}

// 响应结果
public class PagedResult<T>
{
    public List<T> Items { get; set; }
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)Total / PageSize);
}

// 服务实现
public async Task<ApiResponse<PagedResult<YourEntityDTO>>> GetPagedAsync(PagedQueryDTO dto)
{
    var query = _repository.Select();

    if (!string.IsNullOrWhiteSpace(dto.Keyword))
    {
        query = query.Where(x => x.PropertyName.Contains(dto.Keyword));
    }

    var total = await query.CountAsync();
    var items = await query
        .Page((dto.Page - 1) * dto.PageSize, dto.PageSize)
        .ToListAsync();

    var result = new PagedResult<YourEntityDTO>
    {
        Items = _mapper.Map<List<YourEntityDTO>>(items),
        Total = total,
        Page = dto.Page,
        PageSize = dto.PageSize
    };

    return ApiResponse<PagedResult<YourEntityDTO>>.Success(result);
}
```

---

## 异常处理

项目使用全局异常过滤器 `GlobalExceptionFilter`，自动捕获并格式化异常响应。

```csharp
// 返回格式
{
    "success": false,
    "message": "错误描述",
    "data": null
}
```

---

## 日志记录

使用 Serilog 进行结构化日志记录：

```csharp
// 注入 ILogger
private readonly ILogger<YourService> _logger;

// 记录日志
_logger.LogInformation("操作描述: {Detail}", detail);
_logger.LogWarning("警告信息");
_logger.LogError(ex, "错误详情");
```

---

## 配置说明

### 数据库配置 (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=hdems.db"
  }
}
```

### JWT 配置

```json
{
  "Jwt": {
    "SecretKey": "your-secret-key",
    "Issuer": "HDEMS",
    "Audience": "HDEMS.Client",
    "ExpirationMinutes": 120
  }
}
```

---

## Git 提交规范

```
feat: 添加新功能
fix: 修复 Bug
docs: 文档更新
style: 代码格式调整
refactor: 代码重构
perf: 性能优化
test: 测试相关
chore: 构建/工具链相关
```

---

## 项目文件结构速查

```
HDEMS.Api/
├── Controllers/           # API 控制器
├── Filters/               # 过滤器 (认证、异常)
└── Program.cs             # 应用入口

HDEMS.Application/
├── DTOs/                  # 数据传输对象
├── Interfaces/            # 服务接口
├── Services/              # 服务实现
├── Extensions/            # 扩展方法
├── Helpers/               # 辅助类
├── MappingProfile.cs      # AutoMapper 配置
└── ServiceCollectionExtensions.cs  # 服务注册

HDEMS.Domain/
├── Entities/              # 实体类
└── Enums/                 # 枚举类型

HDEMS.Infrastructure/
├── Config/                # 配置类
└── Services/              # 基础设施服务 (JWT、密码等)
```

---

*最后更新时间: 2026-01-23*
