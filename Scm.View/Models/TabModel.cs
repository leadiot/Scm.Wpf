using Com.Scm.Dvo;
using System.Windows;

namespace Com.Scm.Models
{
    public partial class TabModel : ScmDvo
    {
        private string code;
        public string Code { get { return code; } set { SetProperty(ref code, value); } }

        private string header;
        public string Header { get { return header; } set { SetProperty(ref header, value); } }

        /// <summary>
        /// 是否可以关闭
        /// </summary>
        public bool Closable { get; set; } = true;

        private FrameworkElement content;
        public FrameworkElement Content { get { return content; } set { SetProperty(ref content, value); } }
    }
}
