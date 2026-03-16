namespace Com.Scm.Dto.Auth
{
    public class ScmAuthInfo : ScmToken
    {
        public long UserId { get; set; }

        public string UserCode { get; set; }

        public string UserName { get; set; }

        public string Avatar { get; set; }

        public string AccessToken { get; set; }

        public override long GetUserId()
        {
            return UserId;
        }

        public override string GetUserCodes()
        {
            return UserCode;
        }

        public override string GetUserNames()
        {
            return UserName;
        }

        public override string GetAvatar()
        {
            return Avatar;
        }

        public override long GetTerminalId()
        {
            return 0;
        }

        public override string GetTerminalCodes()
        {
            return "";
        }

        public override string GetTerminalNames()
        {
            return "";
        }

        public override string GetAccessToken()
        {
            return AccessToken;
        }
    }
}
