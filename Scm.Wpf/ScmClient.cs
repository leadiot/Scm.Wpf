using Com.Scm.Sys.Menu;
using Com.Scm.Utils;
using Com.Scm.Wpf.Dto;
using Com.Scm.Wpf.Dto.Login;

namespace Com.Scm.Wpf
{
    public class ScmClient
    {
        /// <summary>
        /// 服务地址
        /// </summary>
        public const string SERVER_URL = "http://api.c-scm.net";
        /// <summary>
        /// 接口地址
        /// </summary>
        public const string API_URL = SERVER_URL + "/api";

        /// <summary>
        /// 访问凭据
        /// </summary>
        private string _AccessToken;
        /// <summary>
        /// 应用代码
        /// </summary>
        private string _AppKey;

        /// <summary>
        /// 当前用户
        /// </summary>
        public ScmUserInfo User { get; private set; }
        /// <summary>
        /// 用户菜单
        /// </summary>
        public List<MenuDto> Menu { get; private set; }

        public int ErrorCode { get; private set; }
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<bool> LoginAsync(Dictionary<string, string> body)
        {
            var url = GetUrl("/operator/login");

            var response = await HttpUtils.PostJsonObjectAsync<ScmDataResponse<LoginResult>>(url, body);
            if (response == null)
            {
                return false;
            }
            if (response.Code != 200)
            {
                ErrorMessage = response.GetMessage();
                return false;
            }

            var data = response.data;
            _AccessToken = data.AccessToken;
            User = data.UserInfo;
            return true;
        }

        public async Task<bool> LoadMenuAsync(string lang = null)
        {
            var url = GetUrl("/operator/authoritymenu");

            var body = new Dictionary<string, string>();
            body["client"] = "20";
            body["lang"] = lang ?? "zh-cn";

            var head = new Dictionary<string, string>();
            head["Accesstoken"] = _AccessToken;
            head["Appkey"] = _AppKey;

            var response = await HttpUtils.GetObjectAsync<ScmListResponse<MenuDto>>(url, body, head);
            if (response == null)
            {
                return false;
            }
            if (response.Code != 200)
            {
                ErrorMessage = response.GetMessage();
                return false;
            }

            Menu = response.data;
            return true;
        }

        /// <summary>
        /// POST请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        public async Task<T> PostAsync<T>(string url, Dictionary<string, string> body = null, Dictionary<string, string> head = null)
        {
            url = GetUrl(url);

            if (head == null)
            {
                head = new Dictionary<string, string>();
            }
            head["Accesstoken"] = _AccessToken;
            head["Appkey"] = _AppKey;

            var response = await HttpUtils.PostJsonObjectAsync<ScmDataResponse<T>>(url, body, head);
            if (response == null)
            {
                return default;
            }
            if (response.Success)
            {
                ErrorMessage = response.Message;
                return default;
            }

            return response.data;
        }

        /// <summary>
        /// GET请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string url, Dictionary<string, string> body = null, Dictionary<string, string> head = null)
        {
            url = GetUrl(url);

            if (head == null)
            {
                head = new Dictionary<string, string>();
            }
            head["Accesstoken"] = _AccessToken;
            head["Appkey"] = _AppKey;

            var response = await HttpUtils.GetObjectAsync<ScmDataResponse<T>>(url, body, head);
            if (response == null)
            {
                return default;
            }
            if (response.Success)
            {
                ErrorMessage = response.Message;
                return default;
            }

            return response.data;
        }

        protected string GetUrl(string url)
        {
            return API_URL + url;
        }
    }
}
