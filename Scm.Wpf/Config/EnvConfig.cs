using Com.Scm.Enums;

namespace Com.Scm.Config
{
    /// <summary>
    /// 
    /// </summary>
    public class EnvConfig
    {
        public string CheckUpgradeUrl { get; set; }

        public string UpgradeFilePath { get; set; }

        public string UpgradeJsonName { get; set; }

        public ScmLoginTypeEnum LoginMode { get; set; } = ScmLoginTypeEnum.Operator;

        public void LoadDefault()
        {
            CheckUpgradeUrl = ScmClientEnv.UpgradeUrl;
            UpgradeFilePath = ScmClientEnv.UpgradeFilePath;
            UpgradeJsonName = ScmClientEnv.UpgradeJsonPath;
        }
    }
}
