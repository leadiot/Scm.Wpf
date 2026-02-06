using System.Windows.Controls;

namespace Com.Scm.Wpf.Views.Uc
{
    /// <summary>
    /// UcChart.xaml 的交互逻辑
    /// </summary>
    public partial class UcChart : UserControl
    {
        public UcChart()
        {
            InitializeComponent();
        }

        public string Title { get; set; }

        public string Value { get; set; }
    }
}
