using FreeSql;
using HDEMS.Api.Filters;
using HDEMS.Application;
using HDEMS.Domain;
using HDEMS.Infrastructure;
using HDEMS.Infrastructure.Configuration;
using HDEMS.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// 配置 Serilog
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File("logs/hdems-.txt", rollingInterval: RollingInterval.Day);
});

// 添加服务到容器
builder.Services.AddControllers(options =>
{
    // 全局异常过滤器
    options.Filters.Add<GlobalExceptionFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.WriteIndented = true;
});

// 配置数据库
var dbConfig = builder.Configuration.GetSection("DatabaseConfig").Get<DatabaseConfig>() ?? new DatabaseConfig();
var dbType = dbConfig.DbType?.ToLower() switch
{
    "sqlserver" => DataType.SqlServer,
    _ => DataType.Sqlite
};

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
IFreeSql fsql = new FreeSqlBuilder()
    .UseConnectionString(dbType, connectionString!)
    .UseAutoSyncStructure(false)  // 禁用自动同步结构
    .UseNoneCommandParameter(true)  // 禁用参数化命令
    .UseMonitorCommand(cmd => Log.Debug("SQL: {Sql}", cmd.CommandText))
    .Build();

// 初始化数据库表结构（首次启动时）
await InitializeDatabaseAsync(fsql);

// 注册 FreeSql
builder.Services.AddSingleton(fsql);
builder.Services.AddFreeRepository();

// 配置 JWT 认证
var jwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>() ?? new JwtConfig();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig.Issuer,
            ValidAudience = jwtConfig.Audience,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(jwtConfig.SecretKey ?? "DefaultSecretKeyMustBe32CharsLong!"))
        };
    });

builder.Services.AddAuthorization();

// 配置 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 注册应用服务
builder.Services.AddApplicationServices();

// 注册基础设施服务
builder.Services.AddInfrastructureServices(builder.Configuration);

// 配置 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HDEMS API",
        Version = "v1",
        Description = "宝安区域急救应急物资及值班管理系统 API",
        Contact = new OpenApiContact
        {
            Name = "HDEMS Team"
        }
    });

    // 启用 JWT 注释
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // 包含 XML 注释
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// 初始化数据库（种子数据）
if (dbConfig.AutoSeedData == true)
{
    using (var scope = app.Services.CreateScope())
    {
        var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
        dataSeeder.SeedAsync().Wait();
    }
}

// 配置 HTTP 请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// 健康检查端点
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.Now }))
    .WithName("HealthCheck")
    .WithOpenApi();

app.MapControllers();

app.Run();

/// <summary>
/// 初始化数据库表结构
/// </summary>
static async Task InitializeDatabaseAsync(IFreeSql fsql)
{
    // 检查 Hospital 表是否存在
    var tableExists = await fsql.Ado.ExecuteScalarAsync(
        "SELECT name FROM sqlite_master WHERE type='table' AND name='t_hospital'");

    if (tableExists == null)
    {
        // 表不存在，创建所有表
        try
        {
            fsql.CodeFirst.SyncStructure(typeof(HDEMS.Domain.Entities.Hospital));
            fsql.CodeFirst.SyncStructure(typeof(HDEMS.Domain.Entities.Department));
            fsql.CodeFirst.SyncStructure(typeof(HDEMS.Domain.Entities.Shift));
            fsql.CodeFirst.SyncStructure(typeof(HDEMS.Domain.Entities.PersonRank));
            fsql.CodeFirst.SyncStructure(typeof(HDEMS.Domain.Entities.PersonTitle));
            fsql.CodeFirst.SyncStructure(typeof(HDEMS.Domain.Entities.Person));
            fsql.CodeFirst.SyncStructure(typeof(HDEMS.Domain.Entities.User));
            fsql.CodeFirst.SyncStructure(typeof(HDEMS.Domain.Entities.Material));
            fsql.CodeFirst.SyncStructure(typeof(HDEMS.Domain.Entities.MaterialThreshold));
            fsql.CodeFirst.SyncStructure(typeof(HDEMS.Domain.Entities.Schedule));

            Console.WriteLine("数据库表结构初始化完成");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"数据库初始化警告: {ex.Message}");
        }
    }
    else
    {
        Console.WriteLine("数据库表已存在，跳过初始化");
    }
}
