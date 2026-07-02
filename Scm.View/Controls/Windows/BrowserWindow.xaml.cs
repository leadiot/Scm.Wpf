using Com.Scm.Helper;
using System.Windows.Input;

namespace Com.Scm.Controls.Windows
{
    /// <summary>
    /// 浏览器窗口的交互逻辑
    /// </summary>
    public partial class BrowserWindow : HandyControl.Controls.Window
    {
        public BrowserWindow()
        {
            InitializeComponent();

            InitializeWebView2Async();
        }

        public void Open(string url)
        {
            TbAddress.Text = url;
            NavigateToAddress();
        }

        public void SetToolbarVisible(bool visible)
        {
            BdToolbar.Visibility = visible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public void SetOpenWithVisible(bool visible)
        {
            BtOpenWith.Visibility = visible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        private async void InitializeWebView2Async()
        {
            try
            {
                await WbBrowser.EnsureCoreWebView2Async();

                var settings = WbBrowser.CoreWebView2.Settings;

                settings.IsScriptEnabled = true;
                settings.AreDevToolsEnabled = false;

                WbBrowser.NavigationCompleted += WbBrowser_NavigationCompleted;
                UpdateButtonStates();
            }
            catch (Exception ex)
            {
                ExceptionWindow.ShowException(this, ex);
            }
        }

        private void WbBrowser_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            if (WbBrowser.Source != null)
            {
                TbAddress.Text = WbBrowser.Source.ToString();
            }
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            if (WbBrowser.CoreWebView2 != null)
            {
                BtBack.IsEnabled = WbBrowser.CoreWebView2.CanGoBack;
                BtForward.IsEnabled = WbBrowser.CoreWebView2.CanGoForward;
            }
            else
            {
                BtBack.IsEnabled = false;
                BtForward.IsEnabled = false;
            }
        }

        private void BtBack_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (WbBrowser.CoreWebView2 != null && WbBrowser.CoreWebView2.CanGoBack)
            {
                WbBrowser.CoreWebView2.GoBack();
            }
        }

        private void BtForward_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (WbBrowser.CoreWebView2 != null && WbBrowser.CoreWebView2.CanGoForward)
            {
                WbBrowser.CoreWebView2.GoForward();
            }
        }

        private void BtReload_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigateToAddress();
        }

        private void TbAddress_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NavigateToAddress();
            }
        }

        private void BtGo_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigateToAddress();
        }

        private void NavigateToAddress()
        {
            string url = TbAddress.Text.Trim();
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "https://" + url;
            }

            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                WbBrowser.Source = uri;
            }
            else
            {
                MessageWindow.ShowDialog(this, "请输入有效的URL（如：https://www.baidu.com）", "提示");
            }
        }

        private void BtOpenWith_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OsHelper.Browse(TbAddress.Text);
        }
    }
}
