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
  <b>Scm.Net WPF Desktop Client</b> — A native Windows desktop application framework for enterprise management systems.
</p>

<p align="center">
  <a href="./README.md">简体中文</a> | English
</p>

---

## 📖 Introduction

**Scm.Wpf** is the WPF desktop client of [Scm.Net](https://gitee.com/leadiot/scm.net), providing a localized enterprise management backend for Windows operating systems. Built on .NET 10.0 + WPF architecture, it supports two authentication modes: **Operator Login** and **Terminal Binding**, along with two data access methods: **Local Database** and **Remote API**.

Products built on this framework include: OMS (Order Management), WMS (Warehouse Management), TMS (Transportation Management), DMS (Distribution Management), BMS (Billing Management), YMS (Yard Management), EAM (Asset Management), and IOT (Internet of Things Management).

---

## ✨ Key Features

### Authentication
- **Operator Login** — Username/password authentication with automatic token refresh and expiry handling
- **OIDC/OAuth2** — Third-party federated login (WeCom, DingTalk, etc.)
- **Terminal Binding** — Device binding authentication for unattended terminal scenarios
- **Single Instance** — Mutex-based singleton enforcement; duplicate launches activate the existing window

### Data Access
- **Local Database** — SqlSugar ORM with SQLite, supporting automatic table creation and version upgrades
- **Remote API** — Full RESTful API invocation with unified auth headers and error code handling
- **Data Validation** — Property-level validation via `INotifyDataErrorInfo`

### UI Components
- **PageGrid** — Feature-rich paged data grid
  - Customizable columns, search panel, advanced query
  - Data export: CSV / JSON / SQL / TXT / XLS
  - Inline editing, paginated navigation, multi-select operations
- **NavigationDrawer** — Side navigation drawer with cascading menu tree
- **Toast Notifications** — Success, error, warning status prompts
- **Icon System** — Material Design icon pack integration
- **Theming** — Light / Dark theme switching

### Auto Upgrade
- Version detection and comparison
- Incremental update support
- Download progress display
- Automatic installation and restart

### Menu Action System
- **AAction Abstraction** — Menu-driven action dispatch mechanism
- **BrowserAction** — Embedded WebView2 browser pages
- **ViewAction** — Open WPF native views
- **NativeAction** — Execute local database business logic
- **RemoteAction** — Invoke remote API business logic

---

## 🛠 Technology Stack

| Technology | Version | Description |
| --- | --- | --- |
| [.NET](https://dotnet.microsoft.com) | 10.0 | Runtime framework |
| [WPF](https://docs.microsoft.com/en-us/dotnet/wpf/) | 10.0 | Desktop UI framework |
| [HandyControl](https://handyorg.github.io/handycontrol/) | 3.5.1 | WPF control library |
| [SqlSugarCore](https://www.donet5.com/) | 5.1.4 | ORM data access framework |
| [NLog](https://nlog-project.org/) | 6.1.1 | Logging framework |
| [Newtonsoft.Json](https://www.newtonsoft.com/json) | 13.0.4 | JSON serialization |
| [MiniExcel](https://github.com/mini-software/MiniExcel) | 1.43.0 | Excel import/export |
| [CsvHelper](https://joshclose.github.io/CsvHelper/) | 33.1.0 | CSV file handling |
| [MahApps.Metro.IconPacks.Material](https://github.com/MahApps/MahApps.Metro.IconPacks) | 6.2.1 | Material Design icons |
| [Microsoft.Web.WebView2](https://developer.microsoft.com/microsoft-edge/webview2/) | 1.0.3856.49 | Embedded web browser |
| [System.Management](https://learn.microsoft.com/dotnet/api/system.management) | 10.0.5 | WMI system information |

---

## 📁 Project Structure

| Project | Description |
| --- | --- |
| `Scm.Wpf` | Main application entry (App.xaml / MainWindow.xaml) |
| `Scm.Client` | Client core library (HTTP base class, authentication, DTO) |
| `Scm.Client.Dao` | Data access layer (SqlSugar DAO base, SQL helpers) |
| `Scm.Client.Dvo` | Data value objects (MVVM base, property notification, validation) |
| `Scm.View` | UI component library (PageGrid, NavigationDrawer, Toast, etc.) |
| `Scm.Samples` | Usage examples (local database + remote API scenarios) |
| `Test` | Test project |

### Directory Layout

```
scm.wpf/
├── Scm.Wpf/                     # Main application
│   ├── Actions/                 # Menu action executors
│   ├── Config/                  # Configuration management
│   ├── Controls/                # WPF custom controls
│   ├── Login/                   # Login module
│   │   ├── Auth/                # Authentication windows
│   │   ├── OperatorWindow/      # Operator login window
│   │   └── TerminalWindow/      # Terminal binding window
│   ├── Views/                   # Business views
│   │   ├── About/               # About page
│   │   ├── Demo/                # Demo pages
│   │   ├── Home/                # Dashboard home
│   │   └── Tasks/               # Task scheduling pages
│   ├── App.xaml                 # Application entry (singleton Mutex + startup)
│   ├── MainWindow.xaml          # Main window
│   └── appsettings.json         # Application config
├── Scm.Client/                  # Client core library (netstandard2.0)
│   ├── Dto/                     # Data transfer objects
│   │   ├── Auth/                # Auth-related DTOs
│   │   └── Bind/                # Terminal binding DTOs
│   ├── ScmClient.cs             # Abstract client base (HTTP + Token)
│   ├── ScmOperator.cs           # Operator client
│   └── ScmTerminal.cs           # Terminal client
├── Scm.Client.Dao/              # Data access layer
│   ├── ScmDao.cs                # DAO base (primary key, ID generation)
│   ├── ScmDataDao.cs            # Data DAO (CRUD)
│   ├── ScmVerDao.cs             # Version DAO (auto upgrade)
│   └── SqlHelper.cs             # SQL helper class
├── Scm.Client.Dvo/              # Data value objects layer
│   ├── ScmDvo.cs                # Base class (INotifyPropertyChanged + validation)
│   ├── ScmDataDvo.cs            # Data DVO
│   ├── ScmCommand.cs            # Command implementation (ICommand)
│   └── ScmSearchParamsDvo.cs    # Search parameters DVO
├── Scm.View/                    # UI component library
│   ├── Actions/                 # Action abstractions
│   │   ├── AAction.cs           # Abstract action base
│   │   ├── BrowserAction.cs     # WebView2 browser action
│   │   ├── ViewAction.cs        # WPF view action
│   │   └── Samples/             # Sample action set
│   ├── Config/                  # Configuration models
│   │   ├── SqlConfig.cs         # Database connection config
│   │   └── UpgradeConfig.cs     # Auto upgrade config
│   ├── Controls/                # Custom controls
│   │   ├── Attach/              # Attached properties
│   │   ├── Windows/             # Window controls
│   │   ├── PageGrid.xaml        # Paged data grid
│   │   ├── NavigationDrawer.cs  # Navigation drawer
│   │   ├── NavigationMenu.xaml  # Navigation menu
│   │   └── ToastControl.xaml    # Toast notifications
│   ├── Converters/              # Value converters
│   ├── Models/                  # UI data models
│   └── Views/                   # Common views
├── Scm.Samples/                 # Sample project
│   ├── Views/Native/            # Local database examples
│   └── Views/Remote/            # Remote API examples
├── Libs/                        # Pre-compiled library references
│   ├── net10.0/                 # .NET 10.0 dependencies
│   └── netstandard2.0/          # .NET Standard 2.0 dependencies
├── Test/                        # Test project
└── Scm.Wpf.sln                  # Solution file
```

---

## 🔧 Prerequisites

| Tool | Version | Download |
| --- | --- | --- |
| .NET SDK | ≥ 10.0 | [https://dotnet.microsoft.com](https://dotnet.microsoft.com) |
| Visual Studio | ≥ 2026 | [https://visualstudio.microsoft.com](https://visualstudio.microsoft.com) |
| Windows | ≥ 10 | — |

---

## 🚀 Quick Start

### 1. Clone

```bash
git clone https://gitee.com/leadiot/scm.wpf.git
```

### 2. Configure

Edit `appsettings.json`:

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

### 3. Run

```bash
cd Scm.Wpf
dotnet run
```

Or open `Scm.Wpf.sln` in Visual Studio, set `Scm.Wpf` as the startup project, and press `F5`.

---

## 📄 Core Classes

| Class | Description |
| --- | --- |
| `ScmClient` | Abstract client base class, encapsulates HTTP requests, Token management, file upload/download |
| `ScmOperator` | Operator client, supports password login, OIDC/OAuth2, auto token refresh |
| `ScmTerminal` | Terminal client, device binding authentication for unattended scenarios |
| `ScmDvo` | Data value object base class, implements `INotifyPropertyChanged` + `INotifyDataErrorInfo` |
| `ScmDao` | DAO base class, encapsulates SqlSugar CRUD operations and ID generation |
| `PageGrid` | Paged data grid control, supports search, edit, export, pagination |
| `AAction` | Abstract action base class, menu-driven view switching and business dispatch |

### Example: Custom DVO

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
            AddError(nameof(Name), "Name cannot be empty");
        }
        return !HasErrors;
    }
}
```

### Example: Local Database Query

```csharp
var searchParams = new SearchParamsDvo
{
    Key = "keyword",
    Status = ScmRowStatusEnum.Enabled,
    Page = 1,
    Limit = 20
};

await SearchAsync(searchParams.Page);
```

### Example: Remote API Call

```csharp
var result = await _client.GetObjectAsync<ScmSearchPageResponse<SearchResultDataDvo>>(
    "/api/demo/list",
    new Dictionary<string, string>
    {
        { "key", searchParams.Key },
        { "status", ((int)searchParams.Status).ToString() }
    });
```

---

## 🔗 Related Links

- [Scm.Net Backend](https://gitee.com/leadiot/scm.net) — .NET 10.0 + Vue 3 enterprise management framework
- [Scm.Vue Frontend](https://gitee.com/leadiot/scm.vue) — Vue 3 + Vite + Element Plus frontend framework
- [Online Demo](http://www.c-scm.net)
- [SqlSugar Documentation](https://www.donet5.com/)
- [HandyControl Documentation](https://handyorg.github.io/handycontrol/)

---

## 📄 License

This project is licensed under the [MIT License](LICENSE), intended for learning and reference purposes only.
