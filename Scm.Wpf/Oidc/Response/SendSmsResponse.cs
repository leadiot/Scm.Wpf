namespace Com.Scm.Oidc.Response
{
    /// <summary>
    /// 发送授权码响应
    /// </summary>
    public class SendSmsResponse : OidcResponse
    {
        /// <summary>
        /// 消息凭据，用于后续的消息校验使用
        /// </summary>
        public string Key { get; set; }
    }
}
