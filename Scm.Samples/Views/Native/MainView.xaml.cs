using System.Windows.Controls;

namespace Com.Scm.Wpf.Views.Samples.Native
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : UserControl, ScmPageView
    {
        private ScmWindow _Owner;
        private MainViewDvo _Dvo;
        private ScmSearchPageResponse<SearchResultItemDvo> _Response;

        private SearchView _SearchView;

        private InfoView _ViewControl;
        private InfoViewDvo _ViewDvo;

        public MainView()
        {
            InitializeComponent();
        }

        #region 接口实现

        public void Init(ScmWindow owner)
        {
            _Owner = owner;

            _Dvo = new MainViewDvo();
            _Dvo.Init(owner);

            PgGrid.Init(this, _Dvo);

            _Dvo.FirstPageAsync();
        }

        public UserControl GetView()
        {
            return this;
        }


        public UserControl GetCustomView()
        {
            return null;
        }

        public UserControl GetSearchView()
        {
            return null;
        }

        public UserControl GetInfoView()
        {
            return null;
        }

        public UserControl GetEditView()
        {
            return null;
        }
        #endregion
    }
}
