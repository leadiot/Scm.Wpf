using Com.Scm.Api;
using Com.Scm.Config;
using Com.Scm.Oidc;
using Com.Scm.Utils;
using Com.Scm.Wpf.Dto.Login;
using Com.Scm.Wpf.Dvo.Login;
using System.Windows;
using System.Windows.Controls;

namespace Com.Scm.Uc
{
    /// <summary>
    /// 验证码登录
    /// </summary>
    public partial class UcPassView : UserControl
    {
        /// <summary>
        /// 父窗体
        /// </summary>
        private LoginWindow _Owner;
        /// <summary>
        /// OIDC客户端
        /// </summary>
        private OidcClient _Client;

        private PassDvo _Dvo;

        public UcPassView()
        {
            InitializeComponent();
        }

        public void Init(LoginWindow owner, OidcClient client)
        {
            _Owner = owner;
            _Client = client;

            _Dvo = new PassDvo();
            this.DataContext = _Dvo;
        }

        private void BtVcode_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton != System.Windows.Input.MouseButton.Left)
            {
                return;
            }

            _Dvo.ChangeVCode();
        }

        private void BtVerify_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DoLoginAsync();
        }

        private async void DoLoginAsync()
        {
            _Dvo.Pass = TbPass.Password;

            var body = _Dvo.GetLogin();
            if (body == null)
            {
                return;
            }

            var url = AppSettings.Instance.EnvConfig.GetApiUrl("/operator/SignIn");

            var response = await HttpUtils.PostJsonObjectAsync<ScmApiDataResponse<LoginResult>>(url, body.ToJsonString());
            if (response == null)
            {
                return;
            }
            if (response.Code != 200)
            {
                MessageBox.Show(response.GetMessage());
                return;
            }

            var data = response.Data;
            if (!data.IsSuccess())
            {
                MessageBox.Show(data.GetMessage());
                return;
            }
            _Owner.Load(data);
        }
    }
}
