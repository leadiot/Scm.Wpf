using Com.Scm.Dto.Login;
using Com.Scm.Response;
using Com.Scm.Utils;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Com.Scm
{
    public class ScmOperator : ScmClient
    {
        /// <summary>
        /// HTTP请求对象
        /// </summary>
        private HttpClient _HttpClient;
        /// <summary>
        /// 应用代码
        /// </summary>
        private string _AppKey;
        /// <summary>
        /// 临近过期时间
        /// </summary>
        private long _ExpiresTime;
        /// <summary>
        /// 确认过期时间
        /// </summary>
        private long _ExpiredTime;

        public ScmOperator(string dataDir)
        {
            _Host = REMOTE_HOST;
            RemoteUrl = "http://" + _Host + "/Api";

            TokenName = TOKEN_NAME;

            this.DataDir = dataDir;
        }

        /// <summary>
        /// 是否将要过期
        /// </summary>
        /// <returns></returns>
        public bool IsExpires()
        {
            return TimeUtils.GetUnixTime() > _ExpiresTime;
        }

        /// <summary>
        /// 是否已经过期
        /// </summary>
        /// <returns></returns>
        public bool IsExpired()
        {
            return TimeUtils.GetUnixTime() > _ExpiredTime;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<bool> SignInAsync(Dictionary<string, string> body)
        {
            var url = GetApiUrl("/Operator/SignIn");

            var json = body?.ToJsonString();
            var response = await HttpUtils.PostJsonObjectAsync<ScmApiDataResponse<AuthResult>>(url, json);
            if (response == null)
            {
                return false;
            }
            if (response.Code != 200)
            {
                ErrorMessage = response.GetMessage();
                return false;
            }

            var data = response.Data;
            if (!data.IsSuccess())
            {
                ErrorMessage = data.GetMessage();
                return false;
            }

            data.UserInfo.AccessToken = data.AccessToken;
            _Token = data.UserInfo;
            return true;
        }

        /// <summary>
        /// 刷新凭证
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<bool> RefreshTokenAsync()
        {
            var url = GetApiUrl("/operator/login");

            var body = new Dictionary<string, string>();
            var json = body?.ToJsonString();
            var response = await HttpUtils.PostJsonObjectAsync<ScmApiDataResponse<AuthResult>>(url, json);
            if (response == null)
            {
                return false;
            }
            if (response.Code != 200)
            {
                ErrorMessage = response.GetMessage();
                return false;
            }

            var data = response.Data;
            if (!data.IsSuccess())
            {
                ErrorMessage = data.GetMessage();
                return false;
            }

            data.UserInfo.AccessToken = data.AccessToken;
            _Token = data.UserInfo;
            return true;
        }

        /// <summary>
        /// Form表单提交
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        public async Task<string> PostAsync(string url, HttpContent content, Dictionary<string, string> head = null)
        {
            url = GetApiUrl(url);

            var headers = BuildHeader(head);

            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Content = content;
                foreach (KeyValuePair<string, string> item in headers)
                {
                    request.Headers.TryAddWithoutValidation(item.Key, item.Value);
                }

                var response = await _HttpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        protected override Dictionary<string, string> BuildHeader(Dictionary<string, string> headers)
        {
            if (headers == null)
            {
                headers = new Dictionary<string, string>();
            }
            headers[TokenName] = _Token.GetAccessToken();
            headers["Appkey"] = _AppKey;

            return headers;
        }
    }
}
