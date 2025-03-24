namespace Com.Scm.Oidc
{
    /// <summary>
    /// OIDC配置
    /// </summary>
    public class OidcConfig
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppKey { get; set; }
        /// <summary>
        /// 应用密钥
        /// </summary>
        public string AppSecret { get; set; }
        /// <summary>
        /// 回调地址
        /// </summary>
        public string RedirectUrl { get; set; }
        /// <summary>
        /// 应用模式，暂未使用
        /// </summary>
        public int Mode { get; set; }

        public void Prepare()
        {
        }

        /// <summary>
        /// 使用测试应用
        /// </summary>
        public void UseTest()
        {
            AppKey = "08dc965832db7248";
            AppSecret = "50qvwk2mynnxiq20rgzrx8w8s94kfaml";
        }

        /// <summary>
        /// 使用演示应用
        /// </summary>
        public void UseDemo()
        {
            AppKey = "08dd24bcbf6c7612";
            AppSecret = "835g6dqnl462py752upaevrzfk0ehjtg";
        }
    }
}
