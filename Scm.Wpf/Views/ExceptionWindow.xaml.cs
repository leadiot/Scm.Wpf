using System.Windows;

namespace Com.Scm.Views
{
    /// <summary>
    /// ExceptionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ExceptionWindow : HandyControl.Controls.Window
    {
        public ExceptionWindow()
        {
            InitializeComponent();
        }

        public void ShowException(Exception e)
        {
            if (e == null)
            {
                return;
            }

            TbTitle.Text = e.Message;
            TbTrace.Text = e.StackTrace;
        }

        public static bool? ShowException(Window owner, Exception e, string title = null)
        {
            var dialog = new ExceptionWindow();
            dialog.Owner = owner;
            dialog.Title = title ?? "提示";
            dialog.ShowException(e);
            return dialog.ShowDialog();
        }

        private void BtAccept_Click(object sender, RoutedEventArgs e)
        {
            DoAccept();
        }

        private void DoAccept()
        {
            DialogResult = true;
            Close();
        }
    }
}
