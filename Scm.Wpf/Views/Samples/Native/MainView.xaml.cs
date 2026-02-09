using Com.Scm.Wpf.Dao.Samples;
using Com.Scm.Wpf.Helper;
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

        public void Init(ScmWindow owner)
        {
            _Owner = owner;

            var columns = new List<ScmColumnInfo>
            {
                new ScmColumnInfo { Type=ScmColumnType.Text, Label = "ID", Value = "Id",Hidden=true },
                new ScmColumnInfo { Type=ScmColumnType.CheckBox, Label = "", Value = "IsChecked", Width="70" },
                new ScmColumnInfo { Type=ScmColumnType.Text, Label = "系统编码", Value = "Codec" },
                new ScmColumnInfo { Type=ScmColumnType.Text, Label = "系统名称", Value = "Namec", Width="*", MinWidth="100" }
            };
            PgData.Init(this, columns);

            _Dvo = new MainViewDvo();
            this.DataContext = _Dvo;

            FirstPageAsync();
        }

        public UserControl GetView()
        {
            return this;
        }

        #region 接口实现
        public void FirstPageAsync()
        {
            _SearchParamsDvo.Page = 1;

            ReloadPageAsync();
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

            ReloadPageAsync();
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

            ReloadPageAsync();
        }

        public void EndPageAsync()
        {
            _SearchParamsDvo.Page = (int)_Response.TotalPages;

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
            _SearchParamsDvo.Page = page;

            ReloadPageAsync();
        }

        public async void ReloadPageAsync()
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
                _SearchView = new SearchControl();
            }

            PgData.ShowSearch(_SearchView, SearchData);
        }

        private bool SearchData()
        {
            return true;
        }
        #endregion
    }
}
