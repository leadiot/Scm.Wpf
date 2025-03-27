using Com.Scm.Wpf.Dvo.Samples;
using Com.Scm.Wpf.Models;
using System.Windows;
using System.Windows.Controls;

namespace Com.Scm.Wpf.Views
{
    /// <summary>
    /// UcSamplesView.xaml 的交互逻辑
    /// </summary>
    public partial class SearchView : UserControl
    {
        private ScmClient _Client;
        private SearchDvo _Dvo;

        public SearchView()
        {
            InitializeComponent();
        }

        public void Init(ScmClient client)
        {
            _Client = client;

            _Dvo = new SearchDvo();
            this.DataContext = _Dvo;

            var columns = new List<ColumnInfo>
            {
                new ColumnInfo { Type=ColumnType.Text, Label = "ID", Value = "Id",Hidden=true },
                new ColumnInfo { Type=ColumnType.CheckBox, Label = "", Value = "IsChecked" },
                new ColumnInfo { Type=ColumnType.Text, Label = "系统编码", Value = "Codec" },
                new ColumnInfo { Type=ColumnType.Text, Label = "系统名称", Value = "Namec", Width="*" }
            };
            PgData.SetColumns(columns);

            Search();
        }

        private void BtAppend_Click(object sender, RoutedEventArgs e)
        {
            DrSide.IsOpen = true;
        }

        private void BtEnable_Click(object sender, RoutedEventArgs e)
        {
            foreach (var dvo in _Dvo.Items)
            {
                if (dvo.IsChecked == true)
                {
                    dvo.row_status = Enums.ScmStatusEnum.Enabled;
                }
            }
        }

        private void BtDisable_Click(object sender, RoutedEventArgs e)
        {
            foreach (var dvo in _Dvo.Items)
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
            Search();
        }

        private async void Search()
        {
            var body = _Dvo.ToDictionary();
            body["page"] = "1";
            body["limit"] = "20";
            //await _Client.GetFormObjectAsync<string>("/api/samplesdemo/page", body);
            var response = await _Client.GetFormObjectAsync<ScmSearchPageResponse<SearchResultDvo>>("/urposition/pages", body);
            _Dvo.Items.Clear();
            _Dvo.Items.AddRange(response.Items);
            PgData.ShowData(response.Items);
        }

        private void BtCancel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
