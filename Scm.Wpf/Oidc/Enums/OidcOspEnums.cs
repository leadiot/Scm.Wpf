namespace Com.Scm.Oidc
{
    /// <summary>
    /// 服务商类型枚举
    /// </summary>
    public enum OidcOspEnums
    {
        /// <summary>
        /// 未知
        /// </summary>
        None = 0,
        /// <summary>
        /// 三方授权
        /// </summary>
        OAuth = 1,
        /// <summary>
        /// 验证登录
        /// </summary>
        VCode = 2,
        /// <summary>
        /// 扫码登录
        /// </summary>
        Scan = 3,

        /// <summary>
        /// 更多
        /// </summary>
        More = 8,
        /// <summary>
        /// 其它
        /// </summary>
        Other = 9
    }
}
