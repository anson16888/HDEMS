# HDEMS Backend API

宝安区域急救应急物资及值班管理系统 - 后端API服务

## 技术栈

- **框架**: .NET 9
- **ORM**: FreeSql 3.2.833
- **数据库**: SQLite (默认) / SQL Server
- **认证**: JWT Bearer Token
- **API文档**: Swagger/OpenAPI
- **日志**: Serilog
- **Excel处理**: EPPlus

## 项目结构

```
backend/
├── HDEMS.Api/            # Web API 项目
├── HDEMS.Domain/         # 领域层（实体、枚举）
├── HDEMS.Application/    # 应用层（DTO、服务接口与实现）
└── HDEMS.Infrastructure/ # 基础设施层（数据库配置、基础服务）
```

## 快速开始

### 前置要求

- .NET 9 SDK
- Visual Studio 2022 或 VS Code
- SQLite（默认）/ SQL Server

### 数据库配置

编辑 `HDEMS.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    // SQLite（默认）
    "DefaultConnection": "Data Source=hdems.db"

    // SQL Server（取消注释使用）
    // "DefaultConnection": "Data Source=localhost;Initial Catalog=HDEMS;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
  },
  "DatabaseConfig": {
    "DbType": "Sqlite",  // 或 "SqlServer"
    "AutoSyncStructure": true,
    "AutoSeedData": true
  }
}
```

### 运行项目

```bash
# 还原依赖
cd backend
dotnet restore

# 运行 API
cd HDEMS.Api
dotnet run
```

API 启动后访问:
- Swagger UI: http://localhost:5000/swagger
- API 地址: http://localhost:5000/api

### 默认账户

首次运行会自动创建默认管理员账户:
- 用户名: `admin`
- 密码: `123456`

## API 文档

### 认证接口

| 接口 | 方法 | 描述 |
|------|------|------|
| `/api/auth/login` | POST | 用户登录 |
| `/api/auth/logout` | POST | 用户登出 |
| `/api/auth/refresh` | POST | 刷新Token |
| `/api/auth/change-password` | POST | 修改密码 |

### 物资管理接口

| 接口 | 方法 | 描述 |
|------|------|------|
| `/api/material` | GET | 获取物资列表 |
| `/api/material/{id}` | GET | 获取物资详情 |
| `/api/material` | POST | 创建物资 |
| `/api/material/{id}` | PUT | 更新物资 |
| `/api/material/{id}` | DELETE | 删除物资 |
| `/api/material/import` | POST | 导入物资 |
| `/api/material/export` | POST | 导出物资 |

### 排班管理接口

| 接口 | 方法 | 描述 |
|------|------|------|
| `/api/schedule` | GET | 获取排班列表 |
| `/api/schedule/{id}` | GET | 获取排班详情 |
| `/api/schedule` | POST | 创建排班 |
| `/api/schedule/batch` | POST | 批量创建排班 |
| `/api/schedule/{id}` | PUT | 更新排班 |
| `/api/schedule/{id}` | DELETE | 删除排班 |
| `/api/schedule/overview` | GET | 排班一览表 |
| `/api/schedule/statistics` | GET | 排班统计 |

### 基础数据接口

| 接口 | 方法 | 描述 |
|------|------|------|
| `/api/basicdata/hospitals` | GET | 获取医院列表 |
| `/api/basicdata/departments` | GET | 获取科室列表 |
| `/api/basicdata/shifts` | GET | 获取班次列表 |
| `/api/basicdata/person-ranks` | GET | 获取职级列表 |
| `/api/basicdata/person-titles` | GET | 获取职称列表 |

### 导入导出接口

| 接口 | 方法 | 描述 |
|------|------|------|
| `/api/importexport/material-template` | GET | 下载物资导入模板 |
| `/api/importexport/schedule-template/{type}` | GET | 下载排班导入模板 |

## 数据库实体

| 表名 | 描述 |
|------|------|
| `t_material` | 物资表 |
| `t_schedule` | 排班表 |
| `t_user` | 用户表 |
| `t_hospital` | 医院表 |
| `t_department` | 科室表 |
| `t_shift` | 班次表 |
| `t_person` | 人员表 |
| `t_person_rank` | 人员职级表 |
| `t_person_title` | 人员职称表 |
| `t_material_threshold` | 物资库存阈值配置表 |

## 开发说明

### 添加新功能

1. 在 `HDEMS.Domain/Entities` 添加实体
2. 在 `HDEMS.Application/DTOs` 添加DTO
3. 在 `HDEMS.Application/Interfaces` 添加服务接口
4. 在 `HDEMS.Application/Services` 实现服务
5. 在 `HDEMS.Api/Controllers` 添加控制器

### 数据库同步

开发环境下设置 `AutoSyncStructure: true`，FreeSql 会自动同步数据库结构。

生产环境请使用数据库迁移脚本。
