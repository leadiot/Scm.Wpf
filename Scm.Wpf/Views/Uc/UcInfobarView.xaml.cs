using System.Windows;
using System.Windows.Controls;

namespace Com.Scm.Wpf.Views.Uc
{
    /// <summary>
    /// UcInfobarView.xaml 的交互逻辑
    /// </summary>
    public partial class UcInfobarView : UserControl
    {
        private ScmWindow _ScmWindow;

        public UcInfobarView()
        {
            InitializeComponent();
        }

        public void Init(ScmWindow window)
        {
            _ScmWindow = window;
        }

        private void ToggleMenu_Click(object sender, RoutedEventArgs e)
        {
            if (TbGuid.IsChecked == true)
            {
                _ScmWindow.ShowGuid();
            }
            else
            {
                _ScmWindow.HideGuid();
            }
        }

        private void MinWin_click(object sender, RoutedEventArgs e)
        {

        }

        private void MaxWin_click(object sender, RoutedEventArgs e)
        {

        }

        private void CloseWin_click(object sender, RoutedEventArgs e)
        {

        }

        private void BtHome_Click(object sender, RoutedEventArgs e)
        {
            _ScmWindow.ShowHome();
        }
    }
}
