using Com.Scm.Wpf.Dvo.Samples;
using System.Windows;
using System.Windows.Controls;

namespace Com.Scm.Wpf.Views
{
    /// <summary>
    /// UcSamplesView.xaml 的交互逻辑
    /// </summary>
    public partial class UcSamplesView : UserControl
    {
        private ScmClient _Client;
        private SearchDvo _Dvo;

        public UcSamplesView()
        {
            InitializeComponent();

            _Dvo = new SearchDvo();
            this.DataContext = _Dvo;
        }

        public void Init(ScmClient client)
        {
            _Client = client;
        }

        private void BtAppend_Click(object sender, RoutedEventArgs e)
        {
            DrSide.IsOpen = true;
        }

        private void BtEnable_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtDisable_Click(object sender, RoutedEventArgs e)
        {

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
           var result= await _Client.GetFormStringAsync("/urposition/pages", body);
        }
    }
}
