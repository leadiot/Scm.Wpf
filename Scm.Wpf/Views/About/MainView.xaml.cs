using Com.Scm.Helper;
using Com.Scm.Utils;
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

        private MainViewDvo _Dvo;

        public MainView()
        {
            InitializeComponent();
        }

        public void Init(ScmWindow window)
        {
            _Window = window;

            _Dvo = new MainViewDvo(_Window);
            _Dvo.Init();

            this.DataContext = _Dvo;
        }

        public UserControl GetView()
        {
            return this;
        }

        private void HlWebsite_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                OsHelper.Browse(_Dvo.AppWebsite);
            }
            catch (Exception exception)
            {
                LogUtils.Error(exception);
                _Window.ShowException(exception);
            }
        }

        private void HlProject_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                OsHelper.Browse(_Dvo.AppProject);
            }
            catch (Exception exception)
            {
                LogUtils.Error(exception);
                _Window.ShowException(exception);
            }
        }

        private void HlVersion_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _Dvo.LoadVerInfo();
        }

        private void BtVersion_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _Dvo.LoadVerInfo();
        }

        private void HlEmail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OsHelper.Browse("mailto:" + _Dvo.AppEmail);
            }
            catch (Exception ex)
            {
                LogUtils.Error(ex);
                _Window.ShowException(ex);
            }
        }

        private void HlQq_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Clipboard.SetText(_Dvo.AppContact);
            _Window.ShowToast("QQ群号复制成功！");
        }
    }
}
