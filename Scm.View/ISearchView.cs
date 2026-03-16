namespace Com.Scm
{
    public interface ISearchView
    {
        /// <summary>
        /// 首页
        /// </summary>
        void FirstPageAsync();

        /// <summary>
        /// 前一页
        /// </summary>
        void PrevPageAsync();

        /// <summary>
        /// 后一页
        /// </summary>
        void NextPageAsync();

        /// <summary>
        /// 尾页
        /// </summary>
        void EndPageAsync();

        /// <summary>
        /// 指定页
        /// </summary>
        /// <param name="page"></param>
        void FixedPageAsync(int page);

        /// <summary>
        /// 刷新
        /// </summary>
        void ReloadPageAsync();
    }
}
