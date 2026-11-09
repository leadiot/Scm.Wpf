using Com.Scm.Controls.Attach;
using Com.Scm.Dvo;
using Com.Scm.Helper;
using HandyControl.Controls;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

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
        /// 数据打印事件
        /// </summary>
        private void BtPrint_Click(object sender, RoutedEventArgs e)
        {
            var columns = DataGridColumnsBehavior.GetColumnsSource(DgGrid);
            var window = System.Windows.Window.GetWindow(this);
            var title = window == null ? null : window.Title;

            GridPrinter.Preview(DgGrid.ItemsSource, columns, title);
        }

        /// <summary>
        /// 选项事件：调整列的显示与顺序
        /// </summary>
        private void BtOption_Click(object sender, RoutedEventArgs e)
        {
            var columns = GridData == null ? null : GridData.Columns;
            if (columns == null || columns.Count == 0)
            {
                Growl.Warning("当前列表没有可配置的列");
                return;
            }

            var width = 260;
            var view = new PageGridColumnsView();
            view.Init(DgGrid, columns);
            view.Width = width;
            view.MaxHeight = 480;

            var border = new Border
            {
                Background = GetBrush("ScmPanelBackground", Brushes.White),
                BorderBrush = GetBrush("ScmBorderBrush", Brushes.Gray),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(8),
                CornerRadius = new CornerRadius(12),
                Effect = new DropShadowEffect
                {
                    BlurRadius = 16,
                    ShadowDepth = 2,
                    Direction = 270,
                    Opacity = 0.25,
                    Color = Colors.Black
                },
                Child = view
            };

            if (_OptionPopup != null)
            {
                _OptionPopup.IsOpen = false;
            }

            _OptionPopup = new Popup
            {
                PlacementTarget = BtOption,
                Placement = PlacementMode.Top,
                HorizontalOffset = BtOption.ActualWidth - width,
                StaysOpen = false,
                AllowsTransparency = true,
                PopupAnimation = PopupAnimation.Fade,
                Child = border
            };
            _OptionPopup.IsOpen = true;
        }

        private Popup _OptionPopup;

        /// <summary>
        /// 获取主题画刷，取不到时使用默认值
        /// </summary>
        private Brush GetBrush(string key, Brush fallback)
        {
            return TryFindResource(key) as Brush ?? fallback;
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

        #region 双击与右键菜单
        /// <summary>
        /// 单元格双击路由事件
        /// </summary>
        public static readonly RoutedEvent CellDoubleClickEvent = EventManager.RegisterRoutedEvent(
            "CellDoubleClick", RoutingStrategy.Bubble, typeof(EventHandler<PageGridCellEventArgs>), typeof(PageGrid));

        /// <summary>
        /// 双击单元格时触发（触发前会自动选中该行）
        /// </summary>
        public event EventHandler<PageGridCellEventArgs> CellDoubleClick
        {
            add { AddHandler(CellDoubleClickEvent, value); }
            remove { RemoveHandler(CellDoubleClickEvent, value); }
        }

        /// <summary>
        /// 行右键菜单。菜单打开前其 DataContext 会被设置为当前行数据。
        /// </summary>
        public static readonly DependencyProperty RowContextMenuProperty = DependencyProperty.Register(
            "RowContextMenu", typeof(ContextMenu), typeof(PageGrid), new PropertyMetadata(null, OnRowContextMenuChanged));

        public ContextMenu RowContextMenu
        {
            get { return (ContextMenu)GetValue(RowContextMenuProperty); }
            set { SetValue(RowContextMenuProperty, value); }
        }

        private static void OnRowContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = (PageGrid)d;
            grid.DgGrid.ContextMenu = e.NewValue as ContextMenu;
        }

        /// <summary>
        /// 双击单元格
        /// </summary>
        private void DgGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var row = FindParent<DataGridRow>(e.OriginalSource as DependencyObject);
            if (row == null)
            {
                return;
            }

            var cell = FindParent<DataGridCell>(e.OriginalSource as DependencyObject);

            DgGrid.SelectedItem = row.Item;

            RaiseEvent(new PageGridCellEventArgs(CellDoubleClickEvent, this, cell, row));
        }

        /// <summary>
        /// 右键菜单打开前绑定当前行数据，空白处不弹出
        /// </summary>
        private void DgGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var row = FindParent<DataGridRow>(Mouse.DirectlyOver as DependencyObject);
            if (row == null)
            {
                e.Handled = true;
                return;
            }

            DgGrid.SelectedItem = row.Item;

            if (RowContextMenu != null)
            {
                RowContextMenu.DataContext = row.Item;
            }
        }

        /// <summary>
        /// 查找指定类型的父级元素
        /// </summary>
        private static T FindParent<T>(DependencyObject dependency) where T : DependencyObject
        {
            while (dependency != null && !(dependency is T))
            {
                dependency = VisualTreeHelper.GetParent(dependency);
            }
            return dependency as T;
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

    /// <summary>
    /// 单元格事件参数
    /// </summary>
    public class PageGridCellEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// 单元格。双击行号时为 null
        /// </summary>
        public DataGridCell Cell { get; private set; }

        /// <summary>
        /// 行
        /// </summary>
        public DataGridRow Row { get; private set; }

        /// <summary>
        /// 行数据
        /// </summary>
        public object Item { get { return Row == null ? null : Row.Item; } }

        public PageGridCellEventArgs(RoutedEvent routedEvent, object source, DataGridCell cell, DataGridRow row)
            : base(routedEvent, source)
        {
            Cell = cell;
            Row = row;
        }
    }
}
