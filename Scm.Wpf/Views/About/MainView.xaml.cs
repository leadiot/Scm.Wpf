using Com.Scm.Config;
using Com.Scm.Utils;
using HandyControl.Controls;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Com.Scm.Views.About
{
    /// <summary>
    /// 关于软件
    /// </summary>
    public partial class MainView : UserControl, ScmView
    {
        private ScmWindow _Window;
        private ScmAppInfo _AppInfo;

        private MainViewDvo _Dvo;

        private bool _Loaded;

        public MainView()
        {
            InitializeComponent();
        }

        public void Init(ScmWindow window)
        {
            _Window = window;

            _Dvo = new MainViewDvo();
        }

        public UserControl GetView()
        {
            return this;
        }

        public void Load()
        {
            if (_Loaded)
            {
                return;
            }

            LoadAppInfo();
            _Loaded = true;
        }

        public void LoadAppInfo()
        {
            try
            {
                HrVersion.Text = ScmClientEnv.GetVersionString();
                HrRelease.Text = ScmClientEnv.RELEASE_DATE;

                _AppInfo = _Window.GetAppInfo("nas.net");
                if (_AppInfo == null)
                {
                    _AppInfo = LoadAppDefault();
                }

                TbRemark.Text = _AppInfo.content;
            }
            catch (Exception e)
            {
                LogUtils.Error(e);
            }
        }

        private ScmAppInfo LoadAppDefault()
        {
            var appInfo = new ScmAppInfo();
            appInfo.code = "Nas.Net";
            appInfo.content = "";
            return appInfo;
        }

        private void HlWebsite_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var url = HlWebsite.NavigateUri.ToString();
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            try
            {
                OsHelper.Browse(url);
            }
            catch (Exception exception)
            {
                LogUtils.Error(exception);
                _Window.ShowException(exception);
            }
        }

        private void HlProject_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var url = HlProject.NavigateUri.ToString();
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            try
            {
                OsHelper.Browse(url);
            }
            catch (Exception exception)
            {
                LogUtils.Error(exception);
                _Window.ShowException(exception);
            }
        }

        private void HlVersion_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadVerInfo();
        }

        private void BtVersion_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadVerInfo();
        }

        public void LoadVerInfo()
        {
            try
            {
                var verInfo = _Window.GetVerInfo("nas.net");
                if (verInfo == null)
                {
                    verInfo = LoadVerDefault();
                }

                if (!ScmVerInfo.IsMatch(ScmClientEnv.GetVersionString(), verInfo.ver))
                {
                    Growl.Info("当前已是最新版本！");
                    //_Owner.ShowToast("当前已是最新版本！", Controls.ToastType.Info);
                    return;
                }

                var result = _Window.ShowDialog($"发现新版本 {verInfo.ver}，是否立即升级？\n\n更新内容：\n{verInfo.remark}", "版本升级");
                if (result != true)
                {
                    return;
                }

                var filePath = Assembly.GetExecutingAssembly().Location;
                var basePath = Path.GetDirectoryName(filePath);
                var upgradeFile = Path.Combine(basePath, AppSettings.Instance.Env.UpgradeFilePath ?? ScmClientEnv.UpgradeFile);
                if (!File.Exists(upgradeFile))
                {
                    _Window.ShowAlert("升级程序不存在，无法完成升级操作！", "系统提示");
                    return;
                }

                var fileName = Path.GetFileName(filePath);
                if (fileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                {
                    fileName = fileName.Substring(0, fileName.Length - 3) + "exe";
                }
                SaveUpgradeJson(basePath, fileName, verInfo);

                OsHelper.OpenFile(upgradeFile);

                //_Dvo.DoExit();
            }
            catch (Exception exception)
            {
                LogUtils.Error(exception);
                _Window.ShowException(exception);
            }
        }

        private void SaveUpgradeJson(string filePath, string fileName, ScmVerInfo verInfo)
        {
            var config = new UpgradeConfig();
            config.AutoStart = true;
            config.AutoClose = true;
            config.InstallPath = filePath;
            config.ExecuteFile = fileName;
            config.AppInfo = _AppInfo;
            config.VerInfo = verInfo;

            var jsonFile = Path.Combine(filePath, AppSettings.Instance.Env.UpgradeJsonName);
            var jsonDir = Path.GetDirectoryName(jsonFile);
            if (!Directory.Exists(jsonDir))
            {
                Directory.CreateDirectory(jsonDir);
            }

            var json = config.ToJsonString();
            File.WriteAllText(jsonFile, json);
        }

        public ScmVerInfo LoadVerDefault()
        {
            var verInfo = new ScmVerInfo();
            verInfo.ver = "1.0.0";
            verInfo.date = "2026-01-01";
            verInfo.build = "2026010101";
            return verInfo;
        }

        private void HlEmail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OsHelper.Browse(HlEmail.NavigateUri.ToString());
            }
            catch (Exception ex)
            {
                LogUtils.Error(ex);
                _Window.ShowException(ex);
            }
        }

        private void HlQq_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Clipboard.SetText("297679499");
            Growl.Info("QQ群号复制成功！");
        }
    }
}
