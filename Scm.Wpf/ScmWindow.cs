namespace Com.Scm.Wpf
{
    public interface ScmWindow
    {
        /// <summary>
        /// 显示视图
        /// </summary>
        /// <param name="viewClass"></param>
        /// <param name="useCache"></param>
        void ShowView(string viewClass, bool useCache = true);

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
    }
}
