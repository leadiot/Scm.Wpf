using Com.Scm.Dvo;
using Com.Scm.Models;
using MahApps.Metro.IconPacks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Com.Scm.Controls.Attach
{
    /// <summary>
    /// 根据 ScmColumnInfo 集合生成 DataGrid 列的附加行为
    /// </summary>
    public class DataGridColumnsBehavior
    {
        public static readonly DependencyProperty ColumnsSourceProperty = DependencyProperty.RegisterAttached(
            "ColumnsSource", typeof(IEnumerable<ScmColumnInfo>), typeof(DataGridColumnsBehavior),
            new PropertyMetadata(null, OnColumnsSourceChanged));

        public static IEnumerable<ScmColumnInfo> GetColumnsSource(DependencyObject obj)
        {
            return (IEnumerable<ScmColumnInfo>)obj.GetValue(ColumnsSourceProperty);
        }

        public static void SetColumnsSource(DependencyObject obj, IEnumerable<ScmColumnInfo> value)
        {
            obj.SetValue(ColumnsSourceProperty, value);
        }

        private static void OnColumnsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid grid)
            {
                GenColumns(grid, e.NewValue as IEnumerable<ScmColumnInfo>);
            }
        }

        /// <summary>
        /// 生成列信息
        /// </summary>
        private static void GenColumns(DataGrid grid, IEnumerable<ScmColumnInfo> columns)
        {
            if (columns == null || !columns.Any())
            {
                grid.AutoGenerateColumns = true;
                return;
            }

            grid.AutoGenerateColumns = false;
            grid.Columns.Clear();
            foreach (var info in columns)
            {
                var column = CreateColumn(grid, info);
                if (column != null)
                {
                    grid.Columns.Add(column);
                }
            }
        }

        private static DataGridColumn CreateColumn(DataGrid grid, ScmColumnInfo info)
        {
            if (info.Type == ScmColumnType.Text)
            {
                return CreateTextColumn(info);
            }
            if (info.Type == ScmColumnType.CheckBox)
            {
                return CreateCheckboxColumn(grid, info);
            }
            if (info.Type == ScmColumnType.Template)
            {
                return CreateTemplateColumn(info);
            }
            if (info.Type == ScmColumnType.Status)
            {
                return CreateStatusColumn(grid, info);
            }
            return null;
        }

        /// <summary>
        /// 列通用属性配置
        /// </summary>
        private static void AdjustColumnInfo(DataGridColumn column, ScmColumnInfo info)
        {
            column.Header = info.Label;

            if (info.Hidden)
            {
                column.Visibility = Visibility.Collapsed;
            }

            var uom = ScmColumnSize.Parse(info.Width);
            if (uom.IsNone)
            {
                column.Width = new DataGridLength(uom.Width, DataGridLengthUnitType.Pixel);
            }
            if (uom.IsFill)
            {
                column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            }
            if (uom.IsAuto)
            {
                column.Width = new DataGridLength(1, DataGridLengthUnitType.Auto);
            }
            if (uom.IsFixed)
            {
                column.Width = new DataGridLength(uom.Width, DataGridLengthUnitType.Pixel);
            }

            uom = ScmColumnSize.Parse(info.MinWidth);
            if (uom.IsFixed)
            {
                column.MinWidth = uom.Width;
            }

            column.CanUserSort = info.Sortable;
        }

        /// <summary>
        /// 创建文本框列
        /// </summary>
        private static DataGridTextColumn CreateTextColumn(ScmColumnInfo info)
        {
            var column = new DataGridTextColumn();
            AdjustColumnInfo(column, info);
            var binding = new Binding(info.Value)
            {
                Mode = info.Mode
            };
            if (info.Converter != null)
            {
                binding.Converter = info.Converter;
            }
            column.Binding = binding;
            column.IsReadOnly = info.ReadOnly;

            Style style = new Style(typeof(TextBlock));
            var align = HorizontalAlignment.Left;
            if (info.Align == ScmColumnAlign.Center)
            {
                align = HorizontalAlignment.Center;
            }
            else if (info.Align == ScmColumnAlign.Right)
            {
                align = HorizontalAlignment.Right;
            }
            else if (info.Align == ScmColumnAlign.Stretch)
            {
                align = HorizontalAlignment.Stretch;
            }
            style.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, align));
            column.ElementStyle = style;

            return column;
        }

        /// <summary>
        /// 创建复选框列
        /// </summary>
        private static DataGridCheckBoxColumn CreateCheckboxColumn(DataGrid grid, ScmColumnInfo info)
        {
            var column = new DataGridCheckBoxColumn();
            AdjustColumnInfo(column, info);
            var checkbox = new CheckBox();
            checkbox.Click += (s, e) => SetChecked(grid, ((CheckBox)s).IsChecked == true);
            column.Header = checkbox;
            column.Binding = new Binding(info.Value)
            {
                Mode = info.Mode
            };
            column.CanUserSort = false;

            return column;
        }

        /// <summary>
        /// 设置所有行的选中状态
        /// </summary>
        private static void SetChecked(DataGrid grid, bool isChecked)
        {
            if (grid.ItemsSource == null)
            {
                return;
            }

            foreach (var item in grid.ItemsSource)
            {
                if (item is ScmSearchResultItemDvo dvo)
                {
                    dvo.IsChecked = isChecked;
                }
            }
        }

        /// <summary>
        /// 创建状态列
        /// </summary>
        private static DataGridTemplateColumn CreateStatusColumn(DataGrid grid, ScmColumnInfo info)
        {
            var column = new DataGridTemplateColumn();
            AdjustColumnInfo(column, info);

            DataTemplate template = new DataTemplate();
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(ToggleButton));
            factory.SetValue(ToggleButton.StyleProperty, grid.TryFindResource("ToggleButtonSwitch"));
            factory.SetBinding(ToggleButton.IsCheckedProperty, new Binding(info.Value)
            {
                Mode = info.Mode
            });
            template.VisualTree = factory;

            column.CellTemplate = template;
            return column;
        }

        /// <summary>
        /// 创建模板列
        /// </summary>
        private static DataGridTemplateColumn CreateTemplateColumn(ScmColumnInfo info)
        {
            var column = new DataGridTemplateColumn();
            AdjustColumnInfo(column, info);

            if (info.Format == ScmColumnFormat.Icon)
            {
                column.CellTemplate = CreateIconCellTemplate(info);
            }
            else if (info.Format == ScmColumnFormat.Number)
            {
                column.CellTemplate = CreateNumberCellTemplate(info);
            }
            else if (info.Format == ScmColumnFormat.DateTime)
            {
                column.CellTemplate = CreateDateTimeCellTemplate(info);
            }
            return column;
        }

        /// <summary>
        /// 创建日期列
        /// </summary>
        private static DataTemplate CreateDateTimeCellTemplate(ScmColumnInfo info)
        {
            DataTemplate template = new DataTemplate();
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(TextBlock));
            factory.SetBinding(TextBlock.TextProperty, new Binding(info.Value) { Mode = info.Mode, StringFormat = "{0:yyyy-MM-dd HH:mm:ss}" });
            template.VisualTree = factory;
            return template;
        }

        /// <summary>
        /// 创建数值列
        /// </summary>
        private static DataTemplate CreateNumberCellTemplate(ScmColumnInfo info)
        {
            DataTemplate template = new DataTemplate();
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(TextBlock));
            factory.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Right);
            factory.SetBinding(TextBlock.TextProperty, new Binding(info.Value) { Mode = BindingMode.OneWay, StringFormat = "{0:N2}" });
            template.VisualTree = factory;
            return template;
        }

        /// <summary>
        /// 创建图标列
        /// </summary>
        private static DataTemplate CreateIconCellTemplate(ScmColumnInfo info)
        {
            DataTemplate template = new DataTemplate();
            FrameworkElementFactory gridFactory = new FrameworkElementFactory(typeof(Grid));
            gridFactory.SetValue(Grid.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            gridFactory.SetValue(Grid.VerticalAlignmentProperty, VerticalAlignment.Center);

            var iconFactory = new FrameworkElementFactory(typeof(PackIconMaterial));
            iconFactory.SetBinding(PackIconMaterial.KindProperty, new Binding(info.Value) { Converter = info.Converter, Mode = BindingMode.OneWay });
            if (info.Foreground != null)
            {
                iconFactory.SetBinding(PackIconMaterial.ForegroundProperty, new Binding(info.Foreground) { Mode = BindingMode.OneWay });
            }
            if (info.Background != null)
            {
                iconFactory.SetBinding(PackIconMaterial.BackgroundProperty, new Binding(info.Background) { Mode = BindingMode.OneWay });
            }

            gridFactory.AppendChild(iconFactory);

            template.VisualTree = gridFactory;
            return template;
        }
    }
}
