using Com.Scm.Exceptions;
using Com.Scm.Oidc.Response;
using Com.Scm.Response;
using Com.Scm.Utils;
using System.Text.RegularExpressions;

namespace Com.Scm.Oidc
{
    /// <summary>
    /// OIDC客户端
    /// </summary>
    public class OidcClient
    {
        #region 常量
        public const string PARAM_KEY_CLIENT_ID = "client_id";
        public const string PARAM_KEY_CLIENT_SECRET = "client_secret";
        public const string PARAM_KEY_RESPONSE_TYPE = "response_type";
        public const string PARAM_KEY_REDIRECT_URI = "redirect_uri";
        public const string PARAM_KEY_STATE = "state";
        public const string PARAM_KEY_SCOPE = "scope";
        public const string PARAM_KEY_CODE_CHALLENGE = "code_challenge";
        public const string PARAM_KEY_CODE_CHALLENGE_METHOD = "code_challenge_method";
        public const string PARAM_KEY_CODE_VERIFIER = "code_verifier";

        public const string PARAM_KEY_GRANT_TYPE = "grant_type";
        public const string PARAM_KEY_CODE = "code";
        public const string PARAM_KEY_REFRESH_TOKEN = "refresh_token";
        public const string PARAM_KEY_AUTHORIZATION = "Authorization";

        public const string PARAM_KEY_TICKET = "ticket";
        public const string PARAM_KEY_DIGEST = "digest";
        public const string PARAM_KEY_REQUEST_ID = "request_id";

        public const string PARAM_KEY_NONCE = "nonce";

        /// <summary>
        /// 服务端路径
        /// </summary>
        public const string BASE_URL = "http://www.oidc.org.cn";

        /// <summary>
        /// 数据路径
        /// </summary>
        public const string DATA_URL = BASE_URL + "/data";
        /// <summary>
        /// 授权路径
        /// </summary>
        public const string OAUTH_URL = BASE_URL + "/oauth";
        #endregion

        /// <summary>
        /// 类库版本
        /// </summary>
        public const string Ver = "1.1.0";

        /// <summary>
        /// OIDC配置
        /// </summary>
        private OidcConfig _Config;
        /// <summary>
        /// PKCE对象
        /// </summary>
        private PkceObject _Object;

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="config"></param>
        public OidcClient(OidcConfig config)
        {
            if (config == null)
            {
                config = new OidcConfig();
                config.UseDemo();
            }

            _Config = config;
            _Object = new PkceObject();
        }

        #region 公共方法
        /// <summary>
        /// 获取所有服务商
        /// </summary>
        /// <returns></returns>
        public async Task<List<OidcOspInfo>> ListAllOspAsync()
        {
            var nonce = GenNonce();

            var url = GenAuthUrl("/ListOsp/0");
            url += "?nonce=" + nonce;
            url += "&sign=" + GenSign(nonce);

            var response = await HttpUtils.GetObjectAsync<ListOspResponse>(url);
            if (response == null || !response.IsSuccess())
            {
                return null;
            }

            return response.Data;
        }

        /// <summary>
        /// 获取应用服务商
        /// </summary>
        /// <returns></returns>
        public async Task<List<OidcOspInfo>> ListAppOspAsync()
        {
            var nonce = GenNonce();

            var url = GenAuthUrl("/ListOsp/" + _Config.AppKey);
            url += "?nonce=" + nonce;
            url += "&sign=" + GenSign(nonce);

            var response = await HttpUtils.GetObjectAsync<ListOspResponse>(url);
            if (response == null || !response.IsSuccess())
            {
                return null;
            }

            return response.Data;
        }

        /// <summary>
        /// 获取OIDC标准网页登录地址
        /// </summary>
        /// <param name="responseType">响应方式</param>
        /// <param name=PARAM_KEY_STATE>发起方自定义参数，此参数在回调时进行回传</param>
        /// <returns></returns>
        public string GetWebUrl(string responseType, string state = null)
        {
            if (responseType == null)
            {
                throw new OidcException("response_type 不能为空！");
            }

            var url = GenBaseUrl("/Web/Login");

            var val = new Dictionary<string, string>()
            {
                {PARAM_KEY_CLIENT_ID, _Config.AppKey},
                {PARAM_KEY_RESPONSE_TYPE, responseType},
                {PARAM_KEY_STATE, state},
            };

            return HttpUtils.BuildUrl(val, url);
        }
        #endregion

