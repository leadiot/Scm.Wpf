namespace Com.Scm.Wpf.Config
{
    public class EnvConfig
    {
        public string BaseUrl { get; set; } = "http://api.c-scm.net";

        public string ApiPath { get; set; } = "/api";

        public void Init()
        {
            if (string.IsNullOrEmpty(BaseUrl))
            {
                BaseUrl = "http://api.c-scm.net";
            }
            BaseUrl = BaseUrl.Trim().TrimEnd('/');

            if (string.IsNullOrEmpty(ApiPath))
            {
                ApiPath = "/api";
            }
            ApiPath = ApiPath.Trim().TrimEnd('/');
        }

        public string GetApiUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            if (url.IndexOf("//") > -1)
            {
                return url;
            }

            if (url[0] != '/')
            {
                url = '/' + url;
            }

            return BaseUrl + ApiPath + url;
        }
    }
}
