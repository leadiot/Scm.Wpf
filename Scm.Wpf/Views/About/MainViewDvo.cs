using Com.Scm.Config;
using Com.Scm.Dvo;
using Com.Scm.Helper;
using Com.Scm.Utils;
using System.IO;
using System.Reflection;

namespace Com.Scm.Views.About
{
    public class MainViewDvo : ScmDvo
    {
        private string appCode = ScmClientEnv.ProductCode;
        public string AppCode { get { return appCode; } set { SetProperty(ref appCode, value); } }

        private string appName = ScmClientEnv.ProductName;
        public string AppName { get { return appName; } set { SetProperty(ref appName, value); } }

        /// <summary>
        /// 项目
        /// </summary>
        private string appProject;
        public string AppProject { get { return appProject; } set { SetProperty(ref appProject, value); } }

        /// <summary>
        /// 网站
        /// </summary>
        private string appWebsite;
        public string AppWebsite { get { return appWebsite; } set { SetProperty(ref appWebsite, value); } }

        /// <summary>
        /// 邮件
        /// </summary>
        private string appEmail;
        public string AppEmail { get { return appEmail; } set { SetProperty(ref appEmail, value); } }

        /// <summary>
        /// 群聊
        /// </summary>
        private string appQchat;
        public string AppQchat { get { return appQchat; } set { SetProperty(ref appQchat, value); } }

        private string appVersion;
        public string AppVersion { get { return appVersion; } set { SetProperty(ref appVersion, value); } }

        private string appRelease;
        public string AppRelease { get { return appRelease; } set { SetProperty(ref appRelease, value); } }

        private string remark;
        public string Remark { get { return remark; } set { SetProperty(ref remark, value); } }

        private ScmWindow _Window;

        private ScmAppInfo _AppInfo;

        public MainViewDvo(ScmWindow window)
        {
            _Window = window;
        }

        public void Init()
        {
            LoadAppInfo();
        }

        private ScmAppInfo LoadAppDefault()
        {
            var appInfo = new ScmAppInfo();
            appInfo.code = "Scm.Net";
            appInfo.content = "一款基于.Net 10.0 和 Vue 3.0 技术、适用于企业中后台管理系统的快速开发框架。";
            return appInfo;
        }

        public void LoadAppInfo()
        {
            try
            {
                AppCode = ScmClientEnv.ProductCode;
                AppName = ScmClientEnv.ProductName;
                AppProject = "https://gitee.com/leadiot/scm.net";
                AppWebsite = "http://www.c-scm.net";
                AppEmail = "361341288@qq.com";
                AppQchat = "415872667";
                AppVersion = ScmClientEnv.GetVersionString();
                AppRelease = ScmClientEnv.RELEASE_DATE;

                _AppInfo = _Window.GetAppInfo(AppCode);
                if (_AppInfo == null)
                {
                    _AppInfo = LoadAppDefault();
                }

                AppProject = _AppInfo.project;
                AppWebsite = _AppInfo.website;
                AppEmail = _AppInfo.email;
                AppQchat = _AppInfo.qchat;

                Remark = _AppInfo.content;
            }
            catch (Exception e)
            {
                LogUtils.Error(e);
                _Window.ShowException(e);
            }
        }

        public ScmVerInfo LoadVerDefault()
        {
            var verInfo = new ScmVerInfo();
            verInfo.ver = "1.0.0";
            verInfo.date = "2026-01-01";
            verInfo.build = "2026010101";
            return verInfo;
        }

        public void LoadVerInfo()
        {
            try
            {
                var verInfo = _Window.GetVerInfo(ScmClientEnv.ProductCode);
                if (verInfo == null)
                {
                    verInfo = LoadVerDefault();
                }

                if (!ScmVerInfo.IsMatch(ScmClientEnv.GetVersionString(), verInfo.ver))
                {
                    _Window.ShowToast("当前已是最新版本！");
                    return;
                }

                var result = _Window.ShowConfirm($"发现新版本 {verInfo.ver}，是否立即升级？\n\n更新内容：\n{verInfo.remark}", "版本升级");
                if (result != true)
                {
                    return;
                }

                var filePath = Assembly.GetExecutingAssembly().Location;
                var basePath = Path.GetDirectoryName(filePath);
                var upgradeFile = Path.Combine(basePath, AppSettings.Instance.Env.UpgradeFilePath ?? ScmClientEnv.UpgradeFile);
                if (!File.Exists(upgradeFile))
                {
                    _Window.ShowAlert("升级程序不存在，无法完成升级操作！", "系统提示");
                    return;
                }

                var fileName = Path.GetFileName(filePath);
                if (fileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                {
                    fileName = fileName.Substring(0, fileName.Length - 3) + "exe";
                }
                SaveUpgradeJson(basePath, fileName, verInfo);

                OsHelper.OpenFile(upgradeFile);

                _Window.Exit(false);
            }
            catch (Exception exception)
            {
                LogUtils.Error(exception);
                _Window.ShowException(exception);
            }
        }

        private void SaveUpgradeJson(string filePath, string fileName, ScmVerInfo verInfo)
        {
            var config = new UpgradeConfig();
            config.AutoStart = true;
            config.AutoClose = true;
            config.InstallPath = filePath;
            config.ExecuteFile = fileName;
            config.AppInfo = _AppInfo;
            config.VerInfo = verInfo;

            var jsonFile = Path.Combine(filePath, AppSettings.Instance.Env.UpgradeJsonName);
            var jsonDir = Path.GetDirectoryName(jsonFile);
            if (!Directory.Exists(jsonDir))
            {
                Directory.CreateDirectory(jsonDir);
            }

            var json = config.ToJsonString();
            File.WriteAllText(jsonFile, json);
        }
    }
}
