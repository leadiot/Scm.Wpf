using Com.Scm.Helper;
using Com.Scm.Models;
using System.Collections;
using System.Windows;
using System.Windows.Documents;

namespace Com.Scm.Controls
{
    /// <summary>
    /// 打印预览窗口
    /// </summary>
    public partial class PrintPreviewWindow : Window
    {
        private IEnumerable _ItemSource;
        private List<ScmColumnInfo> _Columns;
        private string _PrintTitle;

        public PrintPreviewWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化预览
        /// </summary>
        /// <param name="document">预览文档</param>
        /// <param name="itemSource">数据列表</param>
        /// <param name="columns">列配置</param>
        /// <param name="title">打印标题</param>
        public void Init(FlowDocument document, IEnumerable itemSource, List<ScmColumnInfo> columns, string title)
        {
            _ItemSource = itemSource;
            _Columns = columns;
            _PrintTitle = title;

            Title = string.IsNullOrEmpty(title) ? "打印预览" : $"打印预览 - {title}";
            Viewer.Document = document;
        }

        /// <summary>
        /// 打印
        /// </summary>
        private void BtPrint_Click(object sender, RoutedEventArgs e)
        {
            GridPrinter.Print(_ItemSource, _Columns, _PrintTitle);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        private void BtClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
