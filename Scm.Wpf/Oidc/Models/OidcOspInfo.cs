namespace Com.Scm.Oidc.Response
{
    /// <summary>
    /// 服务商信息
    /// </summary>
    public class OidcOspInfo
    {
        /// <summary>
        /// 服务商类型
        /// </summary>
        public OidcOspEnums Type { get; set; }

        /// <summary>
        /// 服务商代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 服务商名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 服务商图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 获取授权绝对路径
        /// </summary>
        /// <returns></returns>
        public string GetAuthUrl()
        {
            return $"{OidcClient.OAUTH_URL}/{Code}";
        }

        /// <summary>
        /// 获取图标绝对路径
        /// </summary>
        /// <returns></returns>
        public string GetIconUrl()
        {
            return $"{OidcClient.DATA_URL}/logo/{Icon}";
        }

        /// <summary>
        /// 是否邮件登录
        /// </summary>
        /// <returns></returns>
        public bool IsEmail()
        {
            return Type == OidcOspEnums.VCode && Code.ToLower() == "email";
        }

        /// <summary>
        /// 是否手机登录
        /// </summary>
        /// <returns></returns>
        public bool IsPhone()
        {
            return Type == OidcOspEnums.VCode && Code.ToLower() == "phone";
        }

        /// <summary>
        /// 是否授权登录
        /// </summary>
        /// <returns></returns>
        public bool IsOAuth()
        {
            return Type == OidcOspEnums.OAuth;
        }

        /// <summary>
        /// 是否授权登录
        /// </summary>
        /// <returns></returns>
        public bool IsVCode()
        {
            return Type == OidcOspEnums.VCode;
        }

        /// <summary>
        /// 是否扫码登录
        /// </summary>
        /// <returns></returns>
        public bool IsScan()
        {
            return Type == OidcOspEnums.Scan;
        }

        /// <summary>
        /// 是否更多
        /// </summary>
        /// <returns></returns>
        public bool IsMore()
        {
            return Type == OidcOspEnums.More;
        }

        /// <summary>
        /// 是否其它
        /// </summary>
        /// <returns></returns>
        public bool IsOther()
        {
            return Type == OidcOspEnums.Other;
        }
    }
}
