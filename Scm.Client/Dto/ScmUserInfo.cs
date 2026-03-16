namespace Com.Scm.Dto
{
    public class ScmUserInfo
    {
        public long UserId { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }

        public long UnitId { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public int Grand { get; set; }
    }
}
