using Com.Scm.Api;
using Com.Scm.Enums;
using Com.Scm.Sys.Menu;
using Com.Scm.Utils;
using System.Collections.Generic;

namespace Com.Scm
{
    public class ScmClient
    {
#if DEBUG
        /// <summary>
        /// 服务地址
        /// </summary>
        public const string REMOTE_HOST = "localhost:5000";
#else
        /// <summary>
        /// 服务地址
        /// </summary>
        public const string REMOTE_HOST = "api.c-scm.net";
#endif

        /// <summary>
        /// 服务地址
        /// </summary>
        public const string SERVER_HOST = "api.c-scm.net";

        /// <summary>
        /// 服务器是否为连通状态
        /// </summary>
        public static bool IsConnecting { get; protected set; } = true;

        /// <summary>
        /// 服务器地址
        /// </summary>
        public string RemoteUrl { get; protected set; }

        /// <summary>
        /// 授权令牌名称
        /// </summary>
        public string TokenName { get; protected set; }

        /// <summary>
        /// 用户菜单
        /// </summary>
        public List<MenuDto> Menu { get; protected set; }

        /// <summary>
        /// 主机地址
        /// </summary>
        protected string _Host;

        public void SetHost(string host)
        {
            _Host = host ?? SERVER_HOST;
            RemoteUrl = "http://" + _Host;
        }

        /// <summary>
        /// 获取完整路径
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected string GetApiUrl(string url)
        {
            return RemoteUrl + url;
        }

        /// <summary>
        /// 本地数据目录
        /// </summary>
        public string DataDir { get; set; }

        /// <summary>
        /// 异常代码
        /// </summary>
        public int ErrorCode { get; protected set; }
        /// <summary>
        /// 异常信息
        /// </summary>
        public string ErrorMessage { get; protected set; }

        /// <summary>
        /// 获取应用信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ScmAppInfo GetAppInfo(string code)
        {
            var url = $"http://{SERVER_HOST}/api/ScmInfo/App?code={code}";

            var response = HttpUtils.GetObject<ScmApiDataResponse<ScmAppInfo>>(url);
            if (response != null && response.Success)
            {
                return response.Data;
            }

            return null;
        }

        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ScmVerInfo GetVerInfo(string code)
        {
            var url = $"http://{SERVER_HOST}/api/ScmInfo/Ver?code={code}&client={ScmClientTypeEnum.Windows}";

            var response = HttpUtils.GetObject<ScmApiDataResponse<ScmVerInfo>>(url);
            if (response != null && response.Success)
            {
                return response.Data;
            }

            return null;
        }
    }
}
