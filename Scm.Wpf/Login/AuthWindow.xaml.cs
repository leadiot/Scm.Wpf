using Com.Scm.Api;
using Com.Scm.Config;
using Com.Scm.Oidc;
using Com.Scm.Oidc.Response;
using Com.Scm.Sys.Menu;
using Com.Scm.Utils;
using Com.Scm.Wpf;
using Com.Scm.Wpf.Dto.Login;
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

        public void ShowMain(AuthResult result, List<MenuDto> menus)
        {
            new MainWindow().Init(result, menus);
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

        public async void Load(AuthResult result)
        {
            await LoadMenuAsync(result);
        }

        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public async Task<bool> LoadMenuAsync(AuthResult result, string lang = null)
        {
            var url = AppSettings.Instance.Env.GetApiUrl("/operator/authoritymenu");

            var body = new Dictionary<string, string>();
            body["client"] = "20";
            body["lang"] = lang ?? "zh-cn";

            var head = new Dictionary<string, string>();
            head["ApiToken"] = result.AccessToken;
            head["Appkey"] = "";

            var response = await HttpUtils.GetObjectAsync<ScmApiListResponse<MenuDto>>(url, body, head);
            if (response == null)
            {
                return false;
            }
            if (response.Code != 200)
            {
                MessageBox.Show(response.GetMessage());
                return false;
            }

            ShowMain(result, response.Data);
            return true;
        }
    }
}
