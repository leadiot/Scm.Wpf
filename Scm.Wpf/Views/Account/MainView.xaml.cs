using System.Windows.Controls;

namespace Com.Scm.Views.Account
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : UserControl
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

            _Dvo = new MainViewDvo();
            _Dvo.Init(window);

            this.DataContext = _Dvo;
        }
    }
}
