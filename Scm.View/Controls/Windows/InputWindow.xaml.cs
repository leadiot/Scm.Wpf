using Com.Scm.Dvo;
using System.Windows;

namespace Com.Scm.Controls.Windows
{
    /// <summary>
    /// InputWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InputWindow : Window
    {
        private InputWindowDvo _Dvo;

        public InputWindow()
        {
            InitializeComponent();
        }

        public void Init(InputWindowDvo dvo)
        {
            _Dvo = dvo;
            if (string.IsNullOrEmpty(dvo.Title))
            {
                dvo.Title = "输入";
            }
            if (string.IsNullOrEmpty(dvo.Message))
            {
                dvo.Message = "请输入：";
            }

            this.DataContext = dvo;
            TbInput.Focus();
        }

        public static InputWindow ShowInput(Window owner, string text = null, string value = null, string title = null)
        {
            var dvo = new InputWindowDvo();
            dvo.Title = title;
            dvo.Message = text;
            dvo.Value = value;

            InputWindow inputWindow = new InputWindow();
            inputWindow.Init(dvo);
            inputWindow.Owner = owner;
            inputWindow.ShowDialog();
            return inputWindow;
        }

        public static InputWindow ShowInput(Window owner, InputWindowDvo dvo)
        {
            InputWindow inputWindow = new InputWindow();
            inputWindow.Init(dvo);
            inputWindow.Owner = owner;
            inputWindow.ShowDialog();
            return inputWindow;
        }

        public string InputText
        {
            get { return TbInput.Text; }
            set { TbInput.Text = value; }
        }

        private void BtCancel_Click(object sender, RoutedEventArgs e)
        {
            DoCancel();
        }

        private void BtAccept_Click(object sender, RoutedEventArgs e)
        {
            DoAccept();
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                e.Handled = true;
                DoCancel();
                return;
            }

            if (e.Key == System.Windows.Input.Key.Enter)
            {
                e.Handled = true;
                DoAccept();
                return;
            }
        }

        private void DoCancel()
        {
            DialogResult = false;
            Close();
        }

        private void DoAccept()
        {
            if (!_Dvo.IsValid())
            {
                return;
            }

            DialogResult = true;
            Close();
        }
    }

    public class InputWindowDvo : ScmDvo
    {
        private string _title = "输入";
        public string Title { get { return _title; } set { SetProperty(ref _title, value); } }

        private string _message = "请输入：";
        public string Message { get { return _message; } set { SetProperty(ref _message, value); } }

        private string _value;
        public string Value { get { return _value; } set { SetProperty(ref _value, value); } }
    }
}
