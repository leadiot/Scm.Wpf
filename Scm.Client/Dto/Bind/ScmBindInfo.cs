using Com.Scm.Utils;
using System;

namespace Com.Scm.Dto.Bind
{
    public class ScmBindInfo
    {
        public long id { get; set; }

        /// <summary>
        /// 主机地址
        /// </summary>
        public string host { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public long user_id { get; set; }

        /// <summary>
        /// 用户代码
        /// </summary>
        public string user_codes { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string user_names { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string avatar { get; set; }

        /// <summary>
        /// 终端ID
        /// </summary>
        public long terminal_id { get; set; }

        /// <summary>
        /// 终端代码
        /// </summary>
        public string terminal_codes { get; set; }

        /// <summary>
        /// 终端名称
        /// </summary>
        public string terminal_names { get; set; }

        /// <summary>
        /// 访问令牌
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string refresh_token { get; set; }

        /// <summary>
        /// 临近过期时间，提前5分钟
        /// </summary>
        public long expires_time { get; set; }

        /// <summary>
        /// 最终过期时间
        /// </summary>
        public long expired_time { get; set; }

        public void CalcExpireTime(long expiresIn)
        {
            var now = DateTime.UtcNow;
            expired_time = TimeUtils.GetUnixTime(now.AddSeconds(expiresIn));
            expires_time = TimeUtils.GetUnixTime(now.AddSeconds(expiresIn - 60 * 5));
        }

        public bool IsExpires()
        {
            return TimeUtils.GetUnixTime() > expires_time;
        }

        public bool IsExpired()
        {
            return TimeUtils.GetUnixTime() > expired_time;
        }
    }
}
