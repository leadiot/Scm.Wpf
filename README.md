<p align="center">
  <img src="logo.png" alt="Scm.Net Logo" width="120" />
</p>

<h1 align="center">Scm.Wpf</h1>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet" alt=".NET 10" />
  <img src="https://img.shields.io/badge/WPF-10.0-512BD4?logo=windows" alt="WPF" />
  <img src="https://img.shields.io/badge/license-MIT-green" alt="MIT License" />
  <img src="https://img.shields.io/badge/platform-Windows-lightgrey" alt="Windows" />
</p>

<p align="center">
  <b>Scm.Net WPF 桌面客户端</b> — 企业级管理系统的 Windows 原生桌面应用框架。
</p>

<p align="center">
  简体中文 | <a href="./README.en.md">English</a>
</p>

***

## 📖 项目简介

**Scm.Wpf** 是 [Scm.Net](https://gitee.com/leadiot/scm.net) 的 WPF 桌面客户端，为 Windows 操作系统提供本地化的企业管理后台。采用 .NET 10.0 + WPF 架构，支持**操作员登录**和**终端绑定**两种认证模式，同时支持**本地数据库**和**远程 API** 两种数据访问方式。

基于本框架已构建的产品包括：OMS（订单管理）、WMS（仓储管理）、TMS（运输管理）、DMS（配送管理）、BMS（计费管理）、YMS（园区管理）、EAM（资产管理）、IOT（物联网管理）等。

***

## ✨ 核心特性

### 认证系统

- **操作员登录** — 用户名密码认证，支持 Token 自动刷新与过期处理
- **OIDC/OAuth2 认证** — 支持第三方联合登录（如企业微信、钉钉等）
- **终端绑定** — 设备绑定认证机制，适用于无人值守终端场景
- **单实例运行** — Mutex 确保应用单进程运行，重复启动时自动激活已有窗口

### 数据访问

- **本地数据库** — 基于 SqlSugar ORM 操作 SQLite，支持自动建表和版本升级
- **远程 API** — 完整 RESTful API 调用封装，统一处理认证头、错误码
- **数据验证** — `INotifyDataErrorInfo` 实现属性级字段校验

### UI 组件

- **PageGrid** — 强大的分页数据网格组件
  - 自定义列配置、搜索面板、高级查询
  - 数据导出：CSV / JSON / SQL / TXT / XLS
  - 内联编辑、分页导航、多选操作
- **NavigationDrawer** — 侧边导航抽屉，支持菜单树级联
- **Toast 通知** — 成功、错误、警告等状态提示
- **图标系统** — 集成 Material Design 图标库
- **主题支持** — 亮色/暗色主题切换

### 自动升级

- 版本检测与对比
- 增量更新支持
- 下载进度显示
- 自动安装与重启

### 菜单动作系统

- **AAction 抽象** — 基于菜单驱动的动作分发机制
- **BrowserAction** — 内嵌 WebView2 浏览器页面
- **ViewAction** — 打开 WPF 原生视图
- **NativeAction** — 执行本地数据库业务逻辑
- **RemoteAction** — 调用远程 API 业务逻辑

***

## 🛠 技术栈

| 技术                                                                                     | 版本          | 说明                 |
| -------------------------------------------------------------------------------------- | ----------- | ------------------ |
| [.NET](https://dotnet.microsoft.com)                                                   | 10.0        | 运行时框架              |
| [WPF](https://docs.microsoft.com/zh-cn/dotnet/wpf/)                                    | 10.0        | 桌面 UI 框架           |
| [HandyControl](https://handyorg.github.io/handycontrol/)                               | 3.5.1       | WPF 控件库            |
| [SqlSugarCore](https://www.donet5.com/)                                                | 5.1.4       | ORM 数据访问框架         |
| [NLog](https://nlog-project.org/)                                                      | 6.1.1       | 日志框架               |
| [Newtonsoft.Json](https://www.newtonsoft.com/json)                                     | 13.0.4      | JSON 序列化           |
| [MiniExcel](https://github.com/mini-software/MiniExcel)                                | 1.43.0      | Excel 导入导出         |
| [CsvHelper](https://joshclose.github.io/CsvHelper/)                                    | 33.1.0      | CSV 文件处理           |
| [MahApps.Metro.IconPacks.Material](https://github.com/MahApps/MahApps.Metro.IconPacks) | 6.2.1       | Material Design 图标 |
| [Microsoft.Web.WebView2](https://developer.microsoft.com/microsoft-edge/webview2/)     | 1.0.3856.49 | 内嵌 Web 浏览器         |
| [System.Management](https://learn.microsoft.com/dotnet/api/system.management)          | 10.0.5      | WMI 系统信息           |

***

## 📁 项目结构

| 项目               | 说明                                        |
| ---------------- | ----------------------------------------- |
| `Scm.Wpf`        | 主应用程序入口（App.xaml / MainWindow\.xaml）      |
| `Scm.Client`     | 客户端核心库（HTTP 客户端基类、认证、DTO）                 |
| `Scm.Client.Dao` | 数据访问层（SqlSugar DAO 基类、SQL 辅助）             |
| `Scm.Client.Dvo` | 数据值对象（MVVM 基类、属性通知、数据校验）                  |
| `Scm.View`       | UI 组件库（PageGrid、NavigationDrawer、Toast 等） |
| `Scm.Samples`    | 使用示例（本地数据库场景 + 远程 API 场景）                 |

### 目录结构

```
scm.wpf/
├── Scm.Wpf/                     # 主应用程序
│   ├── Actions/                 # 菜单动作执行器
│   ├── Config/                  # 配置管理
│   ├── Controls/                # WPF 自定义控件
│   ├── Login/                   # 登录模块
│   │   ├── Auth/                # 认证相关窗口
│   │   ├── OperatorWindow/      # 操作员登录窗口
│   │   └── TerminalWindow/      # 终端绑定窗口
│   ├── Views/                   # 业务视图
│   │   ├── About/               # 关于页面
│   │   ├── Demo/                # 演示功能页面
│   │   ├── Home/                # 主页工作台
│   │   └── Tasks/               # 任务调度页面
│   ├── App.xaml                 # 应用入口（单例 Mutex + 启动逻辑）
│   ├── MainWindow.xaml          # 主窗口
│   └── appsettings.json         # 应用配置
├── Scm.Client/                  # 客户端核心库（netstandard2.0）
│   ├── Dto/                     # 数据传输对象
│   │   ├── Auth/                # 认证相关 DTO
│   │   └── Bind/                # 终端绑定 DTO
│   ├── ScmClient.cs             # 抽象客户端基类（HTTP + Token）
│   ├── ScmOperator.cs           # 操作员客户端
│   └── ScmTerminal.cs           # 终端客户端
├── Scm.Client.Dao/              # 数据访问层
│   ├── ScmDao.cs                # DAO 基类（主键、ID 生成）
│   ├── ScmDataDao.cs            # 数据 DAO（CRUD）
│   ├── ScmVerDao.cs             # 版本信息 DAO（自动升级）
│   └── SqlHelper.cs             # SQL 辅助类
├── Scm.Client.Dvo/              # 数据值对象层
│   ├── ScmDvo.cs                # 基类（INotifyPropertyChanged + 数据校验）
│   ├── ScmDataDvo.cs            # 数据 DVO
│   ├── ScmCommand.cs            # 命令实现（ICommand）
│   └── ScmSearchParamsDvo.cs    # 搜索参数 DVO
├── Scm.View/                    # UI 组件库
│   ├── Actions/                 # 动作抽象
│   │   ├── AAction.cs           # 抽象动作基类
│   │   ├── BrowserAction.cs     # WebView2 浏览器动作
│   │   ├── ViewAction.cs        # WPF 视图动作
│   │   └── Samples/             # 示例动作集
│   ├── Config/                  # 配置模型
│   │   ├── SqlConfig.cs         # 数据库连接配置
│   │   └── UpgradeConfig.cs     # 自动升级配置
│   ├── Controls/                # 自定义控件
│   │   ├── Attach/              # 附加属性
│   │   ├── Windows/             # 窗口控件
│   │   ├── PageGrid.xaml        # 分页数据网格
│   │   ├── NavigationDrawer.cs  # 导航抽屉
│   │   ├── NavigationMenu.xaml  # 导航菜单
│   │   └── ToastControl.xaml    # Toast 通知
│   ├── Converters/              # 值转换器
│   ├── Models/                  # UI 数据模型
│   └── Views/                   # 通用视图
├── Scm.Samples/                 # 示例项目
│   ├── Views/Native/            # 本地数据库示例
│   └── Views/Remote/            # 远程 API 示例
├── Libs/                        # 预编译库引用
│   ├── net10.0/                 # .NET 10.0 依赖
│   └── netstandard2.0/          # .NET Standard 2.0 依赖
├── Test/                        # 测试项目
└── Scm.Wpf.sln                  # 解决方案文件
```

***

## 🔧 环境要求

| 工具            | 版本要求   | 下载地址                                 |
| ------------- | ------ | ------------------------------------ |
| .NET SDK      | ≥ 10.0 | <https://dotnet.microsoft.com>       |
| Visual Studio | ≥ 2026 | <https://visualstudio.microsoft.com> |
| Windows       | ≥ 10   | —                                    |

***

## 🚀 快速开始

### 1. 获取代码

```bash
git clone https://gitee.com/leadiot/scm.wpf.git
```

### 2. 配置

编辑 `appsettings.json`：

```json
{
  "AutoStartup": true,
  "WindowState": "Normal",
  "Env": {
    "LoginMode": "Operator",
    "UpgradeJsonName": "upgrade.json"
  },
  "Sql": {
    "Type": "Sqlite",
    "Text": "Data Source=scm.db"
  },
  "Server": {
    "Host": "localhost",
    "Port": "9999"
  }
}
```

### 3. 运行

```bash
cd Scm.Wpf
dotnet run
```

或在 Visual Studio 中直接打开 `Scm.Wpf.sln`，设置 `Scm.Wpf` 为启动项目，按 `F5` 运行。

***

## 📄 核心类说明

| 类名            | 说明                                                           |
| ------------- | ------------------------------------------------------------ |
| `ScmClient`   | 抽象客户端基类，封装 HTTP 请求、Token 管理、文件上传下载                           |
| `ScmOperator` | 操作员客户端，支持密码登录、OIDC/OAuth2、Token 自动刷新                         |
| `ScmTerminal` | 终端客户端，设备绑定认证，用于无人值守终端场景                                      |
| `ScmDvo`      | 数据值对象基类，实现 `INotifyPropertyChanged` + `INotifyDataErrorInfo` |
| `ScmDao`      | DAO 基类，封装 SqlSugar CRUD 操作与 ID 生成                            |
| `PageGrid`    | 分页数据网格控件，支持搜索、编辑、导出、分页                                       |
| `AAction`     | 动作抽象基类，菜单驱动的视图切换与业务分发                                        |

### 示例：自定义 DVO

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

### 示例：本地数据库查询

```csharp
var searchParams = new SearchParamsDvo
{
    Key = "关键字",
    Status = ScmRowStatusEnum.Enabled,
    Page = 1,
    Limit = 20
};

await SearchAsync(searchParams.Page);
```

### 示例：远程 API 调用

```csharp
var result = await _client.GetObjectAsync<ScmSearchPageResponse<SearchResultDataDvo>>(
    "/api/demo/list",
    new Dictionary<string, string>
    {
        { "key", searchParams.Key },
        { "status", ((int)searchParams.Status).ToString() }
    });
```

***

## 🔗 相关链接

- [Scm.Net 后端项目](https://gitee.com/leadiot/scm.net) — .NET 10.0 + Vue 3 企业级中后台框架
- [Scm.Vue 前端项目](https://gitee.com/leadiot/scm.vue) — Vue 3 + Vite + Element Plus 前端框架
- [在线演示](http://www.c-scm.net)
- [SqlSugar 文档](https://www.donet5.com/)
- [HandyControl 文档](https://handyorg.github.io/handycontrol/)

***

## 📄 开源协议

本项目采用 [MIT License](LICENSE)，仅供学习交流使用。
