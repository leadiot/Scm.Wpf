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

        #region 窗体事件
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

        /// <summary>
        /// 显示首页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtHome_Click(object sender, RoutedEventArgs e)
        {
            _ScmWindow.ShowHome();
        }

        /// <summary>
        /// 头像事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtAvatar_Click(object sender, RoutedEventArgs e)
        {
            _Menu.PlacementTarget = BtAvatar;
            _Menu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            _Menu.HorizontalOffset = -150;
            _Menu.IsOpen = true;
            _Menu.Show();
        }

        /// <summary>
        /// 账户信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiInfo_Click(object sender, RoutedEventArgs e)
        {
            _ScmWindow.ShowAccount();
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiLogout_Click(object sender, RoutedEventArgs e)
        {
            _ScmWindow.Logout();
        }

        /// <summary>
        /// 退出应用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiExit_Click(object sender, RoutedEventArgs e)
        {
            _ScmWindow.Exit();
        }
        #endregion
    }
}
