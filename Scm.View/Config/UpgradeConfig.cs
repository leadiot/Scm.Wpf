using Com.Scm.Utils;
using System.IO;

namespace Com.Scm.Config
{
    public class UpgradeConfig
    {
        /// <summary>
        /// 应用标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 安装路径
        /// </summary>
        public string InstallPath { get; set; }

        /// <summary>
        /// 是否重启
        /// </summary>
        public bool AutoStart { get; set; }

        public bool AutoClose { get; set; }

        /// <summary>
        /// 重启文件
        /// </summary>
        public string ExecuteFile { get; set; }

        /// <summary>
        /// 重启参数
        /// </summary>
        public string ExecuteArgs { get; set; }

        /// <summary>
        /// 应用信息
        /// </summary>
        public ScmAppInfo AppInfo { get; set; } = new ScmAppInfo();

        /// <summary>
        /// 版本信息
        /// </summary>
        public ScmVerInfo VerInfo { get; set; } = new ScmVerInfo();

        public void LoadDefault()
        {
            Title = "Nas.Net更新";

            VerInfo.major = 1;
            VerInfo.ver_date = "2024-01-01";
            VerInfo.ver_code = "2024010101";
            VerInfo.ver_info = "1.0.0";
            VerInfo.remark = "初始版本";
            VerInfo.url = "https://download.mobatek.net/2542025111600034/MobaXterm_Portable_v25.4.zip";

            AutoStart = true;
            ExecuteFile = "MobaXterm_Personal_25.4.exe";
            ExecuteArgs = null;
        }

        public void Save()
        {
            var file = Path.Combine(AppContext.BaseDirectory, "upgrade.json");
            var json = this.ToJsonString();
            FileUtils.WriteText(file, json);
        }
    }
}
