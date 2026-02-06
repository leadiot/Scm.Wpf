using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Com.Scm.Controls
{
    /// <summary>
    /// ToastControl.xaml 的交互逻辑
    /// </summary>
    public partial class ToastControl : UserControl
    {
        public ToastControl()
        {
            InitializeComponent();
        }

        public void SetBackground(Brush brush)
        {
            BdPanel.Background = brush;
        }

        public void SetMessage(string message)
        {
            TbMessage.Text = message;
        }

        public void SetCloseVisible(bool visible)
        {
            BtClose.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BtClose_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
