using System.Windows.Controls;

namespace Com.Scm.Wpf.Views.Samples.Native
{
    /// <summary>
    /// EditView.xaml 的交互逻辑
    /// </summary>
    public partial class EditView : UserControl
    {
        private EditDvo _Dvo;

        public EditView()
        {
            InitializeComponent();
        }

        public void Init(EditDvo dvo)
        {
            _Dvo = dvo;
            this.DataContext = dvo;
        }
    }
}
