using Com.Scm.Wpf.Enums;

namespace Com.Scm.Config
{
    /// <summary>
    /// 
    /// </summary>
    public class EnvConfig
    {
        public string UpgradeFilePath { get; set; }

        public string UpgradeJsonName { get; set; }

        public ScmLoginTypeEnum LoginMode { get; set; }

        public void LoadDefault()
        {
            UpgradeFilePath = "Upgrade/Scm.Upgrade.exe";
            UpgradeJsonName = "Upgrade/Scm.Upgrade.json";
        }

        public string GetApiUrl(string path)
        {
            return "http://api.c-scm.net/api" + path;
        }
    }
}
