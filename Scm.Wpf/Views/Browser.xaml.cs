using System.Windows.Controls;

namespace Com.Scm
{
    /// <summary>
    /// Browser.xaml 的交互逻辑
    /// </summary>
    public partial class Browser : HandyControl.Controls.Window
    {
        public Browser()
        {
            InitializeComponent();
            SuppressScriptErrors(WbBrowser, true);
        }

        public void Open(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            //Show();
            WbBrowser.Navigate(url);
            ShowDialog();
        }

        void SuppressScriptErrors(WebBrowser browser, bool hide)
        {
            browser.Navigating += (s, e) =>
            {
                var fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2",
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.NonPublic);
                if (fiComWebBrowser == null)
                {
                    return;
                }

                object objComBrowser = fiComWebBrowser.GetValue(browser);
                if (objComBrowser == null)
                {
                    return;
                }
                objComBrowser.GetType().InvokeMember("Silent",
                    System.Reflection.BindingFlags.SetProperty,
                    null,
                    objComBrowser, new object[] { hide });
            };
        }
    }
}
