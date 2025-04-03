using Com.Scm.Oidc;
using Com.Scm.Oidc.Response;
using Com.Scm.Utils;
using Com.Scm.Wpf;
using Com.Scm.Wpf.Config;
using Com.Scm.Wpf.Dto;
using Com.Scm.Wpf.Dto.Login;
using System.Diagnostics;
using System.Windows;

namespace Com.Scm
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : HandyControl.Controls.Window
    {
        private OidcConfig _Config;
        private OidcClient _Client;

        public Login()
        {
            InitializeComponent();

            Init();
        }

        public async void Init()
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

        public void ShowMain(LoginResult result, List<WpfMenuDto> menus)
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
                var browser = new Browser();
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

        public async void Load(LoginResult result)
        {
            await LoadMenuAsync(result);
        }

        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public async Task<bool> LoadMenuAsync(LoginResult result, string lang = null)
        {
            var url = AppSettings.EnvConfig.GetApiUrl("/operator/authoritymenu");

            var body = new Dictionary<string, string>();
            body["client"] = "20";
            body["lang"] = lang ?? "zh-cn";

            var head = new Dictionary<string, string>();
            head["Accesstoken"] = result.AccessToken;
            head["Appkey"] = "";

            var response = await HttpUtils.GetObjectAsync<ScmListResponse<WpfMenuDto>>(url, body, head);
            if (response == null)
            {
                return false;
            }
            if (response.Code != 200)
            {
                MessageBox.Show(response.GetMessage());
                return false;
            }

            ShowMain(result, response.data);
            return true;
        }
    }
}
