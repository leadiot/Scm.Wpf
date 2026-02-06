using Com.Scm.Enums;
using Com.Scm.Oidc;
using Com.Scm.Uid.Config;
using Com.Scm.Utils;
using Newtonsoft.Json.Linq;

namespace Com.Scm.Config
{
    public class AppSettings
    {
        private JObject _Settings = null;

        public static bool Load()
        {
            Instance = new AppSettings();
            if (!Instance.LoadConfig())
            {
                Instance.LoadDefault();
            }
            return true;
        }

        private bool LoadConfig()
        {
            var file = "appsettings.json";
            if (!FileUtils.ExistsDoc(file))
            {
                _Settings = JObject.Parse("{}");
                return false;
            }

            var text = FileUtils.ReadText(file);
            if (string.IsNullOrEmpty(text))
            {
                _Settings = JObject.Parse("{}");
                return false;
            }

            _Settings = JObject.Parse(text);

            EnvConfig = GetSection<EnvConfig>("Env");
            if (EnvConfig == null)
            {
                EnvConfig = new EnvConfig();
            }

            return true;
        }

        public void LoadDefault()
        {
            if (Env == null)
            {
                Env = new EnvConfig();
                Env.LoadDefault();
            }

            if (Uid == null)
            {
                Uid = new UidConfig();
                Uid.Type = UidType.SnowFlake;
            }

            if (Sql == null)
            {
                Sql = new SqlConfig();
                Sql.Type = "Sqlite";
                Sql.Text = "Data Source=./data/scm.db";
            }

            if (Oidc == null)
            {
                Oidc = new OidcConfig();
                Oidc.UseTest();
            }
        }

        public T GetSection<T>(string section)
        {
            var obj = _Settings[section];
            if (obj == null)
            {
                return default(T);
            }

            return obj.ToObject<T>();
        }

        public EnvConfig EnvConfig { get; private set; }

        /// <summary>
        /// 是否自动启动
        /// </summary>
        public bool AutoStartup { get; set; } = true;

        /// <summary>
        /// 启动后主窗口状态
        /// </summary>
        public ScmWindowState WindowState { get; set; }

        /// <summary>
        /// 环境配置
        /// </summary>
        public EnvConfig Env { get; set; }

        /// <summary>
        /// 主键配置
        /// </summary>
        public UidConfig Uid { get; set; }

        /// <summary>
        /// 数据库配置
        /// </summary>
        public SqlConfig Sql { get; set; }

        /// <summary>
        /// OIDC配置
        /// </summary>
        public OidcConfig Oidc { get; set; }

        public static AppSettings Instance { get; private set; }

        public void Save()
        {
            var file = "appsettings.json";
            var json = this.ToJsonString();
            Com.Scm.Utils.FileUtils.WriteText(file, json);
        }
    }
}
