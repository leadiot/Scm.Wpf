# SCM WPF Client Framework

Simplified Chinese | [English](./README-en.md)

## Project Overview

SCM WPF is a client application framework based on WPF (Windows Presentation Foundation), providing comprehensive features including authentication, data access, UI components, and automatic update functionality. The framework adopts a modular design and supports both local database and remote API data operations.

## Project Structure

```
scm.wpf/
├── Libs/                          # Third-party dependencies
│   ├── net10.0/                   # .NET 10.0 dependencies
│   └── netstandard2.0/            # .NET Standard 2.0 dependencies
├── Scm.Client/                    # Core client library
│   ├── Dto/                       # Data Transfer Objects
│   │   ├── Auth/                  # Authentication-related DTOs
│   │   └── Bind/                  # Terminal binding-related DTOs
│   ├── ScmClient.cs               # Client base class
│   ├── ScmOperator.cs             # Operator client
│   └── ScmTerminal.cs             # Terminal client
├── Scm.Client.Dao/                # Data Access Layer
│   ├── ScmDao.cs                  # Base DAO class
│   ├── ScmDataDao.cs              # Data DAO
│   ├── ScmVerDao.cs               # Version information DAO
│   └── SqlHelper.cs               # SQL helper class
├── Scm.Client.Dvo/                # Data Value Objects
│   ├── ScmDvo.cs                  # Base DVO (implements INotifyPropertyChanged)
│   ├── ScmDataDvo.cs              # Data DVO
│   ├── ScmCommand.cs              # Command implementation
│   └── ScmSearchParamsDvo.cs      # Search parameters DVO
├── Scm.View/                      # UI Component Library
│   ├── Controls/                  # Custom controls
│   │   ├── PageGrid.xaml         # Paginated data grid
│   │   └── NavigationDrawer.xaml # Navigation drawer
│   ├── Converters/               # Value converters
│   ├── Models/                   # Data models
│   └── Views/                    # Common views
├── Scm.Samples/                   # Sample projects
│   ├── Views/Native/             # Local database examples
│   └── Views/Remote/             # Remote API examples
├── Scm.Upgrade/                   # Automatic update module
└── Scm.Wpf/                       # Main application
    ├── Actions/                   # Menu actions
    ├── Config/                    # Configuration management
    ├── Controls/                  # WPF controls
    ├── Login/                     # Login module
    │   ├── Auth/                  # Authentication login
    │   ├── OperatorWindow.xaml   # Operator login window
    │   └── TerminalWindow.xaml    # Terminal binding window
    ├── Views/                     # Business views
    │   ├── About/                 # About page
    │   ├── Demo/                  # Demo page
    │   ├── Home/                  # Home page
    │   └── Tasks/                 # Tasks page
    └── MainWindow.xaml            # Main window
```

## Features

### 1. Authentication System
- **Operator Login**: Username and password authentication
- **OIDC/OAuth2 Authentication**: Support for third-party OAuth login
- **Terminal Binding**: Device binding authentication mechanism
- **Token Management**: Automatic token refresh and expiration handling

### 2. Data Access
- **Local Database**: Uses SqlSugar ORM to operate SQLite
- **Remote API**: Full support for RESTful API calls
- **Data Validation**: Implements INotifyDataErrorInfo for field validation

### 3. UI Components
- **PageGrid**: Powerful paginated data grid component
  - Supports column customization
  - Data export (CSV/JSON/SQL/TXT/XLS)
  - Pagination navigation
- **NavigationDrawer**: Side navigation drawer
- **Toast Notifications**: Success/error/warning alerts
- **Multiple Menu Modes**: Sidebar / Top menu

### 4. Automatic Update
- Version detection and comparison
- Incremental update support
- Download progress display
- Automatic installation and restart

## Quick Start

### Prerequisites
- .NET 10.0 or higher
- Windows 10/11

### Configuration

Configure application settings in `appsettings.json`:

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

### Running the Project

1. Clone the repository
```bash
git clone https://gitee.com/leadiot/scm.wpf.git
```

2. Open `Scm.Wpf.sln` in Visual Studio

3. Set the startup project to `Scm.Wpf`

4. Press F5 to run

## Sample Code

### Local Database Example

```csharp
// Create search parameters
var searchParams = new SearchParamsDvo
{
    Key = "keyword",
    Status = ScmRowStatusEnum.Enabled,
    Page = 1,
    Limit = 20
};

// Execute search
await SearchAsync(searchParams.Page);
```

### Remote API Example

```csharp
// Use client to call API
var result = await _client.GetObjectAsync<ScmSearchPageResponse<SearchResultDataDvo>>(
    "/api/demo/list", 
    new Dictionary<string, string>
    {
        { "key", searchParams.Key },
        { "status", ((int)searchParams.Status).ToString() }
    });
```

### Custom DVO

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

## Core Classes Overview

| Class | Description |
|-------|-------------|
| `ScmClient` | Client base class handling HTTP requests and token management |
| `ScmOperator` | Operator client for user authentication login |
| `ScmTerminal` | Terminal client for device binding |
| `ScmDvo` | Base data value object implementing property change notification and data validation |
| `PageGrid` | Paginated data grid control supporting CRUD operations |
| `ScmDbHelper` | Database helper class managing database initialization and version upgrades |

## Technology Stack

- **.NET 10.0** - Runtime framework
- **WPF** - UI framework
- **SqlSugar** - ORM framework
- **NLog** - Logging framework
- **HandyControl** - WPF control library

## License

This project is for learning and communication purposes only.

## Related Documentation

- [SqlSugar Documentation](https://www.donet5.com/)
- [HandyControl Documentation](https://handyorg.github.io/handycontrol/)
- [WPF Official Documentation](https://docs.microsoft.com/zh-cn/dotnet/wpf/)