using Com.Scm.Config;
using Com.Scm.Enums;
using Com.Scm.Oidc;
using Com.Scm.Oidc.Response;
using Com.Scm.Sys.Menu;
using Com.Scm.Utils;
using Com.Scm.Views;
using Com.Scm.Wpf;
using System.Diagnostics;
using System.Windows;

namespace Com.Scm.Login
{
    /// <summary>
    /// 用户登录窗口的交互逻辑
    /// </summary>
    public partial class AuthWindow : HandyControl.Controls.Window
    {
        private OidcConfig _Config;
        private OidcClient _Client;

        public AuthWindow()
        {
            InitializeComponent();
        }

        public async void Init(AppSettings appSettings)
        {
            LogUtils.Setup();

            AppSettings.Load();

            _Config = new OidcConfig();
            // 使用测试应用
            _Config.UseTest();
            // 此处也可以修改为您自己的应用
            //_Config.AppKey = "YOUR_APP_KEY";
            //_Config.AppSecret = "YOUR_APP_SECRET";
            _Config.Prepare();

            _Client = new OidcClient(_Config);

            UcPass.Init(this, _Client);
            await UcOidc.Init(this, _Client);
        }

        /// <summary>
        /// 访问网站事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HlSite_Click(object sender, RoutedEventArgs e)
        {
            Browse("http://www.oidc.org.cn");
        }

        /// <summary>
        /// 显示验证登录
        /// </summary>
        public void ShowVCode()
        {
            //UcVCode.Visibility = Visibility.Visible;
            //UcOAuth.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 显示授权登录
        /// </summary>
        public void ShowOAuth(OidcOspInfo ospInfo)
        {
            //UcVCode.Visibility = Visibility.Collapsed;
            //UcOAuth.Visibility = Visibility.Visible;

            //UcOAuth.Login(ospInfo);
        }

        public void ShowMain(ScmClient client, List<MenuDto> menus)
        {
            new MainWindow().Init(client, menus);
            Close();
        }

        /// <summary>
        /// 使用系统默认浏览器，访问指定地址
        /// </summary>
        /// <param name="url"></param>
        public void Browse(string url, bool native = true)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }

            if (!native)
            {
                var browser = new BrowserWindow();
                browser.Owner = this;
                browser.Open(url);
                return;
            }

            try
            {
                Process.Start("explorer.exe", '"' + url + '"');
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public async Task<bool> LoadMenuAsync(ScmClient client, string lang = null)
        {
            var menuList = await client.LoadMenuAsync(ScmClientTypeEnum.Windows, lang);
            if (menuList == null)
            {
                MessageWindow.ShowDialog(this, client.ErrorMessage);
                return false;
            }

            ShowMain(client, menuList);
            return true;
        }
    }
}
