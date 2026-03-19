using System.Windows.Controls;

namespace Com.Scm.Views.Samples.Native
{
    /// <summary>
    /// EditView.xaml 的交互逻辑
    /// </summary>
    public partial class EditView : UserControl
    {
        private EditViewDvo _Dvo;

        public EditView()
        {
            InitializeComponent();
        }

        public void Init(EditViewDvo dvo)
        {
            _Dvo = dvo;
            this.DataContext = dvo;
        }
    }
}
