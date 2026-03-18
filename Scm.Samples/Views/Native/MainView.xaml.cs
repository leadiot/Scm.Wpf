using Com.Scm.Enums;
using Com.Scm.Wpf.Dao.Samples;
using Com.Scm.Wpf.Helper;
using Com.Scm.Wpf.Models;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Com.Scm.Wpf.Views.Samples.Native
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : UserControl, ScmView, ScmPageView
    {
        private ScmWindow _Owner;
        private MainViewDvo _Dvo;
        private ScmSearchPageResponse<SearchResultItemDvo> _Response;

        private SearchControl _SearchView;
        private SearchParamsDvo _SearchParamsDvo;

        private EditControl _EditControl;
        private EditControlDvo _EditDvo;

        private ViewControl _ViewControl;
        private ViewControlDvo _ViewDvo;

        public MainView()
        {
            InitializeComponent();
        }

        #region 接口实现

        public void Init(ScmWindow owner)
        {
            _Owner = owner;

            PgData.Init(this);

            _Dvo = new MainViewDvo();
            this.DataContext = _Dvo;

            FirstPageAsync();
        }

        public UserControl GetView()
        {
            return this;
        }

        public List<ScmColumnInfo> GetColumns()
        {
            return null;
        }

        public IEnumerable GetItemsSource()
        {
            return null;
        }

        public void SearchAsync(int pageIndex = 0)
        {
        }

        public void FirstPageAsync()
        {
            _SearchParamsDvo.Page = 1;

            ReloadAsync();
        }

        public void PrevPageAsync()
        {
            var page = _SearchParamsDvo.Page;
            page -= 1;
            if (page < 1)
            {
                page = 1;
            }
            _SearchParamsDvo.Page = page;

            ReloadAsync();
        }

        public void NextPageAsync()
        {
            var page = _SearchParamsDvo.Page;
            page += 1;
            if (page > _Response.TotalPages)
            {
                page = (int)_Response.TotalPages;
            }
            _SearchParamsDvo.Page = page;

            ReloadAsync();
        }

        public void EndPageAsync()
        {
            _SearchParamsDvo.Page = (int)_Response.TotalPages;

            ReloadAsync();
        }

        public void FixedPageAsync(int page)
        {
            if (page > _Response.TotalPages)
            {
                page = (int)_Response.TotalPages;
            }

            if (page < 1)
            {
                page = 1;
            }
            _SearchParamsDvo.Page = page;

            ReloadAsync();
        }

        public async void ReloadAsync()
        {
            var client = SqlHelper.GetSqlClient();

            var body = _Dvo.ToDictionary();
            var result = await client.Queryable<ScmDemoNativeDao>()
                //.Where(a => a != null)
                .Select<SearchResultItemDvo>()
                .ToPageListAsync(0, 10);
            _Response = new ScmSearchPageResponse<SearchResultItemDvo>();
            _Response.Items = result;
            _Response.SetSuccess();

            PgData.ShowData(_Response);
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

        #region 事件处理
        private void BtAppend_Click(object sender, RoutedEventArgs e)
        {
            if (_EditControl == null)
            {
                _EditControl = new EditControl();
            }

            _EditDvo = new EditControlDvo();
            _EditControl.Init(_EditDvo);

            PgData.ShowEdit(_EditControl, SaveData);
        }

        private bool SaveData()
        {
            _EditDvo.ValidateAllProperties();
            if (_EditDvo.HasErrors)
            {
                return false;
            }

            if (!_EditDvo.IsValid())
            {
                return false;
            }

            return true;
        }

        private void BtEnable_Click(object sender, RoutedEventArgs e)
        {
            foreach (var dvo in _Response.Items)
            {
                if (dvo.Checked == true)
                {
                    dvo.row_status = ScmRowStatusEnum.Enabled;
                }
            }
        }

        private void BtDisable_Click(object sender, RoutedEventArgs e)
        {
            foreach (var dvo in _Response.Items)
            {
                if (dvo.Checked == true)
                {
                    dvo.row_status = ScmRowStatusEnum.Disabled;
                }
            }
        }

        private void BtDelete_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtSearch_Click(object sender, RoutedEventArgs e)
        {
            FirstPageAsync();
        }

        private void BtMore_Click(object sender, RoutedEventArgs e)
        {
            if (_SearchView == null)
            {
                _SearchView = new SearchControl();
            }

            PgData.ShowSearch(_SearchView);
        }
        #endregion
    }
}
