using System.Windows.Controls;

namespace Com.Scm.Wpf.Views.Samples.Native
{
    /// <summary>
    /// EditView.xaml 的交互逻辑
    /// </summary>
    public partial class EditControl : UserControl
    {
        private EditControlDvo _Dvo;

        public EditControl()
        {
            InitializeComponent();
        }

        public void Init(EditControlDvo dvo)
        {
            _Dvo = dvo;
            this.DataContext = dvo;
        }
    }
}
