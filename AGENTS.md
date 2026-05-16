# AGENTS.md — Scm.Wpf

Compact guide for AI agents working in this repo. Every line answers: "Would an agent likely miss this without help?"

## Project Type & Stack

- **.NET 10 WPF** desktop app (Windows only). Main EXE: `Scm.Wpf`.
- **UI library**: [HandyControl](https://handyorg.github.io/handycontrol/) 3.5.1 + MahApps.Metro.IconPacks.Material 6.2.1.
- **ORM**: SqlSugarCore 5.1.4 with **SQLite**.
- **Logging**: NLog 6.1.1 (`NLog.config` auto-reloads).
- **JSON**: Newtonsoft.Json 13.0.4.
- **Embedded browser**: WebView2.

## Build & Run

```bash
cd Scm.Wpf
dotnet run
```

Or open `Scm.Wpf.sln` in Visual Studio, set `Scm.Wpf` as startup project, press F5.

**No build scripts, no CI/CD, no centralized package management.** Packages are inline `<PackageReference>` in each `.csproj`.

## Critical: Missing `Libs/` Folder

The repo **does not include** the `Libs/` directory, yet multiple projects reference precompiled DLLs from it:

- `Libs/netstandard2.0/Scm.Common.dll`
- `Libs/netstandard2.0/Scm.Common.Dto.dll`
- `Libs/netstandard2.0/Scm.Common.File.dll`
- `Libs/netstandard2.0/Scm.Common.Http.dll`
- `Libs/netstandard2.0/Scm.Common.Log.dll`
- `Libs/netstandard2.0/Scm.Common.Text.dll`
- `Libs/netstandard2.0/Scm.Common.Time.dll`
- `Libs/netstandard2.0/Scm.Oidc.Client.dll`
- `Libs/net10.0/Scm.Dto.dll`
- `Libs/net10.0/Scm.Uid.dll`

**Without these DLLs the solution will not build.** Obtain them from the upstream [Scm.Net](https://gitee.com/leadiot/scm.net) project or an existing build environment.

## Solution Structure

| Project | TFM | Role |
|---------|-----|------|
| `Scm.Wpf` | `net10.0-windows` | Main app (WinExe). Entry: `App.xaml` / `App.xaml.cs`. |
| `Scm.View` | `net10.0-windows` | UI controls (PageGrid, NavigationDrawer, Toast, actions). |
| `Scm.Samples` | `net10.0-windows` | Example views (native DB + remote API demos). |
| `Scm.Client` | `net10.0` | HTTP client, auth DTOs, `ScmClient` / `ScmOperator` / `ScmTerminal`. |
| `Scm.Client.Dao` | `net10.0` | SqlSugar DAO base (`ScmDao`), `SqlHelper`, CRUD wrappers. |
| `Scm.Client.Dvo` | `net10.0-windows` | MVVM base (`ScmDvo`: `INotifyPropertyChanged` + `INotifyDataErrorInfo`), commands, search params. |
| `Test` | `net10.0` | **Stub console app** (`Program.Main` is empty). Not a real test project. |

## Namespace Convention

- Default root namespace across almost all projects: **`Com.Scm`**.
- Exception: `Scm.Samples` uses **`Com.Scm.Samples`**.
- Do not invent new root namespaces unless explicitly required.

## Code Style

- `.editorconfig` enforces: **spaces**, indent **4** for C#, **2** for XML/JSON/csproj, **CRLF**, UTF-8.
- `<Nullable>disable</Nullable>` in all projects. Do not add nullability annotations.
- `<ImplicitUsings>enable</ImplicitUsings>` in WPF projects.

## Architecture Patterns

### No Dependency Injection Container
Objects are created manually. Configuration is loaded via a singleton:

```csharp
AppSettings.Load();          // reads appsettings.json
AppSettings.Instance.Sql;    // DB config
AppSettings.Instance.Env;    // env config (login mode, upgrade paths)
```

### Single-Instance App
`App.xaml.cs` uses a path-based `Mutex`. If a second instance starts, it activates the existing window via Win32 APIs (`SetForegroundWindow`, `ShowWindow`) and then shuts down.

### Real Bootstrap Is in `SplashWindow`
`App.xaml.cs` only shows `SplashWindow`. The actual startup sequence happens there:
1. `LogUtils.Setup()` — initializes NLog
2. `AppSettings.Load()` — reads `appsettings.json`
3. `ScmClientEnv.Setup()` — paths, ClickOnce detection
4. `SqlHelper.Setup(...)` — SqlSugar + SQLite singleton
5. `UidUtils.InitConfig(...)` — Snowflake ID generator
6. Routes to `OperatorWindow` or `TerminalWindow` based on `LoginMode`

### Menu / Action System
Menus drive view switching through the `AAction` abstract class:

- `ViewAction` — opens a WPF view
- `BrowserAction` — opens a WebView2 page
- `NativeAction` — local DB business logic
- `RemoteAction` — remote API call

New menu-driven features usually require a new `AAction` subclass.

Menu items specify a fully-qualified class name in their `uri` field. `MainWindow.GetAction` instantiates actions via `Assembly.GetEntryAssembly().CreateInstance(className)` — **actions must have a parameterless constructor** and be resolvable from the entry assembly.

### MVVM Base Class
All view-model / data objects inherit from `ScmDvo`:

- Implements `INotifyPropertyChanged` and `INotifyDataErrorInfo`
- Use `SetProperty(ref _field, value)` for change notification
- Override `IsValid()` for per-property validation
- `ToDictionary()` reflects declared public properties

### Data Access
- `SqlHelper.Setup(file)` initializes a singleton `SqlSugarClient` for SQLite.
- `ScmDao` is the entity base: auto-generates IDs via a custom snowflake-like algorithm (`id > 1000` is considered valid).
- `ScmDataDao` extends `ScmDao` with audit fields (`row_status`, `create_time`, `update_time`) and auto-stamps timestamps on create/update.
- Database schema initialization is **imperative** at runtime (no EF migrations). `SplashWindow` calls `SamplesDbHelper.InitDb()` in `Scm.Samples` to create tables.

## Configuration Files

### `appsettings.json` (copied to output)
Key sections:

```json
{
  "Env": { "LoginMode": "Operator|Terminal", "UpgradeFilePath": "...", "UpgradeJsonName": "..." },
  "Server": { "Host": "localhost", "Port": "9999" },
  "Sql": { "Type": "Sqlite", "Text": "Data Source=./Data/scm.db;" },
  "Oidc": { "app_key": "", "app_secret": "", "redirect_uri": "...", "scope": "openid" }
}
```

### `NLog.config` (copied to output)
Logs to `logs/<date>.log`, console, and debugger. `autoReload="true"`.

## Release Checklist (Hard-Coded Versions)

`Scm.Wpf/ScmClientEnv.cs` contains hard-coded version constants with explicit `TODO` comments stating they **must** be updated before every release:

- `VER_DATE` — release date (`yyyy-MM-dd`)
- `VER_CODE` — build code (`yyyyMMddxx`)
- `BUILD`, `PATCH`, `MINOR`, `MAJOR` — semantic version components

An agent building or packaging this app must update these values or the release will ship with stale version metadata.

## Testing

There is **no test framework** configured. `Test/Test.csproj` is an empty console application. Do not assume xUnit/NUnit/MSTest are present.

## UI Styling

- Application-wide resources are in `Scm.Wpf/App.xaml`: merges `HandyControl` skins + `Assets/Styles/Style.xaml`.
- `Style.xaml` defines the design system: colors (`china_blue_*`, `china_neutral_*`), spacing tokens, shadow effects, and shared styles (`ScmCardStyle`, `ScmButtonPrimary`, `ScmTextTitle`, etc.).
- Prefer these shared resources over ad-hoc styling.

## Remote API Behavior

`ScmClient` switches remote host based on build configuration:

- `DEBUG`: `localhost:5000`
- `RELEASE`: `api.c-scm.net`

It also disables TLS certificate validation in certain scenarios (see `ScmClient` HTTP handler setup). Be cautious if modifying security-related code.

## Gotchas

- **Do not delete or rename `Scm.Wpf/Actions/`** — the action dispatcher is wired to menu loading.
- **SqlSugar `IsAutoCloseConnection = true`** is set in `SqlHelper`. Do not change this without understanding the connection-pooling implications.
- **WPF projects require `UseWPF=true`** and will not compile on non-Windows OSs.
- The `Test` project has no references to the WPF projects; it only references `Scm.Common*` DLLs from `Libs/`.
