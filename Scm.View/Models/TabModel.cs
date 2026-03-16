using Com.Scm.Wpf.Dvo;
using System.Windows;

namespace Com.Scm.Wpf.Models
{
    public partial class TabModel : ScmDvo
    {
        private string code;
        public string Code { get { return code; } set { SetProperty(ref code, value); } }

        private string header;
        public string Header { get { return header; } set { SetProperty(ref header, value); } }

        private FrameworkElement content;
        public FrameworkElement Content { get { return content; } set { SetProperty(ref content, value); } }
    }
}
