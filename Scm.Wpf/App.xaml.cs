using Com.Scm.Config;
using Com.Scm.Dao;
using Com.Scm.Enums;
using Com.Scm.Login;
using Com.Scm.Uid.Config;
using Com.Scm.Utils;
using Com.Scm.Views;
using Com.Scm.Wpf.Helper;
using System.Windows;

namespace Com.Scm.Wpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var splashWindow = new SplashWindow();
        splashWindow.Show();

        try
        {
            // 读取配置文件
            //var config = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile(file, optional: false, reloadOnChange: true)
            //    .Build();

            //// 从配置文件初始化Serilog
            //Serilog.Log.Logger = new LoggerConfiguration()
            //     .ReadFrom.Configuration(config)
            //     .CreateLogger();
            LogUtils.Setup();

            // 读取系统配置
            AppSettings.Load();

            // 环境变更配置
            ScmClientEnv.Setup();

            // 数据库配置
            CheckDbVer();

            //NasHelper.Setup();

            // 序列器配置
            var uidConfig = new UidConfig();
            uidConfig.Type = UidType.SnowFlake;
            UidUtils.InitConfig(uidConfig);

            // 校验用户登录
            if (AppSettings.Instance.Env.LoginMode == Enums.ScmLoginTypeEnum.User)
            {
                ShowUserWindow(splashWindow, AppSettings.Instance);
                return;
            }

            await CheckBind(splashWindow);
        }
        catch (Exception ex)
        {
            splashWindow.ShowError(ex);
        }
    }

    /// <summary>
    /// 数据库版本校正
    /// </summary>
    private void CheckDbVer()
    {
        //var dbFile = sqlConfig.Text ?? "Data\\scm.db";
        var dbFile = "Data\\scm.db";
        var dbPath = ScmClientEnv.GetDataPath(dbFile);

        var dir = FileUtils.GetDir(dbPath);
        FileUtils.CreateDir(dir);

        SqlHelper.Setup(dbPath);

        var sqlClient = SqlHelper.GetSqlClient();
        var helper = new ScmDbHelper();
        helper.Init(sqlClient, ScmClientEnv.DataDir);
        helper.InitDb();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        SqlHelper.Close();
        base.OnExit(e);
    }

    /// <summary>
    /// 显示用户登录
    /// </summary>
    /// <param name="splashWindow"></param>
    /// <param name="appSettings"></param>
    private void ShowUserWindow(SplashWindow splashWindow, AppSettings appSettings)
    {
        var window = new UserWindow();
        window.Show();
        splashWindow.Close();
        window.Init(appSettings);
    }

    private async Task CheckSignIn(SplashWindow splashWindow)
    {
        ShowUserWindow(splashWindow, AppSettings.Instance);
    }

    /// <summary>
    /// 显示设备绑定
    /// </summary>
    /// <param name="splashWindow"></param>
    /// <param name="appSettings"></param>
    /// <param name="scmTerminal"></param>
    private void ShowBindWindow(SplashWindow splashWindow, AppSettings appSettings, ScmTerminal scmTerminal)
    {
        var window = new BindWindow();
        window.Show();
        splashWindow.Close();
        window.Init(appSettings, scmTerminal);
    }

    /// <summary>
    /// 校验绑定状态
    /// </summary>
    /// <param name="splashWindow"></param>
    /// <returns></returns>
    private async Task CheckBind(SplashWindow splashWindow)
    {
        var scmTerminal = new ScmTerminal();
        if (!scmTerminal.LoadToken())
        {
            ShowBindWindow(splashWindow, AppSettings.Instance, scmTerminal);
            return;
        }

        // 已过期
        if (scmTerminal.IsExpired())
        {
            ShowBindWindow(splashWindow, AppSettings.Instance, scmTerminal);
            return;
        }

        // 临近过期
        //if (scmTerminal.IsExpires())
        //{
        var result = await scmTerminal.RefreshTokenAsync();
        if (!result)
        {
            ShowBindWindow(splashWindow, AppSettings.Instance, scmTerminal);
            return;
        }
        //}

        ShowMainWindow(splashWindow, AppSettings.Instance, scmTerminal);
    }

    /// <summary>
    /// 显示主窗口
    /// </summary>
    /// <param name="splashWindow"></param>
    /// <param name="appSettings"></param>
    /// <param name="scmTerminal"></param>
    private async void ShowMainWindow(SplashWindow splashWindow, AppSettings appSettings, ScmTerminal scmTerminal)
    {
        var window = new MainWindow();
        await window.Init(appSettings, scmTerminal);

        switch (appSettings.WindowState)
        {
            case ScmWindowState.Minimized:
                window.WindowState = WindowState.Minimized;
                break;
            case ScmWindowState.Maximized:
                window.WindowState = WindowState.Maximized;
                break;
            default:
                window.WindowState = WindowState.Normal;
                break;
        }
        window.Show();

        splashWindow.Close();
        if (appSettings.WindowState == ScmWindowState.Hidden)
        {
            window.Hide();
        }
    }
}

