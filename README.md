<p align="center">
  <img src="logo.png" alt="Scm.Net Logo" width="120" />
</p>

<h1 align="center">Scm.Wpf</h1>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-11.0-512BD4?logo=dotnet" alt=".NET 11" />
  <img src="https://img.shields.io/badge/WPF-11.0-512BD4?logo=windows" alt="WPF" />
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

**Scm.Wpf** 是 [Scm.Net](https://gitee.com/leadiot/scm.net) 的 WPF 桌面客户端，为 Windows 操作系统提供本地化的企业管理后台。采用 .NET 11.0 + WPF 架构，支持**操作员登录**和**终端绑定**两种认证模式，同时支持**本地数据库**和**远程 API** 两种数据访问方式。

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

- **PageGrid** — 强大的分页数据网格组件（[使用说明](docs/PageGrid.md)）
  - 声明式列配置：列类型、宽度、对齐、格式化、是否导出
  - 搜索面板、自定义内容区、高级查询、抽屉式详情/编辑
  - 运行时列设置：勾选显示/隐藏、上下调整列顺序（锚定于「选项」按钮的圆角阴影浮层）
  - 数据导出：XLSX / XLS / CSV / JSON / SQL
  - 打印：A4 分页预览 + 系统打印对话框
  - 单元格双击事件、行右键菜单、多选操作、分页导航
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
| [.NET](https://dotnet.microsoft.com)                                                   | 11.0        | 运行时框架              |
| [WPF](https://docs.microsoft.com/zh-cn/dotnet/wpf/)                                    | 11.0        | 桌面 UI 框架           |
| [HandyControl](https://handyorg.github.io/handycontrol/)                               | 3.5.1       | WPF 控件库            |
| [SqlSugarCore](https://www.donet5.com/)                                                | 5.1.4.221   | ORM 数据访问框架         |
| [NLog](https://nlog-project.org/)                                                      | 6.2.2       | 日志框架               |
| [Newtonsoft.Json](https://www.newtonsoft.com/json)                                     | 13.0.4      | JSON 序列化           |
| [MiniExcel](https://github.com/mini-software/MiniExcel)                                | 1.47.0      | Excel 导入导出         |
| [CsvHelper](https://joshclose.github.io/CsvHelper/)                                    | 34.0.0      | CSV 文件处理           |
| [MahApps.Metro.IconPacks.Material](https://github.com/MahApps/MahApps.Metro.IconPacks) | 6.2.1       | Material Design 图标 |
| [Microsoft.Web.WebView2](https://developer.microsoft.com/microsoft-edge/webview2/)     | 1.0.4314.35 | 内嵌 Web 浏览器         |
| [System.Management](https://learn.microsoft.com/dotnet/api/system.management)          | 11.0.0      | WMI 系统信息           |

***

## 📁 项目结构

| 项目               | 说明                                                       |
| ---------------- | -------------------------------------------------------- |
| `Scm.Wpf`        | 主应用程序入口（`net11.0-windows`，App.xaml / MainWindow.xaml）     |
| `Scm.Client`     | 客户端核心库（HTTP 客户端基类、操作员/终端认证、DTO）                          |
| `Scm.Client.Dao` | 数据访问层（SqlSugar DAO 基类、SQL 辅助）                             |
| `Scm.Client.Dvo` | 数据值对象（MVVM 基类、属性通知、数据校验）                                 |
| `Scm.View`       | UI 组件库（PageGrid、NavigationDrawer、Toast、打印/导出服务等）           |
| `Scm.Samples`    | 使用示例（本地数据库场景 + 远程 API 场景）                                |
| `Scm.Common.Os`  | 系统与剪贴板辅助（文本/图片/文件复制）                                     |
| `Scm.Image`      | 独立 WPF 图片查看器示例（缩放、拖拽、剪贴板）                               |
| `Scm.Audio`      | WPF 空白模板工程（预留）                                           |
| `Scm.Video`      | WPF 空白模板工程（预留）                                           |
| `WinForm`        | WinForms 工具集（HTTP 请求、MQTT 调试、抖音解析）                        |
| `Test`           | 空控制台项目（占位，未配置测试框架）                                       |

### 目录结构

```
scm.wpf/
├── Scm.Wpf/                     # 主应用程序
│   ├── Assets/Styles/           # 全局样式（Style.xaml 设计令牌）
│   ├── Config/                  # 配置模型（AppSettings、EnvConfig）
│   ├── Data/                    # SQLite 数据库与初始化脚本
│   ├── Helper/                  # 辅助类（PasswordBoxHelper）
│   ├── Login/                   # 登录模块
│   │   ├── Auth/                # 认证视图（密码 / OIDC）
│   │   ├── OperatorWindow.*     # 操作员登录窗口
│   │   └── TerminalWindow.*     # 终端绑定窗口
│   ├── Utils/                   # 日志等工具
│   ├── Views/                   # 业务视图
│   │   ├── About/               # 关于页面
│   │   ├── Account/             # 账户页面
│   │   ├── Browser/             # 内嵌浏览器
│   │   ├── Config/              # 参数配置
│   │   ├── Demo/                # 演示功能页面
│   │   ├── Home/                # 主页工作台
│   │   ├── Tasks/               # 定时任务
│   │   └── Uc/                  # 通用视图组件
│   ├── App.xaml                 # 应用入口（单例 Mutex + 启动逻辑）
│   ├── MainWindow.xaml          # 主窗口
│   ├── SplashWindow.xaml        # 启动引导窗口（日志、配置、数据库初始化）
│   ├── ScmClientEnv.cs          # 版本与环境信息
│   └── appsettings.json         # 应用配置
├── Scm.Client/                  # 客户端核心库（net11.0）
│   ├── Dto/Operator/            # 操作员认证 DTO
│   ├── Dto/Terminal/            # 终端绑定 DTO
│   ├── ScmClient.cs             # 客户端基类（HTTP + Token）
│   ├── ScmOperator.cs           # 操作员客户端
│   ├── ScmTerminal.cs           # 终端客户端
│   └── ScmToken.cs              # Token 模型
├── Scm.Client.Dao/              # 数据访问层
│   ├── ScmDao.cs                # DAO 基类（主键、ID 生成）
│   ├── ScmDataDao.cs            # 数据 DAO（CRUD + 审计字段）
│   ├── ScmVerDao.cs             # 版本信息 DAO（自动升级）
│   └── SqlHelper.cs             # SqlSugar 初始化与辅助
├── Scm.Client.Dvo/              # 数据值对象层
│   ├── ScmDvo.cs                # 基类（INotifyPropertyChanged + 数据校验）
│   ├── ScmDataDvo.cs            # 数据 DVO
│   ├── ScmCommand.cs            # 命令实现（ICommand）
│   └── ScmSearchParamsDvo.cs    # 搜索参数 DVO
├── Scm.View/                    # UI 组件库
│   ├── Actions/                 # 动作抽象与示例
│   │   ├── AAction.cs           # 抽象动作基类
│   │   ├── BrowserAction.cs     # WebView2 浏览器动作
│   │   ├── ViewAction.cs        # WPF 视图动作
│   │   └── Samples/             # NativeAction / RemoteAction
│   ├── Config/                  # 配置模型
│   ├── Controls/                # 自定义控件
│   │   ├── Attach/              # 附加行为（DataGridColumnsBehavior、IconElement）
│   │   ├── Windows/             # 通用窗口（浏览器、消息、输入、异常）
│   │   ├── PageGrid.xaml        # 分页数据网格
│   │   ├── PageGridColumnsView.xaml  # 列显示/排序设置面板
│   │   ├── PrintPreviewWindow.xaml   # 打印预览窗口
│   │   ├── NavigationDrawer.*   # 导航抽屉
│   │   ├── NavigationMenu.xaml  # 导航菜单
│   │   └── ToastControl.xaml    # Toast 通知
│   ├── Converters/              # 值转换器
│   ├── Dvo/                     # 菜单 DVO
│   ├── Enums/                   # 登录类型、窗口状态等枚举
│   ├── Helper/                  # GridExporter / GridPrinter / ScmDbHelper 等
│   ├── Models/                  # ScmColumnInfo、PageGridColumnOption、TabModel
│   └── ScmPageGridDvo.cs        # 分页列表 ViewModel 基类（含泛型版本）
├── Scm.Samples/                 # 示例项目
│   ├── Dao/Native/              # 本地示例 DAO
│   ├── Dto/Remote/              # 远程示例 DTO
│   ├── Helper/                  # 示例数据库初始化
│   └── Views/
│       ├── Native/              # 本地数据库示例（Main/Search/Info/Edit/Custom）
│       └── Remote/              # 远程 API 示例（Main/Search/Info/Edit/Custom）
├── Scm.Common.Os/               # 系统辅助库
├── Scm.Image/                   # 图片查看器示例工程
├── Scm.Audio/                   # WPF 空白模板工程（预留）
├── Scm.Video/                   # WPF 空白模板工程（预留）
├── WinForm/                     # WinForms 工具集
├── docs/                        # 文档（PageGrid 使用说明等）
├── Test/                        # 占位控制台项目
└── Scm.Wpf.sln                  # 解决方案文件

../Libs/                         # 预编译库引用（位于仓库上一级目录，需单独签出）
├── net10.0/                     # Scm.Dto / Scm.Uid / Upgrade.Net 等
└── netstandard2.0/              # Scm.Common* / Scm.Oidc.Client 等
```

***

## 🔧 环境要求

| 工具            | 版本要求   | 下载地址                                 |
| ------------- | ------ | ------------------------------------ |
| .NET SDK      | ≥ 11.0 | <https://dotnet.microsoft.com>       |
| Visual Studio | ≥ 2026 | <https://visualstudio.microsoft.com> |
| Windows       | ≥ 10   | —                                    |

***

## 项目依赖

签出本项目时，请同时签出 [Libs](https://gitee.com/leadiot/libs) 到**仓库上一级目录**下的 `Libs/`（各项目以 `..\..\Libs\` 相对路径引用）。

缺少 `Libs/` 时，`Scm.View`、`Scm.Wpf` 等项目因找不到 `Scm.Common*.dll`、`Scm.Uid.dll`、`Upgrade.Net.dll` 等程序集而无法编译。

***

## 🚀 快速开始

### 1. 获取代码

```bash
git clone https://gitee.com/leadiot/scm.wpf.git
cd scm.wpf
git clone https://gitee.com/leadiot/libs.git ../Libs
```

### 2. 配置

编辑 `Scm.Wpf/appsettings.json`：

```json
{
  "Env": {
    "LoginMode": "Operator",
    "UpgradeFilePath": "Upgrade/Upgrade.Wpf.exe",
    "UpgradeJsonPath": "Upgrade/Upgrade.json"
  },
  "Server": {
    "Host": "localhost",
    "Port": "9999"
  },
  "Sql": {
    "Type": "Sqlite",
    "Text": "Data Source=./Data/scm.db;"
  },
  "Oidc": {
    "app_key": "",
    "app_secret": "",
    "redirect_uri": "http://c-scm.net/oauth/callback",
    "scope": "openid"
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

| 类名                     | 说明                                                           |
| ---------------------- | ------------------------------------------------------------ |
| `ScmClient`            | 抽象客户端基类，封装 HTTP 请求、Token 管理、文件上传下载                           |
| `ScmOperator`          | 操作员客户端，支持密码登录、OIDC/OAuth2、Token 自动刷新                         |
| `ScmTerminal`          | 终端客户端，设备绑定认证，用于无人值守终端场景                                      |
| `ScmDvo`               | 数据值对象基类，实现 `INotifyPropertyChanged` + `INotifyDataErrorInfo` |
| `ScmDao`               | DAO 基类，封装 SqlSugar CRUD 操作与 ID 生成                            |
| `ScmPageGridDvo<T>`    | 分页列表 ViewModel 基类，统一分页导航，子类仅实现 `LoadPageAsync`                 |
| `PageGrid`             | 分页数据网格控件，支持搜索、列设置、编辑、导出、打印（详见 [使用说明](docs/PageGrid.md)）          |
| `DataGridColumnsBehavior` | 附加行为，按 `ScmColumnInfo` 自动生成 DataGrid 列                    |
| `GridExporter`         | 列表导出服务，支持 XLSX / XLS / CSV / JSON / SQL                      |
| `GridPrinter`          | 列表打印服务，支持 A4 预览与直接打印                                         |
| `AAction`              | 动作抽象基类，菜单驱动的视图切换与业务分发                                        |

### 示例：分页列表 ViewModel

```csharp
public class UserDvo : ScmPageGridDvo<UserItemDvo>
{
    public UserDvo()
    {
        Columns = new List<ScmColumnInfo>
        {
            new ScmColumnInfo { Value = "codec", Label = "编码", Width = "120" },
            new ScmColumnInfo { Value = "namec", Label = "名称", Width = "*" }
        };
    }

    protected override async Task<ScmPageResult<UserItemDvo>> LoadPageAsync()
    {
        // 使用 Key / PageIndex / PageItems 查询当前页数据
        var page = await _dao.GetPageAsync(Key, PageIndex, PageItems);
        return new ScmPageResult<UserItemDvo> { Items = page.Items, Total = page.Total };
    }
}
```

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

## 📚 文档

- [PageGrid 使用说明](docs/PageGrid.md) — 快速开始、列配置、交互事件、导出、打印、常见问题

***

## 🔗 相关链接

- [Scm.Net 后端项目](https://gitee.com/leadiot/scm.net) — .NET 11.0 + Vue 3 企业级中后台框架
- [Scm.Vue 前端项目](https://gitee.com/leadiot/scm.vue) — Vue 3 + Vite + Element Plus 前端框架
- [Libs 预编译库](https://gitee.com/leadiot/libs) — 本项目必需的本地依赖
- [在线演示](http://www.c-scm.net)
- [SqlSugar 文档](https://www.donet5.com/)
- [HandyControl 文档](https://handyorg.github.io/handycontrol/)

***

## 📄 开源协议

本项目采用 [MIT License](LICENSE)，仅供学习交流使用。
