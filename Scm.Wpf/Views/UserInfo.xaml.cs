using Com.Scm.Oidc.Response;
using System.Windows.Media.Imaging;

namespace Com.Scm.Oidc.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class UserInfo : HandyControl.Controls.Window
    {
        private OidcUserInfo _User;

        public UserInfo()
        {
            InitializeComponent();
        }

        public void ShowInfo(OidcUserInfo userInfo)
        {
            Show();

            if (userInfo == null)
            {
                return;
            }

            _User = userInfo;

            PbIcon.Source = new BitmapImage(new Uri(userInfo.GetAvatarUrl()));
            TbInfo.Text = "您好：" + userInfo.Name;
        }
    }
}