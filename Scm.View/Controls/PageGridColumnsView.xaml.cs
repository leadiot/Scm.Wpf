using Com.Scm.Controls.Attach;
using Com.Scm.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Com.Scm.Controls
{
    /// <summary>
    /// 列设置视图：控制列的显示与顺序
    /// </summary>
    public partial class PageGridColumnsView : UserControl
    {
        private DataGrid _Grid;
        private List<ScmColumnInfo> _Columns;

        /// <summary>
        /// 列设置项
        /// </summary>
        public ObservableCollection<PageGridColumnOption> Options { get; } = new ObservableCollection<PageGridColumnOption>();

        public PageGridColumnsView()
        {
            InitializeComponent();

            DataContext = this;
        }

        /// <summary>
        /// 初始化列设置
        /// </summary>
        /// <param name="grid">目标表格</param>
        /// <param name="columns">列元数据</param>
        public void Init(DataGrid grid, List<ScmColumnInfo> columns)
        {
            _Grid = grid;
            _Columns = columns;

            Options.Clear();
            if (columns != null)
            {
                foreach (var column in columns)
                {
                    Options.Add(new PageGridColumnOption(column));
                }
            }

            RefreshMoveState();
        }

        /// <summary>
        /// 切换列显示
        /// </summary>
        private void BtVisible_Click(object sender, RoutedEventArgs e)
        {
            var option = GetOption(sender);
            if (option == null)
            {
                return;
            }

            var column = FindColumn(option.Info);
            if (column != null)
            {
                column.Visibility = option.Visible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 上移
        /// </summary>
        private void BtUp_Click(object sender, RoutedEventArgs e)
        {
            var option = GetOption(sender);
            if (option == null)
            {
                return;
            }

            var index = Options.IndexOf(option);
            if (index < 1)
            {
                return;
            }

            Move(index, index - 1);
        }

        /// <summary>
        /// 下移
        /// </summary>
        private void BtDown_Click(object sender, RoutedEventArgs e)
        {
            var option = GetOption(sender);
            if (option == null)
            {
                return;
            }

            var index = Options.IndexOf(option);
            if (index < 0 || index >= Options.Count - 1)
            {
                return;
            }

            Move(index, index + 1);
        }

        /// <summary>
        /// 调整列顺序
        /// </summary>
        private void Move(int oldIndex, int newIndex)
        {
            Options.Move(oldIndex, newIndex);

            if (_Columns != null && oldIndex < _Columns.Count && newIndex < _Columns.Count)
            {
                var info = _Columns[oldIndex];
                _Columns.RemoveAt(oldIndex);
                _Columns.Insert(newIndex, info);
            }

            SyncGridColumns();
            RefreshMoveState();
        }

        /// <summary>
        /// 将表格列顺序同步为列设置中的顺序
        /// </summary>
        private void SyncGridColumns()
        {
            if (_Grid == null || _Columns == null)
            {
                return;
            }

            var target = 0;
            foreach (var info in _Columns)
            {
                var column = FindColumn(info);
                if (column == null)
                {
                    continue;
                }

                var current = _Grid.Columns.IndexOf(column);
                if (current != target && target <= _Grid.Columns.Count - 1)
                {
                    _Grid.Columns.RemoveAt(current);
                    _Grid.Columns.Insert(target, column);
                }

                target++;
            }
        }

        /// <summary>
        /// 查找列元数据对应的表格列
        /// </summary>
        private DataGridColumn FindColumn(ScmColumnInfo info)
        {
            if (_Grid == null)
            {
                return null;
            }

            foreach (var column in _Grid.Columns)
            {
                if (DataGridColumnsBehavior.GetColumnInfo(column) == info)
                {
                    return column;
                }
            }

            return null;
        }

        /// <summary>
        /// 刷新区间按钮状态
        /// </summary>
        private void RefreshMoveState()
        {
            for (var i = 0; i < Options.Count; i++)
            {
                Options[i].CanMoveUp = i > 0;
                Options[i].CanMoveDown = i < Options.Count - 1;
            }
        }

        private static PageGridColumnOption GetOption(object sender)
        {
            var checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                return checkBox.DataContext as PageGridColumnOption;
            }

            var button = sender as Button;
            return button == null ? null : button.DataContext as PageGridColumnOption;
        }
    }
}
