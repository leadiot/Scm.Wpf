using Com.Scm.Enums;
using Com.Scm.Utils;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Com.Scm.Wpf.Dvo.Login
{
    public partial class PassDvo : ScmDvo
    {
        public string Key { get; set; }

        /// <summary>
        /// 登录类型
        /// </summary>
        //private ScmLoginModeEnum type = ScmLoginModeEnum.AllUnit;

        /// <summary>
        /// 登录模式
        /// </summary>
        private ScmLoginModeEnum mode = ScmLoginModeEnum.ByPass;
        public ScmLoginModeEnum Mode { get { return mode; } set { SetProperty(ref mode, value); } }

        [Required(ErrorMessage = "登录用户不能为空！")]
        [Length(2, 32, ErrorMessage = "登录用户长度应为2至32个字符")]
        public string User { get { return user; } set { SetProperty(ref user, value); } }
        private string user = "admin@dev";

        [Required(ErrorMessage = "登录口令不能为空！")]
        [Length(6, 32, ErrorMessage = "登录口令长度应为6至32个字符")]
        public string Pass { get { return pass; } set { SetProperty(ref pass, value); } }
        private string pass;

        [Required(ErrorMessage = "验证码不能为空！")]
        [Length(4, 4, ErrorMessage = "验证码长度应为4个字符")]
        public string Code { get { return code; } set { SetProperty(ref code, value); } }
        private string code;

        public string VCodeUrl { get { return vCodeUrl; } set { SetProperty(ref vCodeUrl, value); } }
        private string vCodeUrl;

        public PassDvo()
        {
        }

        public void ChangeVCode(string apiUrl)
        {
            Key = TextUtils.GuidString();
            VCodeUrl = $"{apiUrl}/captcha/cha/{Key}?timestamp={TimeUtils.GetUnixTime()}";
        }

        public override bool IsValid()
        {
            if (!base.IsValid())
            {
                return false;
            }

            if (!Regex.IsMatch(User, @"^\w+@\w+$"))
            {
                AddError(nameof(User), "登录用户格式应为：user@unit");
                return false;
            }

            return true;
        }

        public Dictionary<string, string> GetLogin()
        {
            ValidateAllProperties();
            if (HasErrors)
            {
                return null;
            }

            if (!IsValid())
            {
                return null;
            }

            var user = this.User + "@dev";
            var unit = "";
            var tmp = user.Split('@');
            if (tmp.Length > 1)
            {
                user = tmp[0];
                unit = tmp[1];
            }
            var time = TimeUtils.GetUnixTime();
            var pass = SecUtils.Sha256(this.Pass);

            var body = new Dictionary<string, string>();
            //body["type"] = ((int)type).ToString();
            body["mode"] = ((int)Mode).ToString();
            body["user"] = user;
            body["unit"] = unit;
            body["pass"] = SecUtils.Sha256(pass + '@' + time);
            body["time"] = time.ToString();
            body["key"] = this.Key;
            body["code"] = this.Code;

            return body;
        }
    }
}
