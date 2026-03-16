using Com.Scm.Utils;
using Com.Scm.Views.Demo;
using System.Windows.Controls;

namespace Com.Scm.Wpf.Views.Demo
{
    /// <summary>
    /// DemoView.xaml 的交互逻辑
    /// </summary>
    public partial class DemoView : UserControl, ScmView
    {
        private ScmWindow _Window;

        private DemoViewDvo _Dvo;

        public DemoView()
        {
            InitializeComponent();
        }

        public void Init(ScmWindow window)
        {
            _Window = window;

            _Dvo = new DemoViewDvo();
            this.DataContext = _Dvo;
        }

        public UserControl GetView()
        {
            return this;
        }

        private void BtAppend_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _Dvo.Text += TimeUtils.FormatDataTime(DateTime.Now) + Environment.NewLine;
        }

        private void BtNotice_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _Window.ShowNotice("这是一个Notice");
        }

        private void BtToast_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _Window.ShowToast("这是一个Toast");
        }

        private void BtAlert_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _Window.ShowAlert("这是一个Alert");
        }
    }
}
