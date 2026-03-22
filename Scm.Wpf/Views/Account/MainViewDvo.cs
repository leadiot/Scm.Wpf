using Com.Scm.Dvo;

namespace Com.Scm.Views.Account
{
    public class MainViewDvo : ScmDvo
    {
        private string avatar;
        public string Avatar { get { return avatar; } set { SetProperty(ref avatar, value); } }

        private string userName;
        public string UserName { get { return userName; } set { SetProperty(ref userName, value); } }

        private string userCode;
        public string UserCode { get { return userCode; } set { SetProperty(ref userCode, value); } }

        private string terminalName;
        public string TerminalName { get { return terminalName; } set { SetProperty(ref terminalName, value); } }

        private string terminalCode;
        public string TerminalCode { get { return terminalCode; } set { SetProperty(ref terminalCode, value); } }

        private ScmWindow _Window;

        public MainViewDvo()
        {
        }

        public void Init(ScmWindow window)
        {
            _Window = window;

            var client = window.GetClient();
            var token = client.GetToken();

            avatar = client.GetAvatar(token.GetAvatar());

            userName = token.GetUserNames();
            userCode = token.GetUserCodes();

            terminalName = token.GetTerminalNames();
            terminalCode = token.GetTerminalCodes();
        }
    }
}
