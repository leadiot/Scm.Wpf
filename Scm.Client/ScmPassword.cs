using Com.Scm.Api;
using Com.Scm.Dto;
using Com.Scm.Dto.Login;
using Com.Scm.Enums;
using Com.Scm.Http.Config;
using Com.Scm.Sys.Menu;
using Com.Scm.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Com.Scm
{
    public class ScmPassword
    {
#if DEBUG
        /// <summary>
        /// 服务地址
        /// </summary>
        public const string SERVER_URL = "https://localhost:5000";
#else
        /// <summary>
        /// 服务地址
        /// </summary>
        public const string SERVER_URL = "https://api.c-scm.net";
#endif

        /// <summary>
        /// 默认授权令牌名称
        /// </summary>
        public const string KEY_TOKEN_NAME = "ApiToken";

        /// <summary>
        /// 当前用户
        /// </summary>
        public ScmUserInfo User { get; private set; }
        /// <summary>
        /// 用户菜单
        /// </summary>
        public List<MenuDto> Menu { get; private set; }

        /// <summary>
        /// 服务器地址
        /// </summary>
        public string ServerUrl { get; private set; } = SERVER_URL;

        /// <summary>
        /// 授权令牌名称
        /// </summary>
        public string TokenName { get; set; } = KEY_TOKEN_NAME;

        /// <summary>
        /// 异常代码
        /// </summary>
        public int ErrorCode { get; private set; }
        /// <summary>
        /// 异常信息
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// HTTP请求对象
        /// </summary>
        private HttpClient _HttpClient;

        /// <summary>
        /// 访问凭据
        /// </summary>
        private string _AccessToken;
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

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<bool> LoginAsync(Dictionary<string, string> body)
        {
            var url = GetApiUrl("/operator/SignIn");

            var json = body?.ToJsonString();
            var response = await HttpUtils.PostJsonObjectAsync<ScmApiDataResponse<LoginResult>>(url, json);
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
            _AccessToken = data.AccessToken;
            User = data.UserInfo;
            return true;
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
        /// 刷新凭证
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<bool> RefreshTokenAsync()
        {
            var url = GetApiUrl("/operator/login");

            var body = new Dictionary<string, string>();
            var json = body?.ToJsonString();
            var response = await HttpUtils.PostJsonObjectAsync<ScmApiDataResponse<LoginResult>>(url, json);
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
            _AccessToken = data.AccessToken;
            User = data.UserInfo;
            return true;
        }

        /// <summary>
        /// 加载菜单
        /// </summary>
        /// <param name="type">终端类型</param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public async Task<bool> LoadMenuAsync(ScmClientTypeEnum type, string lang = null)
        {
            var url = GetApiUrl("/operator/authoritymenu");

            var body = new Dictionary<string, string>();
            body["client"] = "20";
            body["lang"] = lang ?? "zh-cn";

            var head = new Dictionary<string, string>();
            head["Accesstoken"] = _AccessToken;
            head["Appkey"] = _AppKey;

            var response = await HttpUtils.GetObjectAsync<ScmApiListResponse<MenuDto>>(url, body, head);
            if (response == null)
            {
                return false;
            }
            if (response.Code != 200)
            {
                ErrorMessage = response.GetMessage();
                return false;
            }

            Menu = response.Data;
            return true;
        }

        /// <summary>
        /// POST请求（同步）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        public T PostFormObject<T>(string url, Dictionary<string, string> body = null, Dictionary<string, string> head = null)
        {
            url = GetApiUrl(url);

            head = BuildHeader(head);

            var json = body?.ToJsonString();
            var response = HttpUtils.PostJsonObject<ScmApiDataResponse<T>>(url, json, head);
            if (response == null)
            {
                return default;
            }
            if (response.Success)
            {
                ErrorMessage = response.Message;
                return default;
            }

            return response.Data;
        }

        /// <summary>
        /// POST请求（异步）
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        public async Task<T> PostFormObjectAsync<T>(string url, Dictionary<string, string> body = null, Dictionary<string, string> head = null)
        {
            url = GetApiUrl(url);

            head = BuildHeader(head);

            var json = body?.ToJsonString();
            var response = await HttpUtils.PostJsonObjectAsync<ScmApiDataResponse<T>>(url, json, head);
            if (response == null)
            {
                return default;
            }
            if (response.Success)
            {
                ErrorMessage = response.Message;
                return default;
            }

            return response.Data;
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

            head = BuildHeader(head);

            foreach (KeyValuePair<string, string> item in head)
            {
                _HttpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
            }

            var response = await _HttpClient.PostAsync(url, content);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// GET请求（同步）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        public T GetObject<T>(string url, Dictionary<string, string> body = null, Dictionary<string, string> head = null)
        {
            url = GetApiUrl(url);

            head = BuildHeader(head);

            var response = HttpUtils.GetObject<ScmApiDataResponse<T>>(url, body, head);
            if (response == null)
            {
                return default;
            }
            if (response.Success)
            {
                ErrorMessage = response.Message;
                return default;
            }

            return response.Data;
        }

        /// <summary>
        /// GET请求（异步）
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<T> GetObjectAsync<T>(string url, Dictionary<string, string> body = null, Dictionary<string, string> head = null)
        {
            url = GetApiUrl(url);

            head = BuildHeader(head);

            var response = await HttpUtils.GetObjectAsync<ScmApiDataResponse<T>>(url, body, head);
            if (response == null)
            {
                return default;
            }
            if (response.Success)
            {
                ErrorMessage = response.Message;
                return default;
            }

            return response.Data;
        }

        /// <summary>
        /// 获取完整路径
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected string GetApiUrl(string url)
        {
            return SERVER_URL + "/Api" + url;
        }

        protected Dictionary<string, string> BuildHeader(Dictionary<string, string> headers)
        {
            if (headers == null)
            {
                headers = new Dictionary<string, string>();
            }
            headers["ApiToken"] = _AccessToken;
            headers["Appkey"] = _AppKey;

            return headers;
        }

        /// <summary>
        /// HTTPS+代理配置（复用企业级逻辑）
        /// </summary>
        /// <param name="timeout">过期时间（分钟）</param>
        /// <param name="certConfig"></param>
        /// <param name="proxyConfig"></param>
        public void CreateHttpClient(int timeout = 5, CertConfig certConfig = null, ProxyConfig proxyConfig = null)
        {
            var handler = new HttpClientHandler();

            // 1. HTTPS证书配置
            if (certConfig != null)
            {
                if (!string.IsNullOrWhiteSpace(certConfig.ClientCertPath) && File.Exists(certConfig.ClientCertPath))
                {
                    var clientCert = new X509Certificate2(
                        certConfig.ClientCertPath, certConfig.ClientCertPassword,
                        X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
                    handler.ClientCertificates.Add(clientCert);
                }
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                {
                    if (certConfig.AllowAnyCert) return true;
                    if (!string.IsNullOrWhiteSpace(certConfig.TrustedCertThumbprint))
                    {
                        string certThumbprint = cert.Thumbprint?.ToUpperInvariant();
                        string trustedThumbprint = certConfig.TrustedCertThumbprint.ToUpperInvariant();
                        return certThumbprint == trustedThumbprint;
                    }
                    return sslPolicyErrors == SslPolicyErrors.None;
                };
            }

            // 2. 代理配置
            if (proxyConfig != null && !string.IsNullOrWhiteSpace(proxyConfig.ProxyUrl))
            {
                var proxyUri = new Uri(proxyConfig.ProxyUrl);
                handler.Proxy = string.IsNullOrWhiteSpace(proxyConfig.ProxyUsername)
                    ? new WebProxy(proxyUri, proxyConfig.BypassLocal)
                    : new WebProxy(proxyUri, proxyConfig.BypassLocal)
                    {
                        Credentials = new NetworkCredential(proxyConfig.ProxyUsername, proxyConfig.ProxyPassword)
                    };
                handler.UseProxy = true;
            }

            // 3. 初始化HttpClient
            _HttpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromMinutes(timeout),
                DefaultRequestHeaders = { { "User-Agent", "Super-Uploader/1.0" } }
            };
        }
    }
}
