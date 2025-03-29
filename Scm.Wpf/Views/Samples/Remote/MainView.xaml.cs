using Com.Scm.Wpf.Dvo.Samples;
using Com.Scm.Wpf.Models;
using System.Windows;
using System.Windows.Controls;

namespace Com.Scm.Wpf.Views.Samples.Remote
{
    /// <summary>
    /// UcSamplesView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : UserControl
    {
        private ScmClient _Client;
        private SearchParamsDvo _Dvo;

        public MainView()
        {
            InitializeComponent();
        }

        public void Init(ScmClient client)
        {
            _Client = client;

            _Dvo = new SearchParamsDvo();
            this.DataContext = _Dvo;

            var columns = new List<ColumnInfo>
            {
                new ColumnInfo { Type=ColumnType.Text, Label = "ID", Value = "Id",Hidden=true },
                new ColumnInfo { Type=ColumnType.CheckBox, Label = "", Value = "IsChecked", Width="70" },
                new ColumnInfo { Type=ColumnType.Text, Label = "系统编码", Value = "Codec" },
                new ColumnInfo { Type=ColumnType.Text, Label = "系统名称", Value = "Namec", Width="*", MinWidth="100" }
            };
            //PgData.Init(columns);

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
            _Dvo.FirstPage();
            var body = _Dvo.ToDictionary();
            var response = await _Client.GetFormObjectAsync<ScmSearchPageResponse<SearchResultDataDvo>>("/urposition/pages", body);
            //_Dvo.Items.Clear();
            //_Dvo.Items.AddRange(response.Items);
            PgData.ShowData(response.Items);
        }

        private void BtCancel_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
