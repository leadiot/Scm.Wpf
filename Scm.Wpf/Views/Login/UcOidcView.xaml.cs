using Com.Scm.Oidc;
using Com.Scm.Oidc.Response;
using Com.Scm.Response;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Com.Scm.Wpf.Views.Login
{
    /// <summary>
    /// 三方授权登录
    /// </summary>
    public partial class UcOidcView : UserControl
    {
        /// <summary>
        /// 父窗体
        /// </summary>
        private LoginWindow _Owner;
        /// <summary>
        /// OIDC客户端
        /// </summary>
        private OidcClient _Client;
        /// <summary>
        /// 客户端凭据
        /// </summary>
        private TicketInfo _Ticket;
        /// <summary>
        /// 服务商信息
        /// </summary>
        private OidcOspInfo _OspInfo;

        public UcOidcView()
        {
            InitializeComponent();
        }

        public async Task Init(LoginWindow owner, OidcClient client)
        {
            _Owner = owner;
            _Client = client;

            var ospList = await _Client.ListAppOspAsync();
            var idpList = new List<OidcOspInfo>();
            foreach (var osp in ospList)
            {
                //if (!osp.IsOAuth())
                //{
                //    continue;
                //}

                osp.Icon = osp.GetIconUrl();
                //osp.Tips = $"使用 {osp.Name} 登录";
                idpList.Add(osp);
            }
            LbOidc.ItemsSource = idpList;
        }

        public async void Login(OidcOspInfo ospInfo)
        {
            _OspInfo = ospInfo;

            PbLogo.Source = new BitmapImage(new Uri(ospInfo.GetIconUrl()));
            LcLoading.Visibility = System.Windows.Visibility.Visible;

            var response = await _Client.HandshakeAsync("login");
            if (response == null)
            {
                ShowNotice("服务端通讯异常！");
                return;
            }
            if (!response.IsSuccess())
            {
                ShowNotice(response.GetMessage());
                return;
            }

            _Ticket = response.Ticket;
            _Owner.Browse(_Client.GetOAuthUrl(ospInfo, _Ticket.Code));
            Listen();
        }

        #region 侦听
        private void Listen()
        {
            _MaxStep = 60 * 10;// 等待10分钟
            Task.Run(ListenAsync);
        }

        private int _MaxStep = 0;
        private async Task ListenAsync()
        {
            while (_MaxStep > 0)
            {
                _MaxStep -= 1;
                Thread.Sleep(1000);

                var response = await _Client.ListenAsync(_Ticket);
                if (response == null)
                {
                    ShowNotice("服务访问异常！");
                    return;
                }
                if (!response.IsSuccess())
                {
                    ShowNotice(response.GetMessage());
                    return;
                }

                _Ticket.Salt = response.Salt;

                if (response.Handle == ListenHandle.None)
                {
                    //ShowNotice("等待用户授权");
                    continue;
                }

                if (response.Handle == ListenHandle.Todo)
                {
                    ShowNotice("等待用户授权");
                    continue;
                }

                if (response.Handle == ListenHandle.Doing)
                {
                    ShowNotice("用户授权中…");
                    continue;
                }

                if (response.Result == ListenResult.Failure)
                {
                    ShowNotice("用户授权失败");
                    return;
                }

                if (response.Result == ListenResult.Success)
                {
                    ShowNotice("用户授权成功");
                    ShowUserInfo(response);
                    return;
                }
            }

            ShowNotice("授权超时，请返回重试！");
            LcLoading.Visibility = System.Windows.Visibility.Hidden;
        }
        #endregion

        /// <summary>
        /// 获取全局用户信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<OidcUserInfo> GetUnionUserInfoAsync(string token)
        {
            var response = await _Client.GetUserInfoAsync(token);
            if (response == null || !response.IsSuccess())
            {
                return null;
            }

            return response.User;
        }

        private async void ShowUserInfo(ListenResponse response)
        {
            var oauthUser = response.User;
            var unionUser = await GetUnionUserInfoAsync(response.access_token);
            var user = unionUser != null ? unionUser : oauthUser;

            Dispatcher.Invoke(() =>
            {
                //_Owner.ShowUser(user);
            });
        }

        private void ShowNotice(string message)
        {
            Dispatcher.Invoke(() =>
            {
                TbNotice.Text = message;
            });
        }

        private void BtReturn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _MaxStep = 0;
            _Owner.ShowVCode();
        }
    }
}
