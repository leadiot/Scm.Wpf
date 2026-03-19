using Com.Scm.Config;
using Com.Scm.Enums;
using Com.Scm.Oidc.Response;
using Com.Scm.Views;
using System.Diagnostics;
using System.Windows;

namespace Com.Scm.Login
{
    /// <summary>
    /// 用户登录窗口的交互逻辑
    /// </summary>
    public partial class OperatorWindow : HandyControl.Controls.Window
    {
        public OperatorWindow()
        {
            InitializeComponent();
        }

        public async void Init(AppSettings appSettings, ScmOperator scmOperator)
        {
            UcPass.Init(this, scmOperator);
            UcOidc.Init(this, scmOperator);
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

        public async Task ShowMain(ScmClient client)
        {
            var menuList = await client.LoadMenuAsync(ScmClientTypeEnum.Windows);
            if (menuList == null)
            {
                MessageWindow.ShowDialog(this, client.ErrorMessage);
                return;
            }

            new MainWindow().Init(client, menuList);
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
    }
}
