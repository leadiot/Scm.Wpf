using Com.Scm.Config;
using Com.Scm.Helper;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Com.Scm.Views.Config
{
    public partial class MainView : UserControl, ScmView
    {
        private ScmWindow _Window;

        public MainView()
        {
            InitializeComponent();
        }

        public UserControl GetView()
        {
            return this;
        }

        public void Init(ScmWindow window)
        {
            _Window = window;

            TbVersion.Text = ScmClientEnv.VER_INFO;
            CkAutoStart.IsChecked = AppSettings.Instance.AutoStartup;
        }

        private void CkAutoStart_Click(object sender, RoutedEventArgs e)
        {
            var appName = ScmClientEnv.ProductName;

            if (CkAutoStart.IsChecked == true)
            {
                AppSettings.Instance.AutoStartup = true;
                var appPath = Assembly.GetEntryAssembly()?.Location;
                if (appPath == null)
                {
                    appPath = Process.GetCurrentProcess().MainModule?.FileName;
                }

                bool success = OsHelper.EnableStartup(appName, appPath);
                if (!success)
                {
                    CkAutoStart.IsChecked = false;
                    AppSettings.Instance.AutoStartup = false;
                    MessageBox.Show("开启开机启动失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                AppSettings.Instance.AutoStartup = false;
                OsHelper.DisableStartup(appName);
            }

            AppSettings.Instance.Save();
        }
    }
}