        #region OAuth登录
        #region 服务端模式
        /// <summary>
        /// 引导授权，适用于服务端
        /// </summary>
        /// <param name="state">发起方自定义参数，此参数在回调时进行回传</param>
        /// <param name="scope">授权范围，可以为空</param>
        /// <returns></returns>
        public string GetAuthorizeAUrl(string state = null, string scope = null)
        {
            var url = GenAuthUrl("/AuthorizeA");

            _Object.Generate();

            var val = new Dictionary<string, string>()
            {
                [PARAM_KEY_RESPONSE_TYPE] = "code",
                [PARAM_KEY_REDIRECT_URI] = _Config.RedirectUrl,
                [PARAM_KEY_STATE] = state,
                [PARAM_KEY_SCOPE] = scope,
                [PARAM_KEY_CODE_CHALLENGE] = _Object.code_challenge,
                [PARAM_KEY_CODE_CHALLENGE_METHOD] = _Object.code_challenge_method
            };

            return HttpUtils.BuildUrl(val, url);
        }

        /// <summary>
        /// 换取令牌
        /// </summary>
        /// <param name="code">服务端回调时传递的参数</param>
        /// <returns></returns>
        public async Task<AccessTokenResponse> AccessTokenAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new OidcException("code 不能为空！");
            }

            var url = GenAuthUrl("/Token");

            var body = new Dictionary<string, string>()
            {
                [PARAM_KEY_GRANT_TYPE] = "authorization_code",
                [PARAM_KEY_CODE] = code,
                [PARAM_KEY_CLIENT_ID] = _Config.AppKey,
                [PARAM_KEY_CLIENT_SECRET] = _Config.AppSecret,
                [PARAM_KEY_REDIRECT_URI] = _Config.RedirectUrl,
                [PARAM_KEY_CODE_VERIFIER] = _Object.code_verifier
            };

