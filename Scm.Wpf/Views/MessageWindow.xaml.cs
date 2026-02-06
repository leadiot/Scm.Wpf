namespace Com.Scm.Views
{
    /// <summary>
    /// ExitWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MessageWindow : HandyControl.Controls.Window
    {
        public MessageWindow()
        {
            InitializeComponent();
        }

        public string Message
        {
            get
            {
                return TbInfo.Text;
            }
            set
            {
                TbInfo.Text = value;
            }
        }

        public static bool? ShowDialog(System.Windows.Window owner, string message, string title = null)
        {
            var dialog = new MessageWindow();
            dialog.Owner = owner;
            dialog.Title = title ?? "提示";
            dialog.Message = message;
            return dialog.ShowDialog();
        }

        private void BtCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DoCancel();
        }

        private void BtAccept_Click(object sender, System.Windows.RoutedEventArgs e)
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
            DialogResult = true;
            Close();
        }
    }
}
