using Com.Scm.Enums;
using Com.Scm.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel.DataAnnotations;

namespace Com.Scm.Wpf.Dvo.Login
{
    public partial class PassDvo : ScmDvo
    {
        public string Key { get; set; }

        [ObservableProperty]
        private LoginTypeEnum type = LoginTypeEnum.ByPass;

        [Required(ErrorMessage = "登录用户不能为空！")]
        [ObservableProperty]
        private string user = "admin@dev";

        [Required(ErrorMessage = "登录口令不能为空！")]
        [ObservableProperty]
        private string pass = "C-scm.cn";

        [Required(ErrorMessage = "验证码不能为空！")]
        [Length(4, 4, ErrorMessage = "验证码长度应为4个字符")]
        [ObservableProperty]
        private string code;

        [ObservableProperty]
        private string vCodeUrl;

        public PassDvo()
        {
            Key = TextUtils.GuidString();
            vCodeUrl = $"{IEnv.API_URL}/captcha/cha/{Key}";
        }

        [RelayCommand]
        public void ChangeVCode2(object obj)
        {

        }

        public void ChangeVCode()
        {
            Key = TextUtils.GuidString();
            VCodeUrl = $"{IEnv.API_URL}/captcha/cha/{Key}?timestamp={TimeUtils.GetUnixTime()}";
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

            var time = TimeUtils.GetUnixTime();
            var pass = TextUtils.Sha(this.Pass);

            var body = new Dictionary<string, string>();
            body["type"] = ((int)Type).ToString();
            body["user"] = this.User;
            body["pass"] = TextUtils.Sha(pass + '@' + time);
            body["time"] = time.ToString();
            body["key"] = this.Key;
            body["code"] = this.Code;

            return body;
        }
    }
}
