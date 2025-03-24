using System.Windows;
using System.Windows.Controls;

namespace Com.Scm.Wpf.Views.Uc
{
    /// <summary>
    /// UcDataView.xaml 的交互逻辑
    /// </summary>
    public partial class UcDataView : UserControl
    {
        public UcDataView()
        {
            InitializeComponent();
        }

        private void BtMore_Click(object sender, RoutedEventArgs e)
        {
            PwForm.IsOpen = BtMore.IsChecked.Value;
        }
    }
}
