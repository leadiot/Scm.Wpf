using Com.Scm.Dto.Bind;
using Com.Scm.Response;
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
        private ScmBindInfo _Info { get; set; }

        /// <summary>
        /// 绑定信息保存文件
        /// </summary>
        private const string INFO_FILE = "info.json";

        public ScmTerminal(string dataDir)
        {
            TokenName = KEY_TOKEN_NAME;
            RemoteUrl = "http://" + SERVER_HOST + "/Api";

            this.DataDir = dataDir;
        }

        public long GetTerminalId()
        {
            return _Info.terminal_id;
        }

        public string GetTerminalCodes()
        {
            return _Info.terminal_codes;
        }

        public bool LoadToken(string file = null)
        {
            _Info = new ScmBindInfo();

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

            _Info = json.AsJsonObject<ScmBindInfo>();
            SetHost(_Info.host);
            _Token = _Info;
            return true;
        }

        private async Task<bool> SaveTokenAsync(string file = null)
        {
            if (string.IsNullOrEmpty(file))
            {
                file = INFO_FILE;
            }

            file = Path.Combine(DataDir, file);
            _Info.host = _Host;
            var json = _Info.ToJsonString();
            return await FileUtils.WriteTextAsync(file, json);
        }

        /// <summary>
        /// 是否将要过期
        /// </summary>
        /// <returns></returns>
        public bool IsExpires()
        {
            return _Info.IsExpires();
        }

        /// <summary>
        /// 是否已经过期
        /// </summary>
        /// <returns></returns>
        public bool IsExpired()
        {
            return _Info.IsExpired();
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
                _Info = data.Adapt<ScmBindInfo>();
                _Info.CalcExpireTime(data.expires);
                _Token = _Info;
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
        public override async void Logout()
        {
            _Info.expires_time = 0;
            _Info.expired_time = 0;
            await SaveTokenAsync();
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
                body["terminal_id"] = _Info.terminal_id.ToString();
                body["access_token"] = _Info.access_token;
                body["refresh_token"] = _Info.refresh_token;

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

                _Info.access_token = data.access_token;
                _Info.refresh_token = data.refresh_token;
                _Info.CalcExpireTime(data.expires);
                _Token = _Info;
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
            var key = _Info.terminal_id + ":" + time + ":" + _Info.access_token;
            var hash = TextUtils.Md5(key);

            var token = _Info.terminal_id + ":" + time + ":" + hash;
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

