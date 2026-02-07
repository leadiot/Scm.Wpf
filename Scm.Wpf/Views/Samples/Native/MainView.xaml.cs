using Com.Scm.Utils;
using Com.Scm.Wpf.Dao.Samples;
using Com.Scm.Wpf.Models;
using System.Windows;
using System.Windows.Controls;

namespace Com.Scm.Wpf.Views.Samples.Native
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : UserControl, ScmView, ISearchView
    {
        private ScmWindow _Owner;
        private SearchParamsDvo _Dvo;
        private ScmSearchPageResponse<ListDvo> _Response;

        private EditView _EditView;
        private InfoView _InfoView;
        private SearchView _SearchView;

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
            if (_EditView == null)
            {
                _EditView = new EditView();
            }

            var dvo = new EditDvo();
            dvo.codec = "123";
            _EditView.Init(dvo);

            PgData.ShowEdit(_EditView);
        }

        private void BtEnable_Click(object sender, RoutedEventArgs e)
        {
            foreach (var dvo in _Response.Items)
            {
                if (dvo.Checked == true)
                {
                    dvo.row_status = Enums.ScmRowStatusEnum.Enabled;
                }
            }
        }

        private void BtDisable_Click(object sender, RoutedEventArgs e)
        {
            foreach (var dvo in _Response.Items)
            {
                if (dvo.Checked == true)
                {
                    dvo.row_status = Enums.ScmRowStatusEnum.Disabled;
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
                _SearchView = new SearchView();
            }

            PgData.ShowSearch(_SearchView);
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
            var client = SqlHelper.GetSqlClient();

            var body = _Dvo.ToDictionary();
            var result = await client.Queryable<ScmDemoNativeDao>()
                //.Where(a => a != null)
                .Select<ListDvo>()
                .ToPageListAsync(0, 10);
            _Response = new ScmSearchPageResponse<ListDvo>();
            _Response.Items = result;
            _Response.SetSuccess();

            PgData.ShowData(_Response);
        }
        #endregion
    }
}
