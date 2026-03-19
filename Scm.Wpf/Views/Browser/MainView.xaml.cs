using System.Windows.Controls;

namespace Com.Scm.Views.Browser
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : UserControl
    {
        private ScmWindow _Window;

        public MainView()
        {
            InitializeComponent();
        }

        public void Init(ScmWindow window)
        {
            _Window = window;

            InitializeWebView2Async();
        }

        private async void InitializeWebView2Async()
        {
            try
            {
                await WbBrowser.EnsureCoreWebView2Async();

                var settings = WbBrowser.CoreWebView2.Settings;

                settings.IsScriptEnabled = true;
                settings.AreDevToolsEnabled = false;
            }
            catch (Exception ex)
            {
                _Window.ShowException(ex);
            }
        }

        public void Browse(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                WbBrowser.Source = uri;
            }
            else
            {
                _Window.ShowError("请输入有效的URL（如：https://www.baidu.com）", "提示");
            }
        }
    }
}
