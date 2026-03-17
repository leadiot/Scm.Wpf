using Com.Scm.Api;
using Com.Scm.Dto.Bind;
using Com.Scm.Utils;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Com.Scm
{
    public class ScmTerminal : ScmClient
    {
        /// <summary>
        /// 默认授权令牌名称
        /// </summary>
        public const string KEY_TOKEN_NAME = "AppToken";

        /// <summary>
        /// 服务端授权信息
        /// </summary>
        private ScmBindInfo _Token { get; set; }

        /// <summary>
        /// 绑定信息保存文件
        /// </summary>
        private const string INFO_FILE = "info.json";

        public ScmTerminal()
        {
            TokenName = KEY_TOKEN_NAME;
            RemoteUrl = "http://" + SERVER_HOST + "/Api";
        }

        public long GetTerminalId()
        {
            return _Token.terminal_id;
        }

        public string GetTerminalCodes()
        {
            return _Token.terminal_codes;
        }

        public bool LoadToken(string file = null)
        {
            _Token = new ScmBindInfo();

            if (string.IsNullOrEmpty(file))
            {
                file = INFO_FILE;
            }

            file = Path.Combine(DataDir, file);

            if (!File.Exists(file))
            {
                return false;
            }
            var json = FileUtils.ReadText(file);
            if (string.IsNullOrEmpty(json))
            {
                return false;
            }

            _Token = json.AsJsonObject<ScmBindInfo>();
            SetHost(_Token.host);
            return true;
        }

        private async Task<bool> SaveTokenAsync(string file = null)
        {
            if (string.IsNullOrEmpty(file))
            {
                file = INFO_FILE;
            }

            file = Path.Combine(DataDir, file);
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
                _Token = data.Adapt<ScmBindInfo>();
                _Token.CalcExpireTime(data.expires);
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
            var url = GetApiUrl("/Terminal/Refresh");

            try
            {
                var body = new Dictionary<string, string>();
                body["terminal_id"] = _Token.terminal_id.ToString();
                body["access_token"] = _Token.access_token;
                body["refresh_token"] = _Token.refresh_token;

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
                _Token.CalcExpireTime(data.expires);
                await SaveTokenAsync();

                return true;
            }
            catch (HttpRequestException ex)
            {
                IsConnecting = false;
                LogUtils.Error(ex);
                throw;
            }
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

        protected override Dictionary<string, string> BuildHeader(Dictionary<string, string> header)
        {
            if (header == null)
            {
                header = new Dictionary<string, string>();
            }
            header[TokenName] = GetBasicToken();

            return header;
        }
    }
}

