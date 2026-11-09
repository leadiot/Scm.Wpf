using Com.Scm.Controls;
using Com.Scm.Models;
using HandyControl.Controls;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Com.Scm.Helper
{
    /// <summary>
    /// 列表数据打印服务
    /// </summary>
    public static class GridPrinter
    {
        /// <summary>
        /// 打印数据列表
        /// </summary>
        /// <param name="itemSource">数据列表</param>
        /// <param name="columns">列配置</param>
        /// <param name="title">打印标题</param>
        /// <returns></returns>
        public static bool Print(IEnumerable itemSource, IEnumerable<ScmColumnInfo> columns, string title = null)
        {
            var printColumns = GetPrintColumns(columns);
            if (printColumns.Count == 0)
            {
                Growl.Warning("没有可打印的列");
                return false;
            }

            var dialog = new PrintDialog();
            if (dialog.ShowDialog() != true)
            {
                return false;
            }

            try
            {
                var document = CreateDocument(itemSource, printColumns, title, dialog.PrintableAreaWidth, dialog.PrintableAreaHeight);

                dialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, title ?? "打印");
                Growl.Success("打印已完成");
                return true;
            }
            catch (Exception ex)
            {
                Growl.Error($"打印失败:{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 打印预览
        /// </summary>
        /// <param name="itemSource">数据列表</param>
        /// <param name="columns">列配置</param>
        /// <param name="title">打印标题</param>
        /// <returns></returns>
        public static bool Preview(IEnumerable itemSource, IEnumerable<ScmColumnInfo> columns, string title = null)
        {
            var printColumns = GetPrintColumns(columns);
            if (printColumns.Count == 0)
            {
                Growl.Warning("没有可打印的列");
                return false;
            }

            var document = CreateDocument(itemSource, printColumns, title, A4PageWidth, A4PageHeight);

            var window = new PrintPreviewWindow();
            window.Init(document, itemSource, printColumns, title);
            window.Owner = Application.Current == null ? null : Application.Current.MainWindow;
            window.ShowDialog();

            return true;
        }

        private const double A4PageWidth = 794;
        private const double A4PageHeight = 1123;

        /// <summary>
        /// 获取可打印的列
        /// </summary>
        private static List<ScmColumnInfo> GetPrintColumns(IEnumerable<ScmColumnInfo> columns)
        {
            return columns == null ? new List<ScmColumnInfo>() : columns.Where(a => !a.Hidden).ToList();
        }

        /// <summary>
        /// 创建打印文档
        /// </summary>
        private static FlowDocument CreateDocument(IEnumerable itemSource, List<ScmColumnInfo> columns, string title, double pageWidth, double pageHeight)
        {
            var document = new FlowDocument
            {
                PageWidth = pageWidth,
                PageHeight = pageHeight,
                PagePadding = new Thickness(40),
                ColumnWidth = double.PositiveInfinity,
                FontFamily = new FontFamily("Microsoft YaHei"),
                FontSize = 12
            };

            if (!string.IsNullOrEmpty(title))
            {
                document.Blocks.Add(new Paragraph(new Run(title))
                {
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 4)
                });
            }

            document.Blocks.Add(new Paragraph(new Run($"打印时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}"))
            {
                FontSize = 10,
                TextAlignment = TextAlignment.Right,
                Margin = new Thickness(0, 0, 0, 8)
            });

            var table = new Table
            {
                CellSpacing = 0,
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(0.5)
            };

            var useStar = IsFixedWidthOverflow(columns, pageWidth - 80);
            foreach (var column in columns)
            {
                table.Columns.Add(new TableColumn { Width = GetColumnWidth(column, useStar) });
            }

            var headerGroup = new TableRowGroup();
            var headerRow = new TableRow { Background = Brushes.Gainsboro, FontWeight = FontWeights.Bold };
            foreach (var column in columns)
            {
                headerRow.Cells.Add(CreateCell(column.Label, column.Align));
            }
            headerGroup.Rows.Add(headerRow);
            table.RowGroups.Add(headerGroup);

            var bodyGroup = new TableRowGroup();
            if (itemSource != null)
            {
                foreach (var item in itemSource)
                {
                    var row = new TableRow();
                    foreach (var column in columns)
                    {
                        row.Cells.Add(CreateCell(GetValue(item, column.Value), column.Align));
                    }
                    bodyGroup.Rows.Add(row);
                }
            }
            table.RowGroups.Add(bodyGroup);

            document.Blocks.Add(table);
            return document;
        }

        /// <summary>
        /// 固定列宽总和超出纸张可用宽度时统一使用比例宽度
        /// </summary>
        private static bool IsFixedWidthOverflow(List<ScmColumnInfo> columns, double availableWidth)
        {
            var total = 0d;
            foreach (var column in columns)
            {
                var uom = ScmColumnSize.Parse(column.Width);
                if (uom.IsFixed)
                {
                    total += uom.Width;
                }
            }
            return total > availableWidth;
        }

        /// <summary>
        /// 计算打印列宽
        /// </summary>
        private static GridLength GetColumnWidth(ScmColumnInfo column, bool useStar)
        {
            var uom = ScmColumnSize.Parse(column.Width);
            if (!useStar && uom.IsFixed)
            {
                return new GridLength(uom.Width, GridUnitType.Pixel);
            }
            return new GridLength(1, GridUnitType.Star);
        }

        /// <summary>
        /// 创建单元格
        /// </summary>
        private static TableCell CreateCell(string text, ScmColumnAlign align)
        {
            var paragraph = new Paragraph(new Run(text ?? string.Empty))
            {
                Margin = new Thickness(0),
                TextAlignment = GetAlignment(align)
            };

            return new TableCell(paragraph)
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(0.5),
                Padding = new Thickness(4, 3, 4, 3)
            };
        }

        /// <summary>
        /// 对齐方式
        /// </summary>
        private static TextAlignment GetAlignment(ScmColumnAlign align)
        {
            if (align == ScmColumnAlign.Center)
            {
                return TextAlignment.Center;
            }
            if (align == ScmColumnAlign.Right)
            {
                return TextAlignment.Right;
            }
            if (align == ScmColumnAlign.Stretch)
            {
                return TextAlignment.Justify;
            }
            return TextAlignment.Left;
        }

        /// <summary>
        /// 获取属性显示值
        /// </summary>
        private static string GetValue(object item, string property)
        {
            if (item == null || string.IsNullOrEmpty(property))
            {
                return string.Empty;
            }

            var prop = item.GetType().GetProperty(property);
            if (prop == null)
            {
                return string.Empty;
            }

            var value = prop.GetValue(item, null);
            if (value == null)
            {
                return string.Empty;
            }
            if (value is bool flag)
            {
                return flag ? "√" : string.Empty;
            }
            if (value is DateTime time)
            {
                return time.ToString("yyyy-MM-dd HH:mm:ss");
            }

            return value.ToString();
        }
    }
}
