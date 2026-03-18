using Com.Scm.Enums;
using Com.Scm.Wpf.Models;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Com.Scm.Wpf.Views.Samples.Remote
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
            this.DataContext = _Dvo;

            PgData.Init(this);

            FirstPageAsync();
        }

        public UserControl GetView()
        {
            return this;
        }

        private void BtAppend_Click(object sender, RoutedEventArgs e)
        {
            DrSide.IsOpen = true;
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

        private void BtCancel_Click(object sender, RoutedEventArgs e)
        {
        }

        #region 接口实现

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
            _Dvo.Page = 1;

            ReloadAsync();
        }

        public void PrevPageAsync()
        {
            var page = _Dvo.Page;
            page -= 1;
            if (page < 1)
            {
                page = 1;
            }
            _Dvo.Page = page;

            ReloadAsync();
        }

        public void NextPageAsync()
        {
            var page = _Dvo.Page;
            page += 1;
            if (page > _Response.TotalPages)
            {
                page = (int)_Response.TotalPages;
            }
            _Dvo.Page = page;

            ReloadAsync();
        }

        public void EndPageAsync()
        {
            _Dvo.Page = (int)_Response.TotalPages;

            ReloadAsync();
        }

        public async void ReloadAsync()
        {
            var body = _Dvo.ToDictionary();
            _Response = await _Owner.GetObjectAsync<ScmSearchPageResponse<SearchResultDataDvo>>("/urposition/pages", body);
            //if (!_Response.Success)
            //{
            //    return;
            //}

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
    }
}
