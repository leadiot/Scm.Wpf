using System.IO;

namespace Com.Scm
{
    public class ScmClientEnv : ScmEnv
    {
        /// <summary>
        /// 发行日期
        /// TODO: （必需修改）发行版本，发布前需要修改此处
        /// </summary>
        public const string VER_DATE = "2026-04-13";

        /// <summary>
        /// 构建版本
        /// TODO: （必需修改）构建版本，发布前需要修改此处，格式为 YYYYMMDDXX，其中 XX 是当天的第几次构建
        /// </summary>
        public const string VER_CODE = "2026041301";

        /// <summary>
        /// 构建版本
        /// TODO: （必需修改）构建版本，发布前需要修改此处，格式为 X，其中 X 是一个整数，表示构建版本号，通常在每次构建时增加，即使没有功能更新或错误修复
        /// </summary>
        public const int BUILD = 15;

        /// <summary>
        /// 修正版本
        /// TODO: （可选修改）修正版本，发布前需要修改此处，格式为 X，其中 X 是一个整数，表示修正版本号，通常在有错误修复但没有新功能添加时增加
        /// </summary>
        public const int PATCH = 15;

        /// <summary>
        /// 次要版本
        /// TODO: （可选修改）次要版本，发布前需要修改此处，格式为 X，其中 X 是一个整数，表示次要版本号，通常在有新功能添加但保持向后兼容时增加
        /// </summary>
        public const int MINOR = 9;

        /// <summary>
        /// 主要版本
        /// TODO: （可选修改）主要版本，发布前需要修改此处，格式为 X，其中 X 是一个整数，表示主要版本号，通常在有重大功能更新或不兼容的 API 变更时增加
        /// </summary>
        public const int MAJOR = 1;

        /// <summary>
        /// 发行版本
        /// 格式：Scm.Net 1.9.15 (Build 2026041301)
        /// </summary>
        public static readonly string VER_INFO = $"{MAJOR}.{MINOR}.{PATCH}.{BUILD} @Build {VER_CODE}";

        public const string CompanyCode = "LeadIOT.Net";
        public const string CompanyName = "LeadIOT.Net";
        public const string SuiteCode = "Scm.Net";
        public const string SuiteName = "Scm.Net";
        public const string ProductCode = "Nas.Net";
        public const string ProductName = "Nas.Net";
        public const string Copyright = "Copyright © SCM 2026";
        public const string SupportEmail = "";

        public const string API_URL = "http://api.c-scm.net/api";
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
        public const string UpgradeFile = "Upgrade.Net.exe";

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
            DataDir = Path.Combine(FileDir, CompanyCode, ProductCode);
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
