namespace Com.Scm.Oidc.Response
{
    /// <summary>
    /// 用户信息响应
    /// </summary>
    public class UserInfoResponse : OidcResponse
    {
        /// <summary>
        /// 用户信息（基于全局的用户信息）
        /// </summary>
        public OidcUserInfo User { get; set; }
    }
}
