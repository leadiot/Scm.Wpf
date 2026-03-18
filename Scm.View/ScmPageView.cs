using Com.Scm.Wpf.Models;
using System.Collections;
using System.Windows.Controls;

namespace Com.Scm
{
    public interface ScmPageView : ScmView
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        List<ScmColumnInfo> GetColumns();

        /// <summary>
        /// 列表数据
        /// </summary>
        IEnumerable GetItemsSource();

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="pageIndex"></param>
        void SearchAsync(int pageIndex = 0);

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
        /// 刷新
        /// </summary>
        void ReloadAsync();

        UserControl GetCustomView();

        UserControl GetSearchView();

        UserControl GetInfoView();

        UserControl GetEditView();
    }
}
