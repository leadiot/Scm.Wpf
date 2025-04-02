namespace Com.Scm.Wpf
{
    public interface ScmWindow
    {
        void ShowView(string viewClass, bool useCache = true);

        Task<T> GetFormObjectAsync<T>(string url, Dictionary<string, string> body = null, Dictionary<string, string> head = null);

        Task<string> GetFormStringAsync(string url, Dictionary<string, string> body = null, Dictionary<string, string> head = null);

        Task<T> PostFormObjectAsync<T>(string url, Dictionary<string, string> body = null, Dictionary<string, string> head = null);

        Task<string> PostFormStringAsync(string url, Dictionary<string, string> body = null, Dictionary<string, string> head = null);

        Task<string> PostJsonStringAsync(string url, string body = null, Dictionary<string, string> head = null);

        Task<T> PostJsonObjectAsync<T>(string url, string body = null, Dictionary<string, string> head = null);
    }
}
