using System.Windows.Controls;

namespace Com.Scm.Wpf.Views.Uc
{
    /// <summary>
    /// UcArticle.xaml 的交互逻辑
    /// </summary>
    public partial class UcArticle : UserControl
    {
        public UcArticle()
        {
            InitializeComponent();
        }

        public string Title { get; set; }

        public string Value { get; set; }
    }
}
