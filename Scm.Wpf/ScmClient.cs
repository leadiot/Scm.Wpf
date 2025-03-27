using Com.Scm.Dvo;
using Com.Scm.Sys.Config;
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
        private string _AppKey = "";

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

        private static Dictionary<string, List<ResOptionDvo>> _Dic = new Dictionary<string, List<ResOptionDvo>>();
        private static Dictionary<string, ConfigDto> _Cfg = new Dictionary<string, ConfigDto>();

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<bool> LoginAsync(Dictionary<string, string> body)
        {
            var url = GenUrl("/operator/login");

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

        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public async Task<bool> LoadMenuAsync(string lang = null)
        {
            var url = GenUrl("/operator/authoritymenu");

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
        /// 获取字典
        /// </summary>
        /// <param name="key"></param>
        /// <param name="useCache"></param>
        /// <returns></returns>
        public async Task<List<ResOptionDvo>> ListDicAsync(string key, bool useCache = true)
        {
            if (useCache)
            {
                if (_Dic.ContainsKey(key))
                {
                    return _Dic[key];
                }
            }

            var url = GenUrl("/scmdic/option/" + key);

            var body = new Dictionary<string, string>();
            //body["client"] = "20";

            var head = new Dictionary<string, string>();
            head["Accesstoken"] = _AccessToken;
            head["Appkey"] = _AppKey;

            var response = await HttpUtils.GetObjectAsync<ScmListResponse<ResOptionDvo>>(url, body, head);
            if (response == null)
            {
                return null;
            }
            if (response.Code != 200)
            {
                ErrorMessage = response.GetMessage();
                return null;
            }

            var dic = response.data;
            if (useCache)
            {
                _Dic[key] = dic;
            }
            return dic;
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="useCache"></param>
        /// <returns></returns>
        public async Task<ConfigDto> ListCfgAsync(string key, bool useCache = true)
        {
            if (useCache)
            {
                if (_Cfg.ContainsKey(key))
                {
                    return _Cfg[key];
                }
            }

            var url = GenUrl("/scmcfg/option/" + key);

            var body = new Dictionary<string, string>();
            //body["client"] = "20";

            var head = new Dictionary<string, string>();
            head["Accesstoken"] = _AccessToken;
            head["Appkey"] = _AppKey;

            var response = await HttpUtils.GetObjectAsync<ScmDataResponse<ConfigDto>>(url, body, head);
            if (response == null)
            {
                return null;
            }
            if (response.Code != 200)
            {
                ErrorMessage = response.GetMessage();
                return null;
            }

            var dic = response.data;
            if (useCache)
            {
                _Cfg[key] = dic;
            }
            return dic;
        }

        /// <summary>
        /// POST请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        public async Task<string> PostFormStringAsync(string url, Dictionary<string, string> body = null, Dictionary<string, string> head = null)
        {
            url = GenUrl(url);

            if (head == null)
            {
                head = new Dictionary<string, string>();
            }
            head["Accesstoken"] = _AccessToken;
            head["Appkey"] = _AppKey;

            return await HttpUtils.PostJsonStringAsync(url, body, head);
        }

        /// <summary>
        /// POST请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        public async Task<string> PostJsonStringAsync(string url, string body = null, Dictionary<string, string> head = null)
        {
            url = GenUrl(url);

            if (head == null)
            {
                head = new Dictionary<string, string>();
            }
            head["Accesstoken"] = _AccessToken;
            head["Appkey"] = _AppKey;

            return await HttpUtils.PostJsonStringAsync(url, null, head);
        }

        /// <summary>
        /// POST请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        public async Task<T> PostFormObjectAsync<T>(string url, Dictionary<string, string> body = null, Dictionary<string, string> head = null)
        {
            url = GenUrl(url);

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
        /// POST请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        public async Task<T> PostJsonObjectAsync<T>(string url, string body = null, Dictionary<string, string> head = null)
        {
            url = GenUrl(url);

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
        public async Task<string> GetFormStringAsync(string url, Dictionary<string, string> body = null, Dictionary<string, string> head = null)
        {
            url = GenUrl(url);

            if (head == null)
            {
                head = new Dictionary<string, string>();
            }
            head["Accesstoken"] = _AccessToken;
            head["Appkey"] = _AppKey;

            return await HttpUtils.GetStringAsync(url, body, head);
        }

        /// <summary>
        /// GET请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<T> GetFormObjectAsync<T>(string url, Dictionary<string, string> body = null, Dictionary<string, string> head = null)
        {
            url = GenUrl(url);

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

        protected string GenUrl(string url)
        {
            return API_URL + url;
        }
    }
}
