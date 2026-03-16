using Com.Scm.Config;
using Com.Scm.Dao;
using Com.Scm.Enums;
using Com.Scm.Login;
using Com.Scm.Uid.Config;
using Com.Scm.Utils;
using Com.Scm.Wpf;
using Com.Scm.Wpf.Enums;
using Com.Scm.Wpf.Helper;
using System.Windows;

namespace Com.Scm
{
    /// <summary>
    /// 启动窗口交互逻辑
    /// </summary>
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            InitializeComponent();
        }

        public bool IsIndeterminate
        {
            get
            {
                return progressBar.IsIndeterminate;
            }
            set
            {
                progressBar.IsIndeterminate = value;
            }
        }

        /// <summary>
        /// 对外提供进度更新方法（需在 UI 线程调用）
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="status"></param>
        public void UpdateProgress(int progress, string status)
        {
            Dispatcher.Invoke(() => // 确保 UI 线程更新
            {
                progressBar.Value = progress;
                statusText.Text = status;
            });
        }

        /// <summary>
        /// 显示异常
        /// </summary>
        /// <param name="exp"></param>
        public void ShowError(Exception exp)
        {
            PlLogo.Visibility = Visibility.Collapsed;
            PlInfo.Visibility = Visibility.Visible;

            TbInfo.Text = exp.Message + Environment.NewLine + exp.StackTrace;
        }

        /// <summary>
        /// 退出事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(1);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
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

                // 1. 日志初始化
                LogUtils.Setup();

                // 2. 读取系统配置
                AppSettings.Load();

                // 3. 环境变更配置
                ScmClientEnv.Setup();

                // 4. 数据库配置
                CheckDbVer();

                //NasHelper.Setup();

                // 序列器配置
                var uidConfig = new UidConfig();
                uidConfig.Type = UidType.SnowFlake;
                UidUtils.InitConfig(uidConfig);

                // 校验用户登录
                if (AppSettings.Instance.Env.LoginMode == ScmLoginTypeEnum.User)
                {
                    ShowUserWindow(AppSettings.Instance);
                    return;
                }

                CheckBind();
            }
            catch (Exception ex)
            {
                ShowError(ex);
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

        /// <summary>
        /// 显示用户登录
        /// </summary>
        /// <param name="splashWindow"></param>
        /// <param name="appSettings"></param>
        private void ShowUserWindow(AppSettings appSettings)
        {
            var window = new AuthWindow();
            window.Show();
            Close();

            window.Init(appSettings);
        }

        private async Task CheckUser()
        {
            ShowUserWindow(AppSettings.Instance);
        }

        /// <summary>
        /// 显示设备绑定
        /// </summary>
        /// <param name="splashWindow"></param>
        /// <param name="appSettings"></param>
        /// <param name="scmTerminal"></param>
        private void ShowBindWindow(AppSettings appSettings, ScmTerminal scmTerminal)
        {
            var window = new BindWindow();
            window.Show();
            Close();

            window.Init(appSettings, scmTerminal);
        }

        /// <summary>
        /// 校验绑定状态
        /// </summary>
        /// <param name="splashWindow"></param>
        /// <returns></returns>
        private async void CheckBind()
        {
            var scmTerminal = new ScmTerminal();
            scmTerminal.DataDir = ScmClientEnv.DataDir;
            if (!scmTerminal.LoadToken())
            {
                ShowBindWindow(AppSettings.Instance, scmTerminal);
                return;
            }

            // 已过期
            if (scmTerminal.IsExpired())
            {
                ShowBindWindow(AppSettings.Instance, scmTerminal);
                return;
            }

            // 临近过期
            //if (scmTerminal.IsExpires())
            //{
            var result = await scmTerminal.RefreshTokenAsync();
            if (!result)
            {
                ShowBindWindow(AppSettings.Instance, scmTerminal);
                return;
            }
            //}

            ShowMainWindow(AppSettings.Instance, scmTerminal);
        }

        /// <summary>
        /// 显示主窗口
        /// </summary>
        /// <param name="splashWindow"></param>
        /// <param name="appSettings"></param>
        /// <param name="scmTerminal"></param>
        private async void ShowMainWindow(AppSettings appSettings, ScmTerminal scmTerminal)
        {
            var window = new MainWindow();
            window.Init(scmTerminal, null);

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

            Close();
            if (appSettings.WindowState == ScmWindowState.Hidden)
            {
                window.Hide();
            }
        }
    }
}
