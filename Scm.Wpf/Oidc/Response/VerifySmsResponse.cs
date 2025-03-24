using Com.Scm.Oidc;
using Com.Scm.Oidc.Response;

namespace Com.Scm.Response
{
    /// <summary>
    /// 校验授权码响应
    /// </summary>
    public class VerifySmsResponse : OidcResponse
    {
        /// <summary>
        /// 授权响应，用于后续的换取访问令牌
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 用户信息（基于当前授权会话的用户信息）
        /// </summary>
        public OidcUserInfo User { get; set; }
    }
}
