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
  <b>Scm.Net WPF Desktop Client</b> — A native Windows desktop application framework for enterprise management systems.
</p>

<p align="center">
  <a href="./README.md">简体中文</a> | English
</p>

---

## 📖 Introduction

**Scm.Wpf** is the WPF desktop client of [Scm.Net](https://gitee.com/leadiot/scm.net), providing a localized enterprise management backend for Windows operating systems. Built on .NET 11.0 + WPF architecture, it supports two authentication modes: **Operator Login** and **Terminal Binding**, along with two data access methods: **Local Database** and **Remote API**.

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
- **PageGrid** — Feature-rich paged data grid ([usage guide](docs/PageGrid.md))
  - Declarative column config: type, width, alignment, formatting, export flag
  - Search panel, custom content area, advanced query, drawer-based detail/edit
  - Runtime column settings: show/hide by checkbox, reorder with up/down buttons (rounded, shadowed popup anchored to the Options button)
  - Data export: XLSX / XLS / CSV / JSON / SQL
  - Printing: A4 paginated preview + system print dialog
  - Cell double-click event, row context menu, multi-select operations, paged navigation
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
| [.NET](https://dotnet.microsoft.com) | 11.0 | Runtime framework |
| [WPF](https://docs.microsoft.com/en-us/dotnet/wpf/) | 11.0 | Desktop UI framework |
| [HandyControl](https://handyorg.github.io/handycontrol/) | 3.5.1 | WPF control library |
| [SqlSugarCore](https://www.donet5.com/) | 5.1.4.221 | ORM data access framework |
| [NLog](https://nlog-project.org/) | 6.2.2 | Logging framework |
| [Newtonsoft.Json](https://www.newtonsoft.com/json) | 13.0.4 | JSON serialization |
| [MiniExcel](https://github.com/mini-software/MiniExcel) | 1.47.0 | Excel import/export |
| [CsvHelper](https://joshclose.github.io/CsvHelper/) | 34.0.0 | CSV file handling |
| [MahApps.Metro.IconPacks.Material](https://github.com/MahApps/MahApps.Metro.IconPacks) | 6.2.1 | Material Design icons |
| [Microsoft.Web.WebView2](https://developer.microsoft.com/microsoft-edge/webview2/) | 1.0.4314.35 | Embedded web browser |
| [System.Management](https://learn.microsoft.com/dotnet/api/system.management) | 11.0.0 | WMI system information |

---

## 📁 Project Structure

| Project | Description |
| --- | --- |
| `Scm.Wpf` | Main application entry (`net11.0-windows`, App.xaml / MainWindow.xaml) |
| `Scm.Client` | Client core library (HTTP base class, operator/terminal auth, DTO) |
| `Scm.Client.Dao` | Data access layer (SqlSugar DAO base, SQL helpers) |
| `Scm.Client.Dvo` | Data value objects (MVVM base, property notification, validation) |
| `Scm.View` | UI component library (PageGrid, NavigationDrawer, Toast, print/export services) |
| `Scm.Samples` | Usage examples (local database + remote API scenarios) |
| `Scm.Common.Os` | OS and clipboard helpers (text/image/file copy) |
| `Scm.Image` | Standalone WPF image viewer sample (zoom, pan, clipboard) |
| `Scm.Audio` | Blank WPF template project (reserved) |
| `Scm.Video` | Blank WPF template project (reserved) |
| `WinForm` | WinForms tooling (HTTP request, MQTT debugging, Douyin parser) |
| `Test` | Empty console project (placeholder, no test framework configured) |

### Directory Layout

```
scm.wpf/
├── Scm.Wpf/                     # Main application
│   ├── Assets/Styles/           # Global styles (Style.xaml design tokens)
│   ├── Config/                  # Configuration models (AppSettings, EnvConfig)
│   ├── Data/                    # SQLite database and init scripts
│   ├── Helper/                  # Helpers (PasswordBoxHelper)
│   ├── Login/                   # Login module
│   │   ├── Auth/                # Auth views (password / OIDC)
│   │   ├── OperatorWindow.*     # Operator login window
│   │   └── TerminalWindow.*     # Terminal binding window
│   ├── Utils/                   # Logging and other utilities
│   ├── Views/                   # Business views
│   │   ├── About/               # About page
│   │   ├── Account/             # Account page
│   │   ├── Browser/             # Embedded browser
│   │   ├── Config/              # Settings
│   │   ├── Demo/                # Demo pages
│   │   ├── Home/                # Dashboard home
│   │   ├── Tasks/               # Scheduled tasks
│   │   └── Uc/                  # Shared view components
│   ├── App.xaml                 # Application entry (singleton Mutex + startup)
│   ├── MainWindow.xaml          # Main window
│   ├── SplashWindow.xaml        # Splash bootstrap (logging, config, DB init)
│   ├── ScmClientEnv.cs          # Version and environment info
│   └── appsettings.json         # Application config
├── Scm.Client/                  # Client core library (net11.0)
│   ├── Dto/Operator/            # Operator auth DTOs
│   ├── Dto/Terminal/            # Terminal binding DTOs
│   ├── ScmClient.cs             # Client base (HTTP + Token)
│   ├── ScmOperator.cs           # Operator client
│   ├── ScmTerminal.cs           # Terminal client
│   └── ScmToken.cs              # Token model
├── Scm.Client.Dao/              # Data access layer
│   ├── ScmDao.cs                # DAO base (primary key, ID generation)
│   ├── ScmDataDao.cs            # Data DAO (CRUD + audit fields)
│   ├── ScmVerDao.cs             # Version DAO (auto upgrade)
│   └── SqlHelper.cs             # SqlSugar setup and helpers
├── Scm.Client.Dvo/              # Data value objects layer
│   ├── ScmDvo.cs                # Base class (INotifyPropertyChanged + validation)
│   ├── ScmDataDvo.cs            # Data DVO
│   ├── ScmCommand.cs            # Command implementation (ICommand)
│   └── ScmSearchParamsDvo.cs    # Search parameters DVO
├── Scm.View/                    # UI component library
│   ├── Actions/                 # Action abstractions and samples
│   │   ├── AAction.cs           # Abstract action base
│   │   ├── BrowserAction.cs     # WebView2 browser action
│   │   ├── ViewAction.cs        # WPF view action
│   │   └── Samples/             # NativeAction / RemoteAction
│   ├── Config/                  # Configuration models
│   ├── Controls/                # Custom controls
│   │   ├── Attach/              # Attached behaviors (DataGridColumnsBehavior, IconElement)
│   │   ├── Windows/             # Common windows (browser, message, input, exception)
│   │   ├── PageGrid.xaml        # Paged data grid
│   │   ├── PageGridColumnsView.xaml  # Column visibility/order panel
│   │   ├── PrintPreviewWindow.xaml   # Print preview window
│   │   ├── NavigationDrawer.*   # Navigation drawer
│   │   ├── NavigationMenu.xaml  # Navigation menu
│   │   └── ToastControl.xaml    # Toast notifications
│   ├── Converters/              # Value converters
│   ├── Dvo/                     # Menu DVO
│   ├── Enums/                   # Login type, window state enums
│   ├── Helper/                  # GridExporter / GridPrinter / ScmDbHelper, etc.
│   ├── Models/                  # ScmColumnInfo, PageGridColumnOption, TabModel
│   └── ScmPageGridDvo.cs        # Paged list view-model base (with generic variant)
├── Scm.Samples/                 # Sample project
│   ├── Dao/Native/              # Native sample DAO
│   ├── Dto/Remote/              # Remote sample DTO
│   ├── Helper/                  # Sample database initialization
│   └── Views/
│       ├── Native/              # Local database examples (Main/Search/Info/Edit/Custom)
│       └── Remote/              # Remote API examples (Main/Search/Info/Edit/Custom)
├── Scm.Common.Os/               # OS helper library
├── Scm.Image/                   # Image viewer sample project
├── Scm.Audio/                   # Blank WPF template project (reserved)
├── Scm.Video/                   # Blank WPF template project (reserved)
├── WinForm/                     # WinForms tooling
├── docs/                        # Documentation (PageGrid guide, etc.)
├── Test/                        # Placeholder console project
└── Scm.Wpf.sln                  # Solution file

../Libs/                         # Prebuilt library references (sibling of the repo, checkout separately)
├── net10.0/                     # Scm.Dto / Scm.Uid / Upgrade.Net, etc.
└── netstandard2.0/              # Scm.Common* / Scm.Oidc.Client, etc.
```

---

## 🔧 Prerequisites

| Tool | Version | Download |
| --- | --- | --- |
| .NET SDK | ≥ 11.0 | [https://dotnet.microsoft.com](https://dotnet.microsoft.com) |
| Visual Studio | ≥ 2026 | [https://visualstudio.microsoft.com](https://visualstudio.microsoft.com) |
| Windows | ≥ 10 | — |

---

## Dependencies

When checking out this project, also check out [Libs](https://gitee.com/leadiot/libs) into the `Libs/` folder **one level above the repository root** (projects reference it via the relative path `..\..\Libs\`).

Without `Libs/`, projects such as `Scm.View` and `Scm.Wpf` fail to compile because `Scm.Common*.dll`, `Scm.Uid.dll`, `Upgrade.Net.dll` and other assemblies cannot be resolved.

---

## 🚀 Quick Start

### 1. Clone

```bash
git clone https://gitee.com/leadiot/scm.wpf.git
cd scm.wpf
git clone https://gitee.com/leadiot/libs.git ../Libs
```

### 2. Configure

Edit `Scm.Wpf/appsettings.json`:

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
| `ScmPageGridDvo<T>` | Paged list view-model base, unified pagination; subclasses implement `LoadPageAsync` only |
| `PageGrid` | Paged data grid control, supports search, column settings, edit, export, print ([usage guide](docs/PageGrid.md)) |
| `DataGridColumnsBehavior` | Attached behavior that builds DataGrid columns from `ScmColumnInfo` |
| `GridExporter` | List export service for XLSX / XLS / CSV / JSON / SQL |
| `GridPrinter` | List print service with A4 preview and direct printing |
| `AAction` | Abstract action base class, menu-driven view switching and business dispatch |

### Example: Paged List View-Model

```csharp
public class UserDvo : ScmPageGridDvo<UserItemDvo>
{
    public UserDvo()
    {
        Columns = new List<ScmColumnInfo>
        {
            new ScmColumnInfo { Value = "codec", Label = "Code", Width = "120" },
            new ScmColumnInfo { Value = "namec", Label = "Name", Width = "*" }
        };
    }

    protected override async Task<ScmPageResult<UserItemDvo>> LoadPageAsync()
    {
        // Query the current page using Key / PageIndex / PageItems
        var page = await _dao.GetPageAsync(Key, PageIndex, PageItems);
        return new ScmPageResult<UserItemDvo> { Items = page.Items, Total = page.Total };
    }
}
```

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

## 📚 Documentation

- [PageGrid Guide](docs/PageGrid.md) — quick start, column config, interaction events, export, printing, FAQ

---

## 🔗 Related Links

- [Scm.Net Backend](https://gitee.com/leadiot/scm.net) — .NET 11.0 + Vue 3 enterprise management framework
- [Scm.Vue Frontend](https://gitee.com/leadiot/scm.vue) — Vue 3 + Vite + Element Plus frontend framework
- [Libs Prebuilt Libraries](https://gitee.com/leadiot/libs) — required local dependencies
- [Online Demo](http://www.c-scm.net)
- [SqlSugar Documentation](https://www.donet5.com/)
- [HandyControl Documentation](https://handyorg.github.io/handycontrol/)

---

## 📄 License

This project is licensed under the [MIT License](LICENSE), intended for learning and reference purposes only.
