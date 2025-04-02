using System.Windows.Controls;

namespace Com.Scm.Wpf.Views.Home
{
    /// <summary>
    /// HomeView.xaml 的交互逻辑
    /// </summary>
    public partial class HomeView : UserControl, ScmView
    {
        public HomeView()
        {
            InitializeComponent();
        }

        public void Init(ScmWindow window)
        {
        }

        public UserControl GetView()
        {
            return this;
        }
    }
}
