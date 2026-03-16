namespace Com.Scm
{
    public abstract class ScmToken
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public abstract long GetUserId();

        /// <summary>
        /// 用户代码
        /// </summary>
        public abstract string GetUserCodes();

        /// <summary>
        /// 用户名称
        /// </summary>
        public abstract string GetUserNames();

        /// <summary>
        /// 用户头像
        /// </summary>
        public abstract string GetAvatar();

        /// <summary>
        /// 终端ID
        /// </summary>
        public abstract long GetTerminalId();

        /// <summary>
        /// 终端代码
        /// </summary>
        public abstract string GetTerminalCodes();

        /// <summary>
        /// 终端名称
        /// </summary>
        public abstract string GetTerminalNames();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract string GetAccessToken();
    }
}
