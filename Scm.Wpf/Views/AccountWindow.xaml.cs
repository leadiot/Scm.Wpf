using System.Windows.Media.Imaging;

namespace Com.Scm.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AccountWindow : HandyControl.Controls.Window
    {
        private ScmWindow _ScmWindow;

        public AccountWindow()
        {
            InitializeComponent();
        }

        public void Init(ScmWindow window)
        {
            if (window == null)
            {
                return;
            }

            this.Owner = window.GetWindow();

            _ScmWindow = window;
            var client = window.GetClient();
            var token = client.GetToken();

            PbIcon.Source = new BitmapImage(new Uri(client.GetAvatar(token.GetAvatar())));
            TbName.Text = "名称：" + token.GetTerminalNames();
            TbCode.Text = "代码：" + token.GetTerminalCodes();
        }
    }
}