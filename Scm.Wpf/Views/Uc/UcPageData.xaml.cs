using Com.Scm.Wpf.Models;
using System.Collections;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Com.Scm.Wpf.Views.Uc
{
    /// <summary>
    /// UcPageData.xaml 的交互逻辑
    /// </summary>
    public partial class UcPageData : UserControl
    {
        private ScmSearchPageResponse<string> _Response;
        private int _PageIndex;

        public UcPageData()
        {
            InitializeComponent();
        }

        private void GenPageInfo()
        {
            var total = _Response.TotalPages;
            var size = _Response.TotalItems;
        }

        public void ShowData(IEnumerable dataSource, List<ColumnInfo> columns)
        {
            if (dataSource == null)
            {
                return;
            }

            foreach (var column in columns)
            {
                if (column.Type == ColumnType.Text)
                {
                    DgGrid.Columns.Add(CreateTextColumn(column));
                    continue;
                }
                if (column.Type == ColumnType.CheckBox)
                {
                    DgGrid.Columns.Add(CreateCheckboxColumn(column));
                    continue;
                }
            }
        }

        private DataGridTextColumn CreateTextColumn(ColumnInfo info)
        {
            var column = new DataGridTextColumn();
            column.Header = info.Label;
            column.Binding = new Binding(info.Value);
            if (info.Hidden)
            {
                column.Visibility = Visibility.Collapsed;
            }
            if (!string.IsNullOrEmpty(info.Value))
            {
                if ("*" == info.Value)
                {
                    column.Width = new DataGridLength();
                }
            }
            return column;
        }

        private DataGridCheckBoxColumn CreateCheckboxColumn(ColumnInfo info)
        {
            var column = new DataGridCheckBoxColumn();
            column.Header = info.Label;
            column.Binding = new Binding(info.Value);
            if (info.Hidden)
            {
                column.Visibility = Visibility.Collapsed;
            }
            if (!string.IsNullOrEmpty(info.Value))
            {
                if ("*" == info.Value)
                {
                    column.Width = new DataGridLength();
                }
            }
            return column;
        }

        public void ShowData(IEnumerable dataSource)
        {
            if (dataSource == null)
            {
                return;
            }

            // 获取数据源类型
            Type itemType = dataSource.GetType();

            // 清除现有列
            DgGrid.Columns.Clear();

            // 遍历所有公共属性
            foreach (PropertyInfo prop in itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                // 创建列对象
                DataGridTextColumn column = new DataGridTextColumn();

                // 设置列属性
                column.Header = prop.Name;
                column.Binding = new Binding(prop.Name);
                column.Width = new DataGridLength(100); // 默认宽度

                // 自动检测数据类型并设置格式
                //Type propertyType = prop.PropertyType;
                //if (typeof(DateTime).IsAssignableFrom(prop.PropertyType))
                //{
                //    column.CellTemplate = CreateDateTimeCellTemplate();
                //}
                //else if (typeof(double).IsAssignableFrom(prop.PropertyType) ||
                //         typeof(int).IsAssignableFrom(prop.PropertyType))
                //{
                //    column.CellTemplate = CreateNumberCellTemplate();
                //}

                // 添加到DataGrid
                DgGrid.Columns.Add(column);
            }

            // 设置数据源
            DgGrid.ItemsSource = dataSource;
        }

        private static DataTemplate CreateDateTimeCellTemplate()
        {
            DataTemplate template = new DataTemplate();
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(TextBlock));
            factory.SetBinding(TextBlock.TextProperty,
                new Binding("Value") { StringFormat = "{0:yyyy-MM-dd HH:mm:ss}" });
            template.VisualTree = factory;
            return template;
        }

        private static DataTemplate CreateNumberCellTemplate()
        {
            DataTemplate template = new DataTemplate();
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(TextBlock));
            factory.SetBinding(TextBlock.TextProperty,
                new Binding("Value") { StringFormat = "{0:N2}" });
            template.VisualTree = factory;
            return template;
        }
    }
}
