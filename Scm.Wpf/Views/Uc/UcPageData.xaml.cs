using Com.Scm.Utils;
using Com.Scm.Wpf.Dvo;
using Com.Scm.Wpf.Models;
using HandyControl.Controls;
using Microsoft.Win32;
using MiniExcelLibs;
using MiniExcelLibs.Attributes;
using MiniExcelLibs.OpenXml;
using System.Collections;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Com.Scm.Wpf.Views.Uc
{
    /// <summary>
    /// UcPageData.xaml 的交互逻辑
    /// </summary>
    public partial class UcPageData : UserControl
    {
        private Views.ISearchView _Owner;
        private ScmPageDataDvo _Dvo;
        private List<ScmColumnInfo> _Columns;
        private List<int> _PageItems = new List<int> { 10, 20, 30, 50, 100, 200, 300, 500 };

        public UcPageData()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 列属性设置
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="autoData"></param>
        public void Init(Views.ISearchView owner, List<ScmColumnInfo> columns, bool autoData = true)
        {
            _Owner = owner;

            //CbItems.ItemsSource = _PageItems;

            _Dvo = new ScmPageDataDvo();
            this.DataContext = _Dvo;

            _Columns = columns;
            if (columns == null)
            {
                DgGrid.AutoGenerateColumns = true;
                return;
            }

            if (autoData)
            {
                columns.Add(new ScmColumnInfo { Type = Models.ScmColumnType.Status, Label = "数据状态", Value = "IsEnabled" });
                columns.Add(new ScmColumnInfo { Type = Models.ScmColumnType.Text, Label = "更新人员", Value = "update_name" });
                columns.Add(new ScmColumnInfo { Type = Models.ScmColumnType.Text, Label = "更新时间", Value = "UpdateTime", Format = ScmColumnFormat.DateTime });
                columns.Add(new ScmColumnInfo { Type = Models.ScmColumnType.Text, Label = "创建人员", Value = "create_name" });
                columns.Add(new ScmColumnInfo { Type = Models.ScmColumnType.Text, Label = "创建时间", Value = "CreateTime", Format = ScmColumnFormat.DateTime });
            }

            DgGrid.AutoGenerateColumns = false;
            foreach (var column in columns)
            {
                if (column.Type == Models.ScmColumnType.Text)
                {
                    DgGrid.Columns.Add(CreateTextColumn(column));
                    continue;
                }
                if (column.Type == Models.ScmColumnType.CheckBox)
                {
                    DgGrid.Columns.Add(CreateCheckboxColumn(column));
                    continue;
                }
                if (column.Type == Models.ScmColumnType.Template)
                {
                    DgGrid.Columns.Add(CreateTemplateColumn(column));
                    continue;
                }
                if (column.Type == Models.ScmColumnType.Status)
                {
                    DgGrid.Columns.Add(CreateStatusColumn(column));
                    continue;
                }
            }
        }

        #region 列配置
        /// <summary>
        /// 列通用属性配置
        /// </summary>
        /// <param name="column"></param>
        /// <param name="info"></param>
        private void AdjustColumnInfo(DataGridColumn column, ScmColumnInfo info)
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
        }

        /// <summary>
        /// 创建文本框列
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private DataGridTextColumn CreateTextColumn(ScmColumnInfo info)
        {
            var column = new DataGridTextColumn();
            AdjustColumnInfo(column, info);
            column.Binding = new Binding(info.Value);
            column.IsReadOnly = info.ReadOnly;

            Style styleRight = new Style(typeof(TextBlock));
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
            styleRight.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, align));
            column.ElementStyle = styleRight;

            return column;
        }

        /// <summary>
        /// 创建复选框列
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private DataGridCheckBoxColumn CreateCheckboxColumn(ScmColumnInfo info)
        {
            var column = new DataGridCheckBoxColumn();
            AdjustColumnInfo(column, info);
            var checkbox = new CheckBox();
            //checkbox.Content = "全选";
            checkbox.Click += CheckAll_Click;
            column.Header = checkbox;
            column.Binding = new Binding(info.Value);
            column.CanUserSort = false;

            return column;
        }

        /// <summary>
        /// 创建状态列
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private DataGridTemplateColumn CreateStatusColumn(ScmColumnInfo info)
        {
            var column = new DataGridTemplateColumn();
            AdjustColumnInfo(column, info);

            DataTemplate template = new DataTemplate();
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(ToggleButton));
            factory.SetValue(ToggleButton.StyleProperty, FindResource("ToggleButtonSwitch"));
            factory.SetBinding(ToggleButton.IsCheckedProperty, new Binding("IsEnabled"));
            template.VisualTree = factory;

            column.CellTemplate = template;
            return column;
        }

        /// <summary>
        /// 创建模板列
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private DataGridTemplateColumn CreateTemplateColumn(ScmColumnInfo info)
        {
            var column = new DataGridTemplateColumn();
            if (info.Format == ScmColumnFormat.Number)
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
        /// <param name="info"></param>
        /// <returns></returns>
        private static DataTemplate CreateDateTimeCellTemplate(ScmColumnInfo info)
        {
            DataTemplate template = new DataTemplate();
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(TextBlock));
            factory.SetBinding(TextBlock.TextProperty, new Binding(info.Value) { StringFormat = "{0:yyyy-MM-dd HH:mm:ss}" });
            template.VisualTree = factory;
            return template;
        }

        /// <summary>
        /// 创建数值列
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private static DataTemplate CreateNumberCellTemplate(ScmColumnInfo info)
        {
            DataTemplate template = new DataTemplate();
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(TextBlock));
            factory.SetBinding(TextBlock.TextProperty, new Binding(info.Value) { StringFormat = "{0:N2}" });
            template.VisualTree = factory;
            return template;
        }
        #endregion

        /// <summary>
        /// 数据展示
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        public void ShowData<T>(ScmSearchPageResponse<T> response) where T : ScmSearchResultItemDvo
        {
            _Dvo.PageIndex = 1;
            _Dvo.TotalPages = (int)response.TotalPages;
            _Dvo.TotalItems = (int)response.TotalItems;

            DgGrid.ItemsSource = response.Items;
        }

        /// <summary>
        /// 选中所有事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckAll_Click(object sender, RoutedEventArgs e)
        {
            var checkbox = (CheckBox)sender;
            SetChecked(checkbox.IsChecked.Value);
        }

        /// <summary>
        /// 设置选中状态
        /// </summary>
        /// <param name="isChecked"></param>
        private void SetChecked(bool isChecked)
        {
            foreach (var item in DgGrid.ItemsSource)
            {
                var dvo = item as ScmSearchResultItemDvo;
                if (dvo == null) return;
                dvo.Checked = isChecked;
            }
        }

        /// <summary>
        /// 数据导出事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtExport_Click(object sender, RoutedEventArgs e)
        {
            Export();
        }

        #region 数据导出
        /// <summary>
        /// 执行导出
        /// </summary>
        private async void Export()
        {
            var dialog = new SaveFileDialog();
            //dialog.CheckFileExists = true;
            dialog.Filter = "Excel 文件(*.xlsx)|*.xlsx|Excel 97-2003 文件(*.xls)|*.xls|CSV (*.csv)|*.csv|SQL (*.sql)|*.sql|JSON (*.json)|*.json";
            var result = dialog.ShowDialog().Value;
            if (!result)
            {
                return;
            }

            var fileName = dialog.FileName;
            if (File.Exists(fileName))
            {
                try
                {
                    File.Delete(fileName);
                }
                catch (Exception exp)
                {
                    Growl.Error(exp.Message);
                    // 文件删除异常
                    return;
                }
            }

            result = false;
            if (fileName.EndsWith(".json"))
            {
                result = await ExportJson(DgGrid.ItemsSource, _Columns, fileName);
            }
            else if (fileName.EndsWith(".csv"))
            {
                result = await ExportCsv(DgGrid.ItemsSource, _Columns, fileName);
            }
            else if (fileName.EndsWith(".sql"))
            {
                result = await ExportSql(DgGrid.ItemsSource, _Columns, fileName);
            }
            else
            {
                result = await ExportXls(DgGrid.ItemsSource, _Columns, fileName);
            }

            if (result)
            {
                Growl.Success("数据导出成功！");
            }
        }

        /// <summary>
        /// 导出CSV
        /// </summary>
        /// <param name="itemSource"></param>
        /// <param name="columns"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> ExportCsv(IEnumerable itemSource, List<ScmColumnInfo> columns, string file)
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
                foreach (var columnParam in columns)
                {
                    var col = new DynamicExcelColumn(columnParam.Label);
                    objs.Add(col);
                }
                config.DynamicColumns = objs.ToArray();
                #endregion

                var items = GetValues(itemSource, columns);

                await MiniExcel.SaveAsAsync(file, items);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"动态列报表导出错误:{ex.Message}");
            }
        }

        /// <summary>
        /// 导出JSON
        /// </summary>
        /// <param name="itemSource"></param>
        /// <param name="columns"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<bool> ExportJson(IEnumerable itemSource, List<ScmColumnInfo> columns, string file)
        {
            var items = GetValues(itemSource, columns);

            var json = items.ToJsonString();

            await File.WriteAllTextAsync(file, json);

            return true;
        }

        /// <summary>
        /// 导出SQL
        /// </summary>
        /// <param name="itemSource"></param>
        /// <param name="columns"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<bool> ExportSql(IEnumerable itemSource, List<ScmColumnInfo> columns, string file)
        {
            var items = GetValues(itemSource, columns);

            // INSERT INTO table_name (column1, column2, column3) VALUES ('', '', '');
            var builder = new StringBuilder();
            builder.Append("INSERT INTO table_name (");
            foreach (var column in columns)
            {
                builder.Append(column.Value).Append(',');
            }
            builder.Remove(builder.Length - 1, 1);
            builder.Append(") VALUES (");

            var insert = builder.ToString();
            builder.Clear();

            foreach (var item in items)
            {
                builder.Append(insert);

                foreach (var column in columns)
                {
                    var val = item[column.Label];
                    builder.Append(val).Append(',');
                }

                builder.Remove(builder.Length - 1, 1);
                builder.AppendLine(");");
            }

            await File.WriteAllTextAsync(file, builder.ToString());

            return true;
        }

        /// <summary>
        /// 导出XLS
        /// </summary>
        /// <param name="itemSource"></param>
        /// <param name="columns"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> ExportXls(IEnumerable itemSource, List<ScmColumnInfo> columns, string file)
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

                    var uom = ScmColumnSize.Parse(columnParam.Width);
                    if (!uom.IsNone)
                    {
                        tmp.Width = uom.Width;
                    }
                    uom = ScmColumnSize.Parse(columnParam.MinWidth);
                    if (!uom.IsNone)
                    {
                        tmp.Width = uom.Width;
                    }
                    objs.Add(tmp);
                }
                config.DynamicColumns = objs.ToArray();
                #endregion

                var items = GetValues(itemSource, columns);

                await MiniExcel.SaveAsAsync(file, items);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"动态列报表导出错误:{ex.Message}");
            }
        }

        /// <summary>
        /// 导出Txt
        /// </summary>
        /// <param name="itemSource"></param>
        /// <param name="columns"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> ExportTxt(IEnumerable itemSource, List<ScmColumnInfo> columns, string file)
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

                    var uom = ScmColumnSize.Parse(columnParam.Width);
                    if (!uom.IsNone)
                    {
                        tmp.Width = uom.Width;
                    }
                    uom = ScmColumnSize.Parse(columnParam.MinWidth);
                    if (!uom.IsNone)
                    {
                        tmp.Width = uom.Width;
                    }
                    objs.Add(tmp);
                }
                config.DynamicColumns = objs.ToArray();
                #endregion

                var items = GetValues(itemSource, columns);

                await MiniExcel.SaveAsAsync(file, items);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"动态列报表导出错误:{ex.Message}");
            }
        }

        /// <summary>
        /// 转换为导出列
        /// </summary>
        /// <param name="source"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> GetValues(IEnumerable source, List<ScmColumnInfo> columns)
        {
            var data = new List<Dictionary<string, object>>();
            foreach (var item in source)
            {
                data.Add(GetValue(item, columns));
            }
            return data;
        }

        /// <summary>
        /// 获取可导出列
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        private Dictionary<string, object> GetValue(object obj, List<ScmColumnInfo> columns)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                var type = obj.GetType();
                foreach (var column in columns)
                {
                    if (!column.Export)
                    {
                        continue;
                    }

                    var prop = type.GetProperty(column.Value);
                    if (prop == null)
                    {
                        continue;
                    }

                    //var propType = prop.DeclaringType;
                    var tmp = prop.GetValue(obj, null);
                    if (tmp is long)
                    {
                        dic[column.Label] = tmp.ToString();
                        continue;
                    }
                    if (tmp is DateTime)
                    {
                        var time = (DateTime)tmp;
                        dic[column.Label] = TimeUtils.FormatDataTime(time);
                        continue;
                    }

                    dic[column.Label] = tmp;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return dic;
        }
        #endregion

        private void BtOption_Click(object sender, RoutedEventArgs e)
        {
            var view = new UcGridColumnsView();

            var window = new PopupWindow();
            window.PopupElement = view;
            window.Width = 240;
            window.Height = 480;
            window.Show(BtOption, false);
        }

        #region 翻页事件
        /// <summary>
        /// 首页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtFirst_Click(object sender, RoutedEventArgs e)
        {
            _Owner.FirstPageAsync();
        }

        /// <summary>
        /// 上一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtPrev_Click(object sender, RoutedEventArgs e)
        {
            _Owner.PrevPageAsync();
        }

        /// <summary>
        /// 下一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtNext_Click(object sender, RoutedEventArgs e)
        {
            _Owner.NextPageAsync();
        }

        /// <summary>
        /// 尾页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtEnd_Click(object sender, RoutedEventArgs e)
        {
            _Owner.EndPageAsync();
        }

        /// <summary>
        /// 指定页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TbPage_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter)
            {
                return;
            }

            e.Handled = true;
            _Owner.FixedPageAsync(_Dvo.PageIndex);
        }
        #endregion

        /// <summary>
        /// 页数量调整
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public void ShowInfo(FrameworkElement element)
        {
            TbTitle.Text = "详情";

            GdPanel.Children.Clear();
            GdPanel.Children.Add(element);

            BtAccept.Visibility = Visibility.Collapsed;
            BtSearch.Visibility = Visibility.Collapsed;

            DrSide.IsOpen = true;
        }

        private SaveDelegate _SaveDelegate;

        public void ShowEdit(FrameworkElement view, SaveDelegate saveDelegate)
        {
            TbTitle.Text = "编辑";

            _SaveDelegate = saveDelegate;

            GdPanel.Children.Clear();
            GdPanel.Children.Add(view);

            BtAccept.Visibility = Visibility.Visible;
            BtSearch.Visibility = Visibility.Collapsed;

            DrSide.IsOpen = true;
        }

        private SearchDelegate _SearchDelegate;

        public void ShowSearch(FrameworkElement view, SearchDelegate searchDelegate)
        {
            TbTitle.Text = "查询";

            _SearchDelegate = searchDelegate;

            GdPanel.Children.Clear();
            GdPanel.Children.Add(view);

            BtAccept.Visibility = Visibility.Collapsed;
            BtSearch.Visibility = Visibility.Visible;

            DrSide.IsOpen = true;
        }

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

            DrSide.IsOpen = false;
        }

        private void BtSearch_Click(object sender, RoutedEventArgs e)
        {
            if (_SearchDelegate == null)
            {
                return;
            }

            if (!_SearchDelegate())
            {
                return;
            }

            DrSide.IsOpen = false;
        }

        private void BtCancel_Click(object sender, RoutedEventArgs e)
        {
            DrSide.IsOpen = false;
        }
    }

    public delegate bool SaveDelegate();

    public delegate bool SearchDelegate();

    public partial class ScmPageDataDvo : ScmDvo
    {
        private int pageIndex;
        public int PageIndex { get { return pageIndex; } set { SetProperty(ref pageIndex, value); } }

        private int pageItems = 20;
        public int PageItems { get { return pageItems; } set { SetProperty(ref pageItems, value); } }

        private int view;
        public int View { get { return view; } set { SetProperty(ref view, value); } }

        private int totalPages;
        public int TotalPages { get { return totalPages; } set { SetProperty(ref totalPages, value); } }

        private int totalItems;
        public int TotalItems { get { return totalItems; } set { SetProperty(ref totalItems, value); } }

        public string PagesInfo
        {
            get { return $"共 {TotalPages} 页"; }
        }
    }
}
