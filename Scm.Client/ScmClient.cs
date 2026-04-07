using Com.Scm.Enums;
using Com.Scm.Http.Config;
using Com.Scm.Response;
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
    public abstract class ScmClient
    {
#if DEBUG
        /// <summary>
        /// 服务地址
        /// </summary>
        public const string REMOTE_HOST = "localhost:5000";
#else
        /// <summary>
        /// 服务地址
        /// </summary>
        public const string REMOTE_HOST = "api.c-scm.net";
#endif

        /// <summary>
        /// 服务地址
        /// </summary>
        public const string SERVER_HOST = "api.c-scm.net";

        /// <summary>
        /// 服务器是否为连通状态
        /// </summary>
        public static bool IsConnecting { get; protected set; } = true;

        /// <summary>
        /// 服务器地址
        /// </summary>
        public string RemoteUrl { get; protected set; }

        /// <summary>
        /// 授权令牌名称
        /// </summary>
        public string TokenName { get; protected set; }

        protected ScmToken _Token { get; set; }

        /// <summary>
        /// 用户菜单
        /// </summary>
        public List<MenuDto> Menu { get; protected set; }

        /// <summary>
        /// 主机地址
        /// </summary>
        protected string _Host;

        /// <summary>
        /// HTTP请求对象
        /// </summary>
        private HttpClient _HttpClient;

        public void SetHost(string host)
        {
            _Host = host ?? SERVER_HOST;
            RemoteUrl = "http://" + _Host + "/Api";
        }

        /// <summary>
        /// 获取完整路径
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetApiUrl(string url)
        {
            return RemoteUrl + url;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetDataUrl(string url)
        {
            return "http://" + _Host + "/Data" + url;
        }

        public string GetAvatar(string avatar)
        {
            return "http://" + _Host + "/Data/Avatar/" + avatar;
        }

        /// <summary>
        /// 本地数据目录
        /// </summary>
        public string DataDir { get; protected set; }

        /// <summary>
        /// 异常代码
        /// </summary>
        public int ErrorCode { get; protected set; }
        /// <summary>
        /// 异常信息
        /// </summary>
        public string ErrorMessage { get; protected set; }

        /// <summary>
        /// 加载菜单
        /// </summary>
        /// <param name="type">终端类型</param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public async Task<List<MenuDto>> LoadMenuAsync(ScmClientTypeEnum type, string lang = null)
        {
            var url = GetApiUrl("/operator/authoritymenu");

            var body = new Dictionary<string, string>();
            body["client"] = "20";
            body["lang"] = lang ?? "zh-cn";

            var head = new Dictionary<string, string>();
            head[TokenName] = _Token.GetAccessToken();
            head["Appkey"] = "";

            var response = await HttpUtils.GetObjectAsync<ScmApiListResponse<MenuDto>>(url, body, head);
            if (response == null)
            {
                return null;
            }
            if (response.Code != 200)
            {
                ErrorMessage = response.GetMessage();
                return null;
            }

            return response.Data;
        }

        /// <summary>
        /// 获取应用信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ScmAppInfo GetAppInfo(string code)
        {
            var url = $"http://{SERVER_HOST}/api/ScmInfo/App?code={code}";

            var response = HttpUtils.GetObject<ScmApiDataResponse<ScmAppInfo>>(url);
            if (response != null && response.Success)
            {
                return response.Data;
            }

            return null;
        }

        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ScmVerInfo GetVerInfo(string code)
        {
            var url = $"http://{SERVER_HOST}/api/ScmInfo/Ver?code={code}&client={ScmClientTypeEnum.Windows}";

            var response = HttpUtils.GetObject<ScmApiDataResponse<ScmVerInfo>>(url);
            if (response != null && response.Success)
            {
                return response.Data;
            }

            return null;
        }

        protected abstract Dictionary<string, string> BuildHeader(Dictionary<string, string> header);

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

            try
            {
                var response = HttpUtils.PostJsonObject<ScmApiDataResponse<T>>(url, body.ToJsonString(), head);
                if (response == null)
                {
                    return default;
                }
                if (!response.Success)
                {
                    ErrorMessage = response.Message;
                    return default;
                }

                return response.Data;
            }
            catch (HttpRequestException ex)
            {
                IsConnecting = false;
                Error(ex);
                throw;
            }
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

            try
            {
                var json = body?.ToJsonString();
                var response = await HttpUtils.PostJsonObjectAsync<ScmApiDataResponse<T>>(url, json, head);
                if (response == null)
                {
                    return default;
                }
                if (!response.Success)
                {
                    ErrorMessage = response.Message;
                    return default;
                }

                return response.Data;
            }
            catch (HttpRequestException ex)
            {
                IsConnecting = false;
                Error(ex);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="json"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        public T PostJsonObject<T>(string url, string json, Dictionary<string, string> head = null)
        {
            url = GetApiUrl(url);

            head = BuildHeader(head);

            try
            {
                var response = HttpUtils.PostJsonObject<ScmApiDataResponse<T>>(url, json, head);
                if (response == null)
                {
                    return default;
                }
                if (!response.Success)
                {
                    ErrorMessage = response.Message;
                    return default;
                }

                return response.Data;
            }
            catch (HttpRequestException ex)
            {
                IsConnecting = false;
                Error(ex);
                throw;
            }
        }

        /// <summary>
        /// POST请求（异步）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="json"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        public async Task<T> PostJsonObjectAsync<T>(string url, string json, Dictionary<string, string> head = null)
        {
            url = GetApiUrl(url);

            head = BuildHeader(head);

            try
            {
                var response = await HttpUtils.PostJsonObjectAsync<ScmApiDataResponse<T>>(url, json, head);
                if (response == null)
                {
                    return default;
                }
                if (!response.Success)
                {
                    ErrorMessage = response.Message;
                    return default;
                }

                return response.Data;
            }
            catch (HttpRequestException ex)
            {
                IsConnecting = false;
                Error(ex);
                throw;
            }
        }

        public bool UploadFile(string url, string filePath, string fileName, Dictionary<string, string> body = null, Dictionary<string, string> header = null)
        {
            url = GetApiUrl(url);

            header = BuildHeader(header);

            try
            {
                var result = HttpUtils.UploadFile(url, filePath, fileName, "file", body, header);
                var response = result.AsJsonObject<ScmApiDataResponse<bool>>();
                if (response == null)
                {
                    return default;
                }
                if (!response.Success)
                {
                    ErrorMessage = response.Message;
                    return default;
                }

                return response.Data;
            }
            catch (HttpRequestException ex)
            {
                IsConnecting = false;
                Error(ex);
                throw;
            }
        }

        public async Task<string> PostObjectAsync(string url, HttpContent content, Dictionary<string, string> head = null)
        {
            url = GetApiUrl(url);

            if (_HttpClient == null)
            {
                CreateHttpClient();
            }

            head = BuildHeader(head);

            foreach (KeyValuePair<string, string> item in head)
            {
                _HttpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
            }

            try
            {
                var response = await _HttpClient.PostAsync(url, content);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                IsConnecting = false;
                Error(ex);
                throw;
            }
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

            try
            {
                var response = await HttpUtils.GetObjectAsync<ScmApiDataResponse<T>>(url, body, head);
                if (response == null)
                {
                    return default;
                }
                if (!response.Success)
                {
                    ErrorMessage = response.Message;
                    return default;
                }

                return response.Data;
            }
            catch (HttpRequestException ex)
            {
                IsConnecting = false;
                Error(ex);
                throw;
            }
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

            try
            {
                var response = HttpUtils.GetObject<ScmApiDataResponse<T>>(url, body, head);
                if (response == null)
                {
                    return default;
                }
                if (!response.Success)
                {
                    ErrorMessage = response.Message;
                    return default;
                }

                return response.Data;
            }
            catch (HttpRequestException ex)
            {
                IsConnecting = false;
                Error(ex);
                throw;
            }
        }

        /// <summary>
        /// 心跳服务（同步）
        /// 检测设备是否在线
        /// </summary>
        /// <returns></returns>
        public bool Echo(string msg = "check")
        {
            var url = "/Hb/Echo";
            if (string.IsNullOrEmpty(msg))
            {
                url += "?msg=" + msg;
            }

            try
            {
                ScmApiResponse response = GetObject<ScmApiResponse>(url);

                IsConnecting = response.IsSuccess();
                return IsConnecting;
            }
            catch (HttpRequestException ex)
            {
                IsConnecting = false;
                Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 心跳服务（异步）
        /// 检测设备是否在线
        /// </summary>
        /// <param name="method"></param>
        /// <param name="timeoutMs"></param>
        /// <returns></returns>
        public async Task<bool> EchoAsync(string msg)
        {
            var url = "/Hb/Echo";
            if (string.IsNullOrEmpty(msg))
            {
                url += "?msg=" + msg;
            }

            try
            {
                ScmApiResponse response = await GetObjectAsync<ScmApiResponse>(url);

                IsConnecting = response.IsSuccess();
                return IsConnecting;
            }
            catch (HttpRequestException ex)
            {
                IsConnecting = false;
                Error(ex);
                return false;
            }
        }

        public bool DownloadFile(string url, string nativeFile)
        {
            url = GetApiUrl(url);

            try
            {
                return HttpUtils.DownloadFile(url, nativeFile);
            }
            catch (HttpRequestException ex)
            {
                IsConnecting = false;
                Error(ex);
                throw;
            }
        }

        /// <summary>
        /// HTTPS+代理配置（复用企业级逻辑）
        /// </summary>
        /// <param name="timeout">过期时间（分钟）</param>
        /// <param name="certConfig"></param>
        /// <param name="proxyConfig"></param>
        protected void CreateHttpClient(int timeout = 5, CertConfig certConfig = null, ProxyConfig proxyConfig = null)
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

        public ScmToken GetToken()
        {
            return _Token;
        }

        public virtual void Logout()
        {
            _Token = null;
        }

        public void Error(Exception exp)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Error: {exp.Message}");
        }
    }
}
