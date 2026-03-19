namespace Com.Scm.Dvo
{
    /// <summary>
    /// 查询参数
    /// </summary>
    public class ScmSearchParamsDvo : ScmDvo
    {
        private int _Page;
        /// <summary>
        /// 页索引
        /// </summary>
        public int Page { get { return _Page; } set { SetProperty(ref _Page, value); } }

        private int _Limit;
        /// <summary>
        /// 页面大小
        /// </summary>
        public int Limit { get { return _Limit; } set { SetProperty(ref _Limit, value); } }

        public void FirstPage()
        {
            Page = 1;
        }

        public virtual string GetPageUrl()
        {
            return "";
        }
    }
}
