namespace Com.Scm.Wpf.Views
{
    public interface ISearchView
    {
        /// <summary>
        /// 首页
        /// </summary>
        void FirstPageAsync();

        /// <summary>
        /// 尾页
        /// </summary>
        void LastPageAsync();

        /// <summary>
        /// 前一页
        /// </summary>
        void PrevPageAsync();

        /// <summary>
        /// 后一页
        /// </summary>
        void NextPageAsync();

        /// <summary>
        /// 刷新
        /// </summary>
        void ReloadPageAsync();
    }
}
