using Com.Scm.Api;
using Com.Scm.Dto;
using Com.Scm.Enums;
using Com.Scm.Http.Config;
using Com.Scm.Utils;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Com.Scm
{
    public class ScmTerminal
    {
        /// <summary>
        /// 服务地址
        /// </summary>
        public const string SERVER_HOST = "api.c-scm.net";

        /// <summary>
        /// 默认授权令牌名称
        /// </summary>
        public const string KEY_TOKEN_NAME = "AppToken";

        /// <summary>
        /// 服务器是否为连通状态
        /// </summary>
        public static bool IsConnecting { get; private set; } = true;

        /// <summary>
        /// 主机地址
        /// </summary>
        private string _Host;

        /// <summary>
        /// 服务器地址
        /// </summary>
        public string ServerUrl { get; private set; }

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
        /// 服务端授权信息
        /// </summary>
        private ScmTerminalInfo _Token { get; set; }

        /// <summary>
        /// 绑定信息保存文件
        /// </summary>
        private const string INFO_FILE = "info.json";

        public ScmTerminal()
        {
        }

        public long GetTerminalId()
        {
            return _Token.terminal_id;
        }

        public string GetTerminalCodes()
        {
            return _Token.terminal_codes;
        }

        public void Init(string host)
        {
            _Host = host ?? SERVER_HOST;
            ServerUrl = "http://" + _Host;
        }

        public bool LoadToken(string file = null)
        {
            _Token = new ScmTerminalInfo();

            if (string.IsNullOrEmpty(file))
            {
                file = INFO_FILE;
            }

            file = Path.Combine(ScmClientEnv.DataDir, file);

            if (!File.Exists(file))
            {
                return false;
            }
            var json = FileUtils.ReadText(file);
            if (string.IsNullOrEmpty(json))
            {
                return false;
            }

            _Token = json.AsJsonObject<ScmTerminalInfo>();
            Init(_Token.host);
            return true;
        }

        private async Task<bool> SaveTokenAsync(string file = null)
        {
            if (string.IsNullOrEmpty(file))
            {
                file = INFO_FILE;
            }

            file = Path.Combine(ScmClientEnv.DataDir, file);
            _Token.host = _Host;
            var json = _Token.ToJsonString();
            return await FileUtils.WriteTextAsync(file, json);
        }

        /// <summary>
        /// 是否将要过期
        /// </summary>
        /// <returns></returns>
        public bool IsExpires()
        {
            return _Token.IsExpires();
        }

        /// <summary>
        /// 是否已经过期
        /// </summary>
        /// <returns></returns>
        public bool IsExpired()
        {
            return _Token.IsExpired();
        }

        /// <summary>
        /// 设备绑定
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<bool> BindAsync(Dictionary<string, string> body)
        {
            var url = GetApiUrl("/Terminal/Bind");

            try
            {
                var json = body?.ToJsonString();
                var response = await HttpUtils.PostJsonObjectAsync<ScmApiDataResponse<BindResult>>(url, json);
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
                _Token = data.Adapt<ScmTerminalInfo>();
                _Token.CalcExpireTime(data.expires_in);
                await SaveTokenAsync();

                IsConnecting = true;
                return true;
            }
            catch (HttpRequestException ex)
            {
                IsConnecting = false;
                LogUtils.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Logout()
        {
            _Token.expires_time = 0;
            _Token.expired_time = 0;
            await SaveTokenAsync();
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

            try
            {
                var body = new Dictionary<string, string>();
                var json = body?.ToJsonString();
                var response = await HttpUtils.PostJsonObjectAsync<ScmApiDataResponse<BindResult>>(url, json);
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
                if (data == null)
                {
                    return false;
                }

                _Token.access_token = data.access_token;
                _Token.refresh_token = data.refresh_token;
                _Token.CalcExpireTime(data.expires_in);

                return true;
            }
            catch (HttpRequestException ex)
            {
                IsConnecting = false;
                LogUtils.Error(ex);
                throw;
            }
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
                LogUtils.Error(ex);
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
                LogUtils.Error(ex);
                throw;
            }
        }
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
                LogUtils.Error(ex);
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
                LogUtils.Error(ex);
                throw;
            }
        }

        public string UploadFile(string url, string filePath, string fileName, Dictionary<string, string> body = null, Dictionary<string, string> header = null)
        {
            url = GetApiUrl(url);

            header = BuildHeader(header);

            try
            {
                return HttpUtils.UploadFile(url, filePath, fileName, "file", body, header);
            }
            catch (HttpRequestException ex)
            {
                IsConnecting = false;
                LogUtils.Error(ex);
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
                LogUtils.Error(ex);
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
                LogUtils.Error(ex);
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
                LogUtils.Error(ex);
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
                LogUtils.Error(ex);
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
                LogUtils.Error(ex);
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
                LogUtils.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// 获取完整路径
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetApiUrl(string url)
        {
            return ServerUrl + "/Api" + url;
        }

        private string GetBasicToken()
        {
            var time = TimeUtils.GetUnixTime(true);
            var key = _Token.terminal_id + ":" + time + ":" + _Token.access_token;
            var hash = TextUtils.Md5(key);

            var token = _Token.terminal_id + ":" + time + ":" + hash;
            var bytes = System.Text.Encoding.UTF8.GetBytes(token);
            //return Basic(Convert.ToBase64String(bytes));
            return HttpUtils.ToBase64String(bytes);
        }

        private Dictionary<string, string> BuildHeader(Dictionary<string, string> header)
        {
            if (header == null)
            {
                header = new Dictionary<string, string>();
            }
            header[TokenName] = GetBasicToken();

            return header;
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
    }
}

