using System.Windows.Controls;

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
    }
}
