using Com.Scm.Config;
using Com.Scm.Dto;
using Com.Scm.Dvo;
using Com.Scm.Helper;
using Com.Scm.Upgrade.Config;
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

        /// <summary>
        /// 使用帮助
        /// </summary>
        private string appHelp;
        public string AppHelp { get { return appHelp; } set { SetProperty(ref appHelp, value); } }

        private string appVersion;
        public string AppVersion { get { return appVersion; } set { SetProperty(ref appVersion, value); } }

        private string appRelease;
        public string AppRelease { get { return appRelease; } set { SetProperty(ref appRelease, value); } }

        private string introduction;
        public string Introduction { get { return introduction; } set { SetProperty(ref introduction, value); } }

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

        public async void LoadAppInfo()
        {
            try
            {
                AppCode = ScmClientEnv.ProductCode;
                AppName = ScmClientEnv.ProductName;
                AppVersion = ScmClientEnv.VER_INFO;
                AppRelease = ScmClientEnv.VER_DATE;

                _AppInfo = await _Window.GetAppInfoAsync(AppCode);
                if (_AppInfo == null)
                {
                    _AppInfo = LoadAppDefault();
                }

                AppProject = _AppInfo.project ?? "https://gitee.com/leadiot/nas.net";
                AppWebsite = _AppInfo.homepage ?? "http://www.c-scm.net";
                AppEmail = _AppInfo.email ?? "361341288@qq.com";
                AppQchat = _AppInfo.qchat ?? "415872667";
                AppHelp = "http://www.c-scm.net/nas/help.html";

                Introduction = _AppInfo.content;
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
            verInfo.ver_info = "1.0.0";
            verInfo.ver_date = "2026-01-01";
            verInfo.ver_code = "2026010101";
            return verInfo;
        }

        public async void LoadVerInfo()
        {
            try
            {
                var verInfo = await _Window.GetVerInfoAsync(ScmClientEnv.ProductCode);
                if (verInfo == null)
                {
                    verInfo = LoadVerDefault();
                }

                if (!verInfo.IsNewer(ScmClientEnv.BUILD))
                {
                    _Window.ShowToast("当前已是最新版本！");
                    return;
                }

                var result = _Window.ShowConfirm($"发现新版本 {verInfo.ver_info}，是否立即升级？\n\n更新内容：\n{verInfo.remark}", "版本升级");
                if (result != true)
                {
                    return;
                }

                var filePath = Assembly.GetExecutingAssembly().Location;
                var installPath = Path.GetDirectoryName(filePath);
                var upgradeFile = Path.Combine(installPath, AppSettings.Instance.Env.UpgradeFilePath ?? ScmClientEnv.UpgradeFilePath);
                if (!File.Exists(upgradeFile))
                {
                    _Window.ShowAlert("升级程序不存在，无法完成升级操作！", "系统提示");
                    return;
                }

                var launchFile = Path.GetFileName(filePath);
                if (launchFile.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                {
                    launchFile = launchFile.Substring(0, launchFile.Length - 3) + "exe";
                }
                SaveUpgradeJson(installPath, launchFile, verInfo);

                OsHelper.OpenFile(upgradeFile, Path.GetDirectoryName(upgradeFile));

                _Window.Exit(false);
            }
            catch (Exception exception)
            {
                LogUtils.Error(exception);
                _Window.ShowException(exception);
            }
        }

        private void SaveUpgradeJson(string installPath, string launchFile, ScmVerInfo verInfo)
        {
            var config = new UpgradeConfig();
            config.Title = AppName;
            config.AppInfo = _AppInfo.content;
            config.VerInfo = verInfo.remark;
            config.OldVersion = ScmClientEnv.VER_INFO;
            config.NewVersion = verInfo.ver_info;
            config.AutoStart = true;
            config.AutoClose = true;

            var backupPath = Path.Combine(installPath, "backup");
            var tempPath = Path.Combine(installPath, "temp");

            var dataFile = Path.Combine(installPath, "data", "scm.db");
            var time = DateTime.Now.ToString("yyyyMMddHHmmss");
            var backupFile = Path.Combine(backupPath, "data", $"scm-{time}.db");

            var stepConfig = StepConfig.NewCreateDirStep("创建备份目录", backupPath);
            config.AddStep(stepConfig);

            stepConfig = StepConfig.NewCreateDirStep("创建临时目录", tempPath);
            config.AddStep(stepConfig);

            var downloadFile = Path.Combine(tempPath, "upgrade.zip");
            stepConfig = StepConfig.NewDownloadStep("升级文件下载", verInfo.url, downloadFile);
            config.AddStep(stepConfig);

            stepConfig = StepConfig.NewMoveDocStep("数据备份", dataFile, backupFile);
            stepConfig.ContinueOnError = true;
            config.AddStep(stepConfig);

            stepConfig = StepConfig.NewUnzipStep("升级包解压", downloadFile, tempPath);
            config.AddStep(stepConfig);

            stepConfig = StepConfig.NewMoveDirStep("应用更新", Path.Combine(tempPath, "Scm.Wpf"), installPath);
            config.AddStep(stepConfig);

            stepConfig = StepConfig.NewCopyDocStep("数据恢复", backupFile, dataFile);
            stepConfig.ContinueOnError = true;
            config.AddStep(stepConfig);

            //stepConfig = StepConfig.NewCopyDocStep("配置恢复", Path.Combine(installPath, launchFile), null);
            //config.AddStep(stepConfig);

            stepConfig = StepConfig.NewLaunchStep("启用应用", Path.Combine(installPath, launchFile), null);
            config.AddStep(stepConfig);

            stepConfig = StepConfig.NewDeleteDirStep("临时文件清理", tempPath);
            config.AddStep(stepConfig);

            var jsonFile = Path.Combine(installPath, AppSettings.Instance.Env.UpgradeJsonName ?? ScmClientEnv.UpgradeJsonPath);
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
