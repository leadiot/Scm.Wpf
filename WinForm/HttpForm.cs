using Com.Scm;

namespace WinForm
{
    public partial class HttpForm : Form
    {
        public HttpForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var terminal = new ScmTerminal(@"D:\workspace\Git\Nas.Wpf\Nas.Wpf\bin\Debug\net10.0-windows");
            terminal.LoadToken();
            terminal.GetObject<string>("/Nas/Info/2070388028899201024");
        }
    }
}