            return await HttpUtils.PostFormObjectAsync<AccessTokenResponse>(url, body, null);
        }
        #endregion

        #region 客户端模式
        /// <summary>
        /// 握手
        /// </summary>
        /// <param name="state">回调参数，可以为空</param>
        /// <returns></returns>
        public async Task<HandshakeResponse> HandshakeAsync(string state)
        {
            var url = GenAuthUrl("/Handshake");

            var body = new Dictionary<string, string>()
            {
                [PARAM_KEY_RESPONSE_TYPE] = "token",
                [PARAM_KEY_CLIENT_ID] = _Config.AppKey,
                [PARAM_KEY_REDIRECT_URI] = "",
                [PARAM_KEY_STATE] = state,
                [PARAM_KEY_SCOPE] = "",
                [PARAM_KEY_REQUEST_ID] = TextUtils.TimeString()
            };

            return await HttpUtils.GetObjectAsync<HandshakeResponse>(url, body, null);
        }

        /// <summary>
        /// 侦听
        /// </summary>
        /// <param name="ticket">服务交互凭证</param>
        /// <returns></returns>
        public async Task<ListenResponse> ListenAsync(TicketInfo ticket)
        {
            if (ticket == null)
            {
                throw new OidcException("ticket 不能为空！");
            }

            var url = GenAuthUrl("/Listen");

            var body = new Dictionary<string, string>()
            {
                [PARAM_KEY_CLIENT_ID] = _Config.AppKey,
                [PARAM_KEY_TICKET] = ticket.Code,
                [PARAM_KEY_DIGEST] = ticket.GetDigest()
            };

            return await HttpUtils.GetObjectAsync<ListenResponse>(url, body, null);
        }
        #endregion

        #endregion

        /// <summary>
        /// 刷新令牌
        /// </summary>
        /// <param name="accessToken">访问令牌</param>
        /// <param name="refreshToken">刷新令牌</param>
        /// <returns></returns>
        public async Task<RefreshTokenResponse> RefreshTokenAsync(string accessToken, string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new OidcException("accessToken 不能为空！");
            }

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new OidcException("refreshToken 不能为空！");
            }

            var url = GenAuthUrl("/Token");

            var header = new Dictionary<string, string>()
            {
                [PARAM_KEY_AUTHORIZATION] = accessToken,
            };

            var body = new Dictionary<string, string>()
            {
                [PARAM_KEY_GRANT_TYPE] = "refresh_token",
                [PARAM_KEY_REFRESH_TOKEN] = refreshToken,
                [PARAM_KEY_CLIENT_ID] = _Config.AppKey,
                [PARAM_KEY_CLIENT_SECRET] = _Config.AppSecret,
                [PARAM_KEY_REDIRECT_URI] = _Config.RedirectUrl
            };

            return await HttpUtils.PostFormObjectAsync<RefreshTokenResponse>(url, body, header);
        }

        #region 代码调用登录
        /// <summary>
        /// 引导授权，适用于客户端
        /// 此方法适用于不指定授权服务商的情况
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public string GetAuthorizeBUrl(string ticket)
        {
            if (!IsTicket(ticket))
            {
                throw new OidcException("无效的 ticket！");
            }

            var url = GenAuthUrl("/AuthorizeB");
            url += $"?{PARAM_KEY_TICKET}={ticket}";

            return url;
        }

        /// <summary>
        /// 执行登录（适用于服务端）
        /// </summary>
        /// <param name="ospCode">服务商代码</param>
        /// <param name="responseType">响应方式</param>
        /// <param name="redirectUri">回调地址</param>
        /// <param name="state">回调参数</param>
        /// <param name="scope">权限范围</param>
        /// <returns></returns>
        public string GetLoginAUrl(string ospCode, string responseType, string redirectUri, string state = null, string scope = null)
        {
            if (string.IsNullOrWhiteSpace(ospCode))
            {
                throw new OidcException("无效的 ospCode！");
            }

            var url = GenAuthUrl("/LoginB/" + ospCode);
            var val = new Dictionary<string, string>()
            {
                { PARAM_KEY_CLIENT_ID, _Config.AppKey},
                { PARAM_KEY_RESPONSE_TYPE, responseType},
                { PARAM_KEY_REDIRECT_URI, redirectUri},
                { PARAM_KEY_STATE, state},
                { PARAM_KEY_SCOPE, scope}
            };

            return HttpUtils.BuildUrl(val, url);
        }

        /// <summary>
        /// 执行登录（适用于客户端）
        /// </summary>
        /// <param name="ospCode">服务商代码</param>
        /// <param name="ticket">访问票据</param>
        /// <returns></returns>
        public string GetLoginBUrl(string ospCode, string ticket)
        {
            if (string.IsNullOrWhiteSpace(ospCode))
            {
                throw new OidcException("无效的 ospCode！");
            }
            if (!IsTicket(ticket))
            {
                throw new OidcException("无效的 ticket！");
            }

            var url = GenAuthUrl("/LoginB/" + ospCode);
            url += $"?{PARAM_KEY_TICKET}={ticket}";

            return url;
        }

        /// <summary>
        /// 根据服务商返回不同的授权路径，适用于客户端
        /// </summary>
        /// <param name="osp"></param>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public string GetOAuthUrl(OidcOspInfo osp, string ticket)
        {
            if (osp == null || osp.IsMore())
            {
                return GetAuthorizeBUrl(ticket);
            }


            return GetLoginBUrl(osp.Code, ticket);
        }
        #endregion

        #region 验证码登录
        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="type">验证码类型</param>
        /// <param name="code">验证码接收地址（邮件或手机）</param>
        /// <param name="requestId">请求ID，用于防重复提交</param>
        /// <returns></returns>
        public async Task<SendSmsResponse> SendSmsAsync(OidcSmsEnums type, string code, string requestId = null)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new OidcException("code 不能为空！");
            }

            if (string.IsNullOrWhiteSpace(requestId))
            {
                requestId = TextUtils.TimeString();
            }

            var url = GenAuthUrl("/SendSms");
            var val = new Dictionary<string, string>()
            {
                ["type"] = type.ToString(),
                ["code"] = code,
                ["request_id"] = requestId
            };

            return await HttpUtils.GetObjectAsync<SendSmsResponse>(HttpUtils.BuildUrl(val, url));
        }

        /// <summary>
        /// 校验验证码，适用于服务端
        /// </summary>
        /// <param name="key">发送验证码时，服务端返回的Key</param>
        /// <param name="sms">验证码</param>
        /// <returns></returns>
        public async Task<VerifySmsResponse> VerifySmsAAsync(string key, string sms)
        {
            if (!IsTicket(key))
            {
                throw new OidcException("无效的 key！");
            }
            if (!IsSmsCode(sms))
            {
                throw new OidcException("无效的 sms！");
            }

            var url = GenAuthUrl("/VerifySmsA");
            var body = new Dictionary<string, string>()
            {
                [PARAM_KEY_CLIENT_ID] = _Config.AppKey,
                ["key"] = key,
                ["sms"] = sms
            };

            return await HttpUtils.PostFormObjectAsync<VerifySmsResponse>(url, body);
        }

        /// <summary>
        /// 校验验证码，适用于客户端
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="key"></param>
        /// <param name="sms"></param>
        /// <returns></returns>
        public async Task<VerifySmsResponse> VerifySmsBAsync(string ticket, string key, string sms)
        {
            if (!IsTicket(ticket))
            {
                throw new OidcException("无效的 ticket！");
            }

            if (!IsTicket(key))
            {
                throw new OidcException("无效的 key！");
            }
            if (!IsSmsCode(sms))
            {
                throw new OidcException("无效的 sms！");
            }

            var url = GenAuthUrl("/VerifySmsA");
            var body = new Dictionary<string, string>()
            {
                [PARAM_KEY_TICKET] = ticket,
                ["key"] = key,
                ["sms"] = sms
            };

            return await HttpUtils.PostFormObjectAsync<VerifySmsResponse>(url, body);
        }
        #endregion

        #region 获取用户信息
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="accessToken">访问令牌</param>
        /// <returns></returns>
        public async Task<UserInfoResponse> GetUserInfoAsync(string accessToken)
        {
            var url = GenAuthUrl("/User");
            url += "?token=" + accessToken;

            return await HttpUtils.GetObjectAsync<UserInfoResponse>(url);
        }
        #endregion

        #region 心跳
        /// <summary>
        /// 心跳，适用于客户端
        /// </summary>
        /// <param name="accessToken">访问令牌</param>
        /// <param name="type">心跳类型</param>
        /// <param name="data">心跳数据</param>
        /// <returns></returns>
        public async Task<HeartBeatResponse> HeartBeatAsync(string accessToken, int type, string data = null)
        {
            var url = GenAuthUrl("/HeartBeat");

            var body = new Dictionary<string, string>()
            {
                ["token"] = accessToken,
                ["device"] = "",
                ["type"] = type.ToString(),
                ["data"] = data
            };

            return await HttpUtils.PostFormObjectAsync<HeartBeatResponse>(url, body);
        }
        #endregion

        #region 私有方法
        public string GenBaseUrl(string url)
        {
            return BASE_URL + url;
        }

        public string GenAuthUrl(string url)
        {
            return OAUTH_URL + url;
        }

        public string GenDataUrl(string url)
        {
            return DATA_URL + url;
        }

        private string GenNonce()
        {
            return TextUtils.RandomString(8);
        }

        private string GenSign(string nonce)
        {
            var text = _Config.AppSecret + "@" + nonce + "@" + _Config.AppSecret;
            return TextUtils.Md5(text);
        }

        public bool IsSmsCode(string sms)
        {
            return sms != null && Regex.IsMatch(sms, @"^\d{6}$");
        }

        public bool IsTicket(string ticket)
        {
            return ticket != null && Regex.IsMatch(ticket, @"^[\da-fA-F]{32}$");
        }
        #endregion
    }
}
