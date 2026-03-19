using Com.Scm.Views.Samples.Native;
using System.Windows;
using System.Windows.Controls;

namespace Com.Scm.Samples.Views.Remote
{
    /// <summary>
    /// CustomView.xaml 的交互逻辑
    /// </summary>
    public partial class CustomView : UserControl
    {
        private SearchParamsDvo _Dvo;

        public CustomView()
        {
            InitializeComponent();
        }

        public void Init(SearchParamsDvo dvo)
        {
            _Dvo = dvo;
        }

        private void BtAppend_Click(object sender, RoutedEventArgs e)
        {
            //DrSide.IsOpen = true;
        }

        private void BtEnable_Click(object sender, RoutedEventArgs e)
        {
            //foreach (var dvo in _Response.Items)
            //{
            //    if (dvo.IsChecked == true)
            //    {
            //        dvo.row_status = ScmRowStatusEnum.Enabled;
            //    }
            //}
        }

        private void BtDisable_Click(object sender, RoutedEventArgs e)
        {
            //foreach (var dvo in _Response.Items)
            //{
            //    if (dvo.IsChecked == true)
            //    {
            //        dvo.row_status = ScmRowStatusEnum.Disabled;
            //    }
            //}
        }

        private void BtDelete_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
