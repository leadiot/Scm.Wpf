using Com.Scm.Controls;
using HandyControl.Controls;
using System.Windows;
using System.Windows.Controls;

namespace Com.Scm.Views.Samples.Native
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : UserControl, ScmView
    {
        private ScmWindow _Owner;
        private MainViewDvo _Dvo;

        public MainView()
        {
            InitializeComponent();
        }

        public void Init(ScmWindow owner)
        {
            _Owner = owner;

            _Dvo = new MainViewDvo();
            _Dvo.Init(owner);

            PgGrid.DataContext = _Dvo;

            _Dvo.FirstPageAsync();
        }

        public UserControl GetView()
        {
            return this;
        }

        /// <summary>
        /// 双击单元格（行已自动选中）
        /// </summary>
        private void PgGrid_CellDoubleClick(object sender, PageGridCellEventArgs e)
        {
            var item = e.Item as SearchResultItemDvo;
            if (item == null)
            {
                return;
            }

            Growl.Info($"双击：{item.namec}");
        }

        /// <summary>
        /// 右键菜单：菜单项的 DataContext 即当前行数据
        /// </summary>
        private void MenuInfo_Click(object sender, RoutedEventArgs e)
        {
            var item = GetMenuItem(sender);
            if (item == null)
            {
                return;
            }

            Growl.Info($"查看：{item.namec}");
        }

        private void MenuCopy_Click(object sender, RoutedEventArgs e)
        {
            var item = GetMenuItem(sender);
            if (item == null)
            {
                return;
            }

            Clipboard.SetText(item.codec ?? string.Empty);
            Growl.Success("已复制编码");
        }

        private void MenuRefresh_Click(object sender, RoutedEventArgs e)
        {
            _Dvo.ReloadAsync();
        }

        private static SearchResultItemDvo GetMenuItem(object sender)
        {
            var menuItem = sender as MenuItem;
            return menuItem == null ? null : menuItem.DataContext as SearchResultItemDvo;
        }
    }
}
