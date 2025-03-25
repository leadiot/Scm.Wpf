using Com.Scm.Wpf.Models;
using MiniExcelLibs;
using MiniExcelLibs.Attributes;
using MiniExcelLibs.OpenXml;
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

        public void SetColumns(List<ColumnInfo> columns, bool autoData = true)
        {
            if (autoData)
            {
                columns.Add(new ColumnInfo { Label = "数据状态", Value = "row_status" });
                columns.Add(new ColumnInfo { Label = "更新人员", Value = "update_name" });
                columns.Add(new ColumnInfo { Label = "更新时间", Value = "update_time" });
                columns.Add(new ColumnInfo { Label = "创建人员", Value = "create_name" });
                columns.Add(new ColumnInfo { Label = "创建时间", Value = "create_time" });
            }

            DgGrid.AutoGenerateColumns = false;
            foreach (var column in columns)
            {
                if (column.Type == Models.ColumnType.Text)
                {
                    DgGrid.Columns.Add(CreateTextColumn(column));
                    continue;
                }
                if (column.Type == Models.ColumnType.CheckBox)
                {
                    DgGrid.Columns.Add(CreateCheckboxColumn(column));
                    continue;
                }
            }
        }

        public void ShowData(IEnumerable dataSource)
        {
            DgGrid.ItemsSource = dataSource;
        }

        private DataGridTextColumn CreateTextColumn(ColumnInfo info)
        {
            var column = new DataGridTextColumn();
            column.Header = info.Label;
            column.Binding = new Binding(info.Value);
            column.IsReadOnly = true;

            if (info.Hidden)
            {
                column.Visibility = Visibility.Collapsed;
            }

            var uom = SizeUom.Parse(info.Width);
            if (uom.IsFill)
            {
                column.Width = new DataGridLength(0, DataGridLengthUnitType.Star);
            }
            if (uom.IsFixed)
            {
                column.Width = new DataGridLength(uom.Width, DataGridLengthUnitType.Pixel);
            }

            uom = SizeUom.Parse(info.MinWidth);
            if (uom.IsFixed)
            {
                column.MinWidth = uom.Width;
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

            var uom = SizeUom.Parse(info.Width);
            if (uom.IsFill)
            {
                column.Width = new DataGridLength(0, DataGridLengthUnitType.Star);
            }
            if (uom.IsFixed)
            {
                column.Width = new DataGridLength(uom.Width, DataGridLengthUnitType.Pixel);
            }

            uom = SizeUom.Parse(info.MinWidth);
            if (uom.IsFixed)
            {
                column.MinWidth = uom.Width;
            }

            return column;
        }

        public void ShowData0(IEnumerable dataSource)
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

        public async Task<bool> Export(IEnumerable itemSource, List<ColumnInfo> columns)
        {
            try
            {
                if (columns == null || columns.Count < 1)
                {
                    throw new Exception("请选择需要导出的列!");
                }

                #region 配置
                var config = new OpenXmlConfiguration { };
                List<DynamicExcelColumn> objs = new List<DynamicExcelColumn>();
                int index = 0;
                foreach (var columnParam in columns)
                {
                    var tmp = new DynamicExcelColumn(columnParam.Label);
                    tmp.Index = index++;

                    var uom = SizeUom.Parse(columnParam.Width);
                    if (!uom.IsNone)
                    {
                        tmp.Width = uom.Width;
                    }
                    uom = SizeUom.Parse(columnParam.MinWidth);
                    if (!uom.IsNone)
                    {
                        tmp.Width = uom.Width;
                    }
                    objs.Add(tmp);
                }
                config.DynamicColumns = objs.ToArray();
                #endregion

                #region 获取值
                var values = new List<Dictionary<string, object>>();
                foreach (var dto in itemSource)
                {
                    var dic = new Dictionary<string, object>();
                    foreach (var columnParam in columns)
                    {
                        dic.Add(columnParam.Label, GetModelValue(columnParam.Value, dto));
                    }
                    values.Add(dic);
                }
                #endregion

                await MiniExcel.SaveAsAsync("", null);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"动态列报表导出错误:{ex.Message}");
            }
        }

        private string GetModelValue(string fieldName, object obj)
        {
            try
            {
                object o = obj.GetType().GetProperty(fieldName).GetValue(obj, null);
                string Value = Convert.ToString(o);
                if (string.IsNullOrEmpty(Value)) return "";
                return Value;
            }
            catch
            {
                return "";
            }
        }
    }
}
