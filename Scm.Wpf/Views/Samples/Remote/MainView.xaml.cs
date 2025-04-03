using Com.Scm.Wpf.Models;
using System.Windows;
using System.Windows.Controls;

namespace Com.Scm.Wpf.Views.Samples.Remote
{
    /// <summary>
    /// UcSamplesView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : UserControl, ScmView, ISearchView
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

            var columns = new List<ColumnInfo>
            {
                new ColumnInfo { Type=ColumnType.Text, Label = "ID", Value = "Id",Hidden=true },
                new ColumnInfo { Type=ColumnType.CheckBox, Label = "", Value = "IsChecked", Width="70" },
                new ColumnInfo { Type=ColumnType.Text, Label = "系统编码", Value = "Codec" },
                new ColumnInfo { Type=ColumnType.Text, Label = "系统名称", Value = "Namec", Width="*", MinWidth="100" }
            };
            PgData.Init(this, columns);

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
                if (dvo.IsChecked == true)
                {
                    dvo.row_status = Enums.ScmStatusEnum.Enabled;
                }
            }
        }

        private void BtDisable_Click(object sender, RoutedEventArgs e)
        {
            foreach (var dvo in _Response.Items)
            {
                if (dvo.IsChecked == true)
                {
                    dvo.row_status = Enums.ScmStatusEnum.Disabled;
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
        public void FirstPageAsync()
        {
            _Dvo.Page = 1;

            ReloadPageAsync();
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

            ReloadPageAsync();
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

            ReloadPageAsync();
        }

        public void EndPageAsync()
        {
            _Dvo.Page = (int)_Response.TotalPages;

            ReloadPageAsync();
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
            _Dvo.Page = page;

            ReloadPageAsync();
        }

        public async void ReloadPageAsync()
        {
            var body = _Dvo.ToDictionary();
            _Response = await _Owner.GetObjectAsync<ScmSearchPageResponse<SearchResultDataDvo>>("/urposition/pages", body);
            //if (!_Response.Success)
            //{
            //    return;
            //}

            PgData.ShowData(_Response);
        }
        #endregion
    }
}
