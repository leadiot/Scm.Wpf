using System.Windows.Controls;

namespace Com.Scm.Views.Samples.Remote
{
    /// <summary>
    /// UcSamplesView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : UserControl, ScmView, ScmPageView
    {
        private ScmWindow _Owner;
        private SearchParamsDvo _Dvo;
        private ScmSearchPageResponse<SearchResultDataDvo> _Response;

        public MainView()
        {
            InitializeComponent();
        }

        public void Init(ScmWindow owner)
        {
            _Owner = owner;

            _Dvo = new SearchParamsDvo();

            PgData.Init(this, _Dvo);

            _Dvo.FirstPageAsync();
        }

        public UserControl GetView()
        {
            return this;
        }

        #region 接口实现

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
