namespace Com.Scm.Wpf.Config
{
    public class EnvConfig
    {
        public string ApiUrl { get; set; } = "http://api.c-scm.net/api";

        public string GetUrl(string url)
        {
            return ApiUrl + url;
        }
    }
}
