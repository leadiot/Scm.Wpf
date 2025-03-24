using Newtonsoft.Json;

namespace Com.Scm.Oidc.Response
{
    /// <summary>
    /// 刷新令牌响应
    /// </summary>
    public class RefreshTokenResponse : OidcResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }
    }
}
