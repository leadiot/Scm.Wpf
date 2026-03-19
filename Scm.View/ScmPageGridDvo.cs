using Com.Scm.Wpf.Dvo;
using Com.Scm.Wpf.Models;
using System.Windows;

namespace Com.Scm.Wpf
{
    public class ScmPageGridDvo : ScmDvo
    {
        /// <summary>
        /// 表头组件是否可见
        /// </summary>
        private Visibility headVisibility;
        public Visibility HeadVisibility { get { return headVisibility; } set { SetProperty(ref headVisibility, value); } }

        /// <summary>
        /// 客制组件是否可见
        /// </summary>
        private Visibility customVisibility = Visibility.Collapsed;
        public Visibility CustomVisibility { get { return customVisibility; } set { SetProperty(ref customVisibility, value); } }

        /// <summary>
        /// 搜索组件是否可见
        /// </summary>
        private Visibility searchVisibility;
        public Visibility SearchVisibility { get { return searchVisibility; } set { SetProperty(ref searchVisibility, value); } }

        /// <summary>
        /// 高级组件是否可见
        /// </summary>
        private Visibility moreVisibility = Visibility.Collapsed;
        public Visibility MoreVisibility { get { return moreVisibility; } set { SetProperty(ref moreVisibility, value); } }

        /// <summary>
        /// 表尾组件是否可见
        /// </summary>
        private Visibility footVisibility;
        public Visibility FootVisibility { get { return footVisibility; } set { SetProperty(ref footVisibility, value); } }

        /// <summary>
        /// 分页组件是否可见
        /// </summary>
        private Visibility pageVisibility;
        public Visibility PageVisibility { get { return pageVisibility; } set { SetProperty(ref pageVisibility, value); } }

        /// <summary>
        /// 操作组件是否可见
        /// </summary>
        private Visibility optionVisibility = Visibility.Collapsed;
        public Visibility OptionVisibility { get { return optionVisibility; } set { SetProperty(ref optionVisibility, value); } }

        /// <summary>
        /// 当前页索引
        /// </summary>
        private int pageIndex;
        public int PageIndex { get { return pageIndex; } set { SetProperty(ref pageIndex, value); } }

        /// <summary>
        /// 当前页数量
        /// </summary>
        private int pageItems = 20;
        public int PageItems { get { return pageItems; } set { SetProperty(ref pageItems, value); } }

        private int view;
        public int View { get { return view; } set { SetProperty(ref view, value); } }

        /// <summary>
        /// 总页数
        /// </summary>
        protected int totalPages;
        public int TotalPages { get { return totalPages; } set { SetProperty(ref totalPages, value); } }

        /// <summary>
        /// 总行数
        /// </summary>
        protected int totalItems;
        public int TotalItems { get { return totalItems; } set { SetProperty(ref totalItems, value); } }

        /// <summary>
        /// 查询条件
        /// </summary>
        private string key;
        public string Key { get { return key; } set { SetProperty(ref key, value); } }

        /// <summary>
        /// 展示列表
        /// </summary>
        /// <returns></returns>
        public List<ScmColumnInfo> Columns { get; protected set; }

        /// <summary>
        /// 数据列表
        /// </summary>
        public List<ScmSearchResultItemDvo> Items { get; protected set; }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="pageIndex"></param>
        public virtual void SearchAsync(int pageIndex = 0)
        {
        }

        /// <summary>
        /// 首页
        /// </summary>
        public virtual void FirstPageAsync()
        {
        }

        /// <summary>
        /// 前一页
        /// </summary>
        public virtual void PrevPageAsync()
        {
        }

        /// <summary>
        /// 后一页
        /// </summary>
        public virtual void NextPageAsync()
        {
        }

        /// <summary>
        /// 尾页
        /// </summary>
        public virtual void EndPageAsync()
        {
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public virtual void ReloadAsync()
        {
        }
    }
}
