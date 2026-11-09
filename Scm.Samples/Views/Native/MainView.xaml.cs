using System.Windows.Controls;

namespace Com.Scm.Views.Samples.Native
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : UserControl, ScmView
    {
        private ScmWindow _Owner;
        private MainViewDvo _Dvo;

        public MainView()
        {
            InitializeComponent();
        }

        public void Init(ScmWindow owner)
        {
            _Owner = owner;

            _Dvo = new MainViewDvo();
            _Dvo.Init(owner);

            PgGrid.DataContext = _Dvo;

            _Dvo.FirstPageAsync();
        }

        public UserControl GetView()
        {
            return this;
        }
    }
}
