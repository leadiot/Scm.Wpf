using Com.Scm.Controls.Windows;
using System.Windows;

namespace Com.Scm
{
    /// <summary>
    /// DemoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DemoWindow : HandyControl.Controls.Window
    {
        private DemoWindowDvo _Dvo;

        public DemoWindow()
        {
            InitializeComponent();
        }

        public void Init(ScmWindow window)
        {
            _Dvo = new DemoWindowDvo();
            _Dvo.Init(window);

            this.DataContext = _Dvo;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dvo = new InputWindowDvo();
            dvo.Title = "aaa";
            dvo.Message = "bbb";
            dvo.Value = "111";
            var window = InputWindow.ShowInput(this, dvo);

            var input = window.InputText;
            TbInput.Text = input;
        }
    }
}