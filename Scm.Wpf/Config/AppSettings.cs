using Newtonsoft.Json.Linq;
using System.IO;

namespace Com.Scm.Wpf.Config
{
    public class AppSettings
    {
        private static JObject _Settings = null;

        public static bool Load()
        {
            var file = "appsettings.json";
            if (!File.Exists(file))
            {
                _Settings = JObject.Parse("{}");
                return false;
            }

            var text = File.ReadAllText(file);
            if (string.IsNullOrEmpty(text))
            {
                _Settings = JObject.Parse("{}");
                return false;
            }

            _Settings = JObject.Parse(text);
            return true;
        }

        public static T GetSection<T>(string section)
        {
            var obj = _Settings[section];
            if (obj == null)
            {
                return default(T);
            }

            return obj.ToObject<T>();
        }
    }
}
