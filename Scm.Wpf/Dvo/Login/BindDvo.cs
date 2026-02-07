using Com.Scm.Utils;
using Com.Scm.Wpf.Dvo;
using System.ComponentModel.DataAnnotations;

namespace Com.Scm.Dvo.Login
{
    public partial class BindDvo : ScmDvo
    {
        [Required(ErrorMessage = "远端路径不能为空！")]
        private string host = "";
        public string Host { get { return host; } set { SetProperty(ref host, value); } }

        [Required(ErrorMessage = "终端代码不能为空！")]
        [Length(12, 12, ErrorMessage = "终端代码长度应为12个字符")]
        private string code = "";
        public string Code { get { return code; } set { SetProperty(ref code, value); } }

        [Required(ErrorMessage = "终端授权不能为空！")]
        [Length(16, 16, ErrorMessage = "终端授权长度应为16个字符")]
        private string pass = "";
        public string Pass { get { return pass; } set { SetProperty(ref pass, value); } }

        private string version = "1.0";
        public string Version { get { return version; } set { SetProperty(ref version, value); } }

        public BindDvo()
        {
            Version = $"版本信息：V{ScmClientEnv.GetVersionString()} Build {ScmClientEnv.BUILD}";
        }

        public Dictionary<string, string> GetBind()
        {

            var macList = OsHelper.GetValidMacAddresses();

            var body = new Dictionary<string, string>();
            body["codes"] = this.Code;
            body["pass"] = this.Pass;
            body["mac"] = macList.Count > 0 ? macList[0] : "";
            body["os"] = OsHelper.GetOSInfo().FullName;
            body["dn"] = OsHelper.GetDeviceName();
            body["dm"] = OsHelper.GetDeviceModel();

            return body;
        }
    }
}
