using HandyControl.Tools.Extension;
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
        private ContextMenu _Menu;

        public UcInfobarView()
        {
            InitializeComponent();
        }

        public void Init(ScmWindow window)
        {
            _ScmWindow = window;
            _Menu = FindResource("CmMenu") as ContextMenu;
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

        private void MiInfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MiExit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtAvatar_Click(object sender, RoutedEventArgs e)
        {
            _Menu.PlacementTarget = BtAvatar;
            _Menu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            _Menu.HorizontalOffset = -150;
            _Menu.IsOpen = true;
            _Menu.Show();
        }
    }
}
