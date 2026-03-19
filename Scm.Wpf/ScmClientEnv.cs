using System.IO;

namespace Com.Scm
{
    public class ScmClientEnv : ScmEnv
    {
        public const string API_URL = "http://api.c-scm.net/api";

        /// <summary>
        /// 主版本
        /// </summary>
        public const int MAJOR = 1;

        /// <summary>
        /// 子版本
        /// </summary>
        public const int MINOR = 4;

        /// <summary>
        /// 修正版本
        /// </summary>
        public const int PATCH = 8;

        /// <summary>
        /// 构建版本
        /// </summary>
        public const string BUILD = "2026020601";

        /// <summary>
        /// 发行日期
        /// </summary>
        public const string RELEASE_DATE = "2026-02-06";

        public const string CompanyName = "Scm.Net";
        public const string ProductName = "Nas.Net";
        public const string Copyright = "Copyright © SCM 2026";
        public const string SupportEmail = "";

        public const string Website = "https://www.c-scm.net";
        public const string UpdateUrl = "https://www.c-scm.net";
        public const string HelpUrl = "https://www.c-scm.net";
        public const string PrivacyUrl = "https://www.c-scm.net";
        public const string TermsUrl = "https://www.c-scm.net";
        public const string LicenseUrl = "https://www.c-scm.net";
        public const string DocumentationUrl = "https://www.c-scm.net";
        public const string ReleaseNotesUrl = "https://www.c-scm.net";
        public const string BugReportUrl = "https://www.c-scm.net";
        public const string FeedbackUrl = "https://www.c-scm.net";

        /// <summary>
        /// 升级程序文件
        /// </summary>
        public const string UpgradeFile = "Scm.Upgrade.exe";

        /// <summary>
        /// 应用程序目录
        /// </summary>
        public static string FileDir { get; private set; }

        /// <summary>
        /// 应用数据目录
        /// </summary>
        public static string DataDir { get; private set; }
        /// <summary>
        /// 是否网络安装
        /// </summary>
        public static bool IsClickOnceDeployed { get; private set; }


        /// <summary>
        /// 版本信息
        /// </summary>
        /// <returns></returns>
        public static string GetVersionString()
        {
            return $"{MAJOR}.{MINOR}.{PATCH}";
        }

        /// <summary>
        /// 构建信息
        /// </summary>
        /// <returns></returns>
        public static string GetBuildString()
        {
            return $"Build {BUILD}";
        }

        public static void Setup()
        {
            FileDir = AppDomain.CurrentDomain.BaseDirectory;
            if (FileDir.IndexOf(@"\AppData\Local\Apps\", StringComparison.OrdinalIgnoreCase) < 1)
            {
                DataDir = FileDir;
                return;
            }

            IsClickOnceDeployed = true;
            //// C:\Users\[用户名]\AppData\Roaming\
            //_DataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            //// C:\Users\[用户名]\AppData\Local\
            //_DataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            //// C:\ProgramData\
            //_DataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            // C:\Users\[用户名]\Documents\
            FileDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            DataDir = Path.Combine(FileDir, CompanyName, ProductName);
            if (!Directory.Exists(DataDir))
            {
                Directory.CreateDirectory(DataDir);
            }
        }

        public static string GetDataPath(string path)
        {
            return Path.Combine(DataDir, path);
        }
    }
}
