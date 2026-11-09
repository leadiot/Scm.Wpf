using Com.Scm.Controls.Attach;
using Com.Scm.Dvo;
using Com.Scm.Helper;
using HandyControl.Controls;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Com.Scm.Controls
{
    /// <summary>
    /// PageGrid.xaml 的交互逻辑。
    /// 数据上下文应设置为 ScmPageGridDvo 派生类实例。
    /// </summary>
    public partial class PageGrid : UserControl
    {
        public PageGrid()
        {
            InitializeComponent();
        }

        private ScmPageGridDvo GridData { get { return DataContext as ScmPageGridDvo; } }

        #region 依赖属性
        /// <summary>
        /// 表头自定义内容（显示在查询栏左侧）
        /// </summary>
        public static readonly DependencyProperty CustomContentProperty = DependencyProperty.Register(
            "CustomContent", typeof(UIElement), typeof(PageGrid), new PropertyMetadata(null, OnCustomContentChanged));

        public UIElement CustomContent
        {
            get { return (UIElement)GetValue(CustomContentProperty); }
            set { SetValue(CustomContentProperty, value); }
        }

        private static void OnCustomContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = (PageGrid)d;
            grid.GdCustom.Visibility = e.NewValue == null ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 高级查询内容（点击"高级"按钮后在抽屉中显示）
        /// </summary>
        public static readonly DependencyProperty SearchContentProperty = DependencyProperty.Register(
            "SearchContent", typeof(FrameworkElement), typeof(PageGrid), new PropertyMetadata(null, OnSearchContentChanged));

        public FrameworkElement SearchContent
        {
            get { return (FrameworkElement)GetValue(SearchContentProperty); }
            set { SetValue(SearchContentProperty, value); }
        }

        private static void OnSearchContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = (PageGrid)d;
            grid.BtAdvanced.Visibility = e.NewValue == null ? Visibility.Collapsed : Visibility.Visible;
        }
        #endregion

        /// <summary>
        /// 查询事件
        /// </summary>
        private void BtSearch_Click(object sender, RoutedEventArgs e)
        {
            GridData?.SearchAsync(0);
        }

        /// <summary>
        /// 高级查询事件
        /// </summary>
        private void ShowSearch_Click(object sender, RoutedEventArgs e)
        {
            if (SearchContent != null)
            {
                ShowSearch(SearchContent);
            }
        }

        /// <summary>
        /// 数据导出事件
        /// </summary>
        private async void BtExport_Click(object sender, RoutedEventArgs e)
        {
            var columns = DataGridColumnsBehavior.GetColumnsSource(DgGrid);
            await GridExporter.ExportAsync(DgGrid.ItemsSource, columns);
        }

        /// <summary>
        /// 选项事件
        /// </summary>
        private void BtOption_Click(object sender, RoutedEventArgs e)
        {
            var view = new PageGridColumnsView();

            var window = new PopupWindow();
            window.PopupElement = view;
            window.Width = 240;
            window.Height = 480;
            window.Show(BtOption, false);
        }

        /// <summary>
        /// 翻页事件
        /// </summary>
        private void Pagination_PageUpdated(object sender, HandyControl.Data.FunctionEventArgs<int> e)
        {
            GridData?.SearchAsync(e.Info);
        }

        #region 抽屉面板
        private SaveDelegate _SaveDelegate;

        /// <summary>
        /// 显示详情
        /// </summary>
        /// <param name="element"></param>
        public void ShowInfo(FrameworkElement element)
        {
            TbTitle.Text = "详情";

            GdDrawer.Children.Clear();
            GdDrawer.Children.Add(element);

            BtAccept.Visibility = Visibility.Collapsed;
            BtDrawerSearch.Visibility = Visibility.Collapsed;

            DrDrawer.IsOpen = true;
        }

        /// <summary>
        /// 显示编辑
        /// </summary>
        /// <param name="view"></param>
        /// <param name="saveDelegate"></param>
        public void ShowEdit(FrameworkElement view, SaveDelegate saveDelegate)
        {
            TbTitle.Text = "编辑";

            _SaveDelegate = saveDelegate;

            GdDrawer.Children.Clear();
            GdDrawer.Children.Add(view);

            BtAccept.Visibility = Visibility.Visible;
            BtDrawerSearch.Visibility = Visibility.Collapsed;

            DrDrawer.IsOpen = true;
        }

        /// <summary>
        /// 显示查询
        /// </summary>
        /// <param name="view"></param>
        public void ShowSearch(FrameworkElement view)
        {
            TbTitle.Text = "查询";

            GdDrawer.Children.Clear();
            GdDrawer.Children.Add(view);

            BtAccept.Visibility = Visibility.Collapsed;
            BtDrawerSearch.Visibility = Visibility.Visible;

            DrDrawer.IsOpen = true;
        }

        /// <summary>
        /// 保存事件
        /// </summary>
        private void BtAccept_Click(object sender, RoutedEventArgs e)
        {
            if (_SaveDelegate == null)
            {
                return;
            }

            if (!_SaveDelegate())
            {
                return;
            }

            DrDrawer.IsOpen = false;
        }

        /// <summary>
        /// 取消事件
        /// </summary>
        private void BtCancel_Click(object sender, RoutedEventArgs e)
        {
            DrDrawer.IsOpen = false;
        }
        #endregion

        #region 选中项
        public object SelectedItem { get { return DgGrid.SelectedItem; } set { DgGrid.SelectedItem = value; } }

        public IList SelectedItems { get { return DgGrid.SelectedItems; } }

        public int SelectedIndex { get { return DgGrid.SelectedIndex; } set { DgGrid.SelectedIndex = value; } }

        public object SelectedValue { get { return DgGrid.SelectedValue; } set { DgGrid.SelectedValue = value; } }

        public IList<DataGridCellInfo> SelectedCells { get { return DgGrid.SelectedCells; } }
        #endregion
    }

    public delegate bool SaveDelegate();
}
