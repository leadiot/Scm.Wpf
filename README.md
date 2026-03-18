

# SCM WPF 客户端框架

简体中文 | [English](./README-en.md)

## 项目简介

SCM WPF 是一个基于 WPF (Windows Presentation Foundation) 的客户端应用程序框架，提供完整的认证、数据访问、UI 组件和自动升级功能。该框架采用模块化设计，支持本地数据库和远程 API 两种数据操作方式。

## 项目结构

```
scm.wpf/
├── Libs/                          # 第三方依赖库
│   ├── net10.0/                   # .NET 10.0 依赖
│   └── netstandard2.0/            # .NET Standard 2.0 依赖
├── Scm.Client/                    # 客户端核心库
│   ├── Dto/                       # 数据传输对象
│   │   ├── Auth/                  # 认证相关 DTO
│   │   └── Bind/                  # 终端绑定相关 DTO
│   ├── ScmClient.cs               # 客户端基类
│   ├── ScmOperator.cs             # 操作员客户端
│   └── ScmTerminal.cs             # 终端客户端
├── Scm.Client.Dao/                # 数据访问层
│   ├── ScmDao.cs                  # 基础 DAO 类
│   ├── ScmDataDao.cs              # 数据 DAO
│   ├── ScmVerDao.cs               # 版本信息 DAO
│   └── SqlHelper.cs               # SQL 辅助类
├── Scm.Client.Dvo/                # 数据值对象
│   ├── ScmDvo.cs                  # 基础 DVO (实现 INotifyPropertyChanged)
│   ├── ScmDataDvo.cs              # 数据 DVO
│   ├── ScmCommand.cs              # 命令实现
│   └── ScmSearchParamsDvo.cs      # 搜索参数 DVO
├── Scm.View/                      # UI 组件库
│   ├── Controls/                  # 自定义控件
│   │   ├── PageGrid.xaml         # 分页数据网格
│   │   └── NavigationDrawer.xaml # 导航抽屉
│   ├── Converters/               # 值转换器
│   ├── Models/                   # 数据模型
│   └── Views/                    # 通用视图
├── Scm.Samples/                   # 示例项目
│   ├── Views/Native/             # 本地数据库示例
│   └── Views/Remote/             # 远程 API 示例
├── Scm.Upgrade/                   # 自动升级模块
└── Scm.Wpf/                       # 主应用程序
    ├── Actions/                   # 菜单动作
    ├── Config/                    # 配置管理
    ├── Controls/                  # WPF 控件
    ├── Login/                     # 登录模块
    │   ├── Auth/                  # 认证登录
    │   ├── OperatorWindow.xaml   # 操作员登录窗口
    │   └── TerminalWindow.xaml    # 终端绑定窗口
    ├── Views/                     # 业务视图
    │   ├── About/                 # 关于页面
    │   ├── Demo/                  # 演示页面
    │   ├── Home/                  # 主页
    │   └── Tasks/                 # 任务页面
    └── MainWindow.xaml            # 主窗口
```

## 功能特性

### 1. 认证系统
- **操作员登录**: 用户名密码认证
- **OIDC/OAuth2 认证**: 支持第三方 OAuth 登录
- **终端绑定**: 设备绑定认证机制
- **Token 管理**: 自动刷新和过期处理

### 2. 数据访问
- **本地数据库**: 使用 SqlSugar ORM 操作 SQLite
- **远程 API**: 完整的 RESTful API 调用支持
- **数据验证**: INotifyDataErrorInfo 实现字段验证

### 3. UI 组件
- **PageGrid**: 强大的分页数据网格组件
  - 支持列自定义
  - 数据导出 (CSV/JSON/SQL/TXT/XLS)
  - 分页导航
- **NavigationDrawer**: 侧边导航抽屉
- **Toast 通知**: 成功/错误/警告提示
- **多种菜单模式**: 侧边栏/顶部菜单

### 4. 自动升级
- 版本检测和对比
- 增量更新支持
- 下载进度显示
- 自动安装和重启

## 快速开始

### 环境要求
- .NET 10.0 或更高版本
- Windows 10/11

### 配置说明

在 `appsettings.json` 中配置应用设置：

```json
{
  "AutoStartup": true,
  "WindowState": "Normal",
  "Env": {
    "LoginMode": "Terminal",
    "UpgradeJsonName": "upgrade.json"
  },
  "Sql": {
    "Type": "Sqlite",
    "Text": "Data Source=scm.db"
  }
}
```

### 运行项目

1. 克隆仓库
```bash
git clone https://gitee.com/leadiot/scm.wpf.git
```

2. 使用 Visual Studio 打开 `Scm.Wpf.sln`

3. 设置启动项目为 `Scm.Wpf`

4. 按 F5 运行

## 示例代码

### 本地数据库示例

```csharp
// 创建搜索参数
var searchParams = new SearchParamsDvo
{
    Key = "关键字",
    Status = ScmRowStatusEnum.Enabled,
    Page = 1,
    Limit = 20
};

// 执行搜索
await SearchAsync(searchParams.Page);
```

### 远程 API 示例

```csharp
// 使用客户端调用 API
var result = await _client.GetObjectAsync<ScmSearchPageResponse<SearchResultDataDvo>>(
    "/api/demo/list", 
    new Dictionary<string, string>
    {
        { "key", searchParams.Key },
        { "status", ((int)searchParams.Status).ToString() }
    });
```

### 自定义 DVO

```csharp
public class MyDvo : ScmDvo
{
    private string _name;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }
    
    public override bool IsValid()
    {
        ClearErrors();
        if (string.IsNullOrEmpty(Name))
        {
            AddError(nameof(Name), "名称不能为空");
        }
        return !HasErrors;
    }
}
```

## 核心类说明

| 类名 | 说明 |
|------|------|
| `ScmClient` | 客户端基类，处理 HTTP 请求和 Token 管理 |
| `ScmOperator` | 操作员客户端，用于用户登录认证 |
| `ScmTerminal` | 终端客户端，用于设备绑定 |
| `ScmDvo` | 数据值对象基类，实现属性变更通知和数据验证 |
| `PageGrid` | 分页数据网格控件，支持 CRUD 操作 |
| `ScmDbHelper` | 数据库辅助类，管理数据库初始化和版本升级 |

## 技术栈

- **.NET 10.0** - 运行时框架
- **WPF** - UI 框架
- **SqlSugar** - ORM 框架
- **NLog** - 日志框架
- **HandyControl** - WPF 控件库

## 许可证

本项目仅供学习交流使用。

## 相关文档

- [SqlSugar 文档](https://www.donet5.com/)
- [HandyControl 文档](https://handyorg.github.io/handycontrol/)
- [WPF 官方文档](https://docs.microsoft.com/zh-cn/dotnet/wpf/)