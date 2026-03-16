namespace Com.Scm
{
    /// <summary>
    /// SCM窗口接口
    /// </summary>
    public interface ScmWindow
    {
        /// <summary>
        /// GET请求，返回对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="query"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        Task<T> GetObjectAsync<T>(string url, Dictionary<string, string> query = null, Dictionary<string, string> head = null);

        /// <summary>
        /// GET请求，返回文本
        /// </summary>
        /// <param name="url"></param>
        /// <param name="query"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        Task<string> GetStringAsync(string url, Dictionary<string, string> query = null, Dictionary<string, string> head = null);

        /// <summary>
        /// POST请求，返回对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="form"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        Task<T> PostFormObjectAsync<T>(string url, Dictionary<string, string> form = null, Dictionary<string, string> head = null);

        /// <summary>
        /// POST请求，返回文本
        /// </summary>
        /// <param name="url"></param>
        /// <param name="form"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        Task<string> PostFormStringAsync(string url, Dictionary<string, string> form = null, Dictionary<string, string> head = null);

        /// <summary>
        /// POST请求，返回对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="json"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        Task<T> PostJsonObjectAsync<T>(string url, string json = null, Dictionary<string, string> head = null);

        /// <summary>
        /// POST请求，返回文本
        /// </summary>
        /// <param name="url"></param>
        /// <param name="json"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        Task<string> PostJsonStringAsync(string url, string json = null, Dictionary<string, string> head = null);

        void HideGuid();

        void ShowGuid();

        void HideMenu();

        void ShowMenu();

        void HideTray();

        void ShowTray();

        void ShowAlert(string message, string title = null);

        void ShowToast(string message, ToastType type = ToastType.Info);

        void ShowNotice(string message);

        void ShowError(string message);

        void ShowException(Exception exception);

        bool? ShowDialog(string message, string title = null);

        void ShowHome();

        /// <summary>
        /// 显示视图
        /// </summary>
        /// <param name="viewClass"></param>
        /// <param name="useCache"></param>
        void ShowView(string codec, string namec, string viewClass, bool useCache = true);

        ScmAppInfo GetAppInfo(string code);

        ScmVerInfo GetVerInfo(string code);
    }
}
