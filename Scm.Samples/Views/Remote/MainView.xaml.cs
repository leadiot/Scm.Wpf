using System.Windows.Controls;

namespace Com.Scm.Views.Samples.Remote
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : UserControl, ScmView
    {
        private ScmWindow _Owner;
        private SearchParamsDvo _Dvo;

        public MainView()
        {
            InitializeComponent();
        }

        public void Init(ScmWindow owner)
        {
            _Owner = owner;

            _Dvo = new SearchParamsDvo();
            _Dvo.Init();

            PgData.DataContext = _Dvo;

            _Dvo.FirstPageAsync();
        }

        public UserControl GetView()
        {
            return this;
        }
    }
}
