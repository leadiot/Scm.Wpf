using Com.Scm.Models;
using HandyControl.Controls;
using System.Collections.ObjectModel;
using System.Windows;

namespace Com.Scm.Dvo
{
    /// <summary>
    /// 分页查询结果
    /// </summary>
    /// <typeparam name="T">明细类型</typeparam>
    public class ScmPageResult<T>
    {
        /// <summary>
        /// 当前页数据
        /// </summary>
        public List<T> Items { get; set; }

        /// <summary>
        /// 数据总行数
        /// </summary>
        public int Total { get; set; }
    }

    /// <summary>
    /// 分页列表视图模型基类。分页导航由基类统一实现，子类仅需实现数据加载。
    /// </summary>
    public abstract class ScmPageGridDvo : ScmDvo
    {
        /// <summary>
        /// 表头组件是否可见
        /// </summary>
        private Visibility headVisibility;
        public Visibility HeadVisibility { get { return headVisibility; } set { SetProperty(ref headVisibility, value); } }

        /// <summary>
        /// 搜索组件是否可见
        /// </summary>
        private Visibility searchVisibility;
        public Visibility SearchVisibility { get { return searchVisibility; } set { SetProperty(ref searchVisibility, value); } }

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
        /// 每页数量
        /// </summary>
        private int pageItems = 20;
        public int PageItems { get { return pageItems; } set { SetProperty(ref pageItems, value); } }

        /// <summary>
        /// 当前页索引
        /// </summary>
        private int pageIndex = 1;
        public int PageIndex { get { return pageIndex; } set { SetProperty(ref pageIndex, value); } }

        /// <summary>
        /// 总行数
        /// </summary>
        private int itemCount;
        public int ItemCount { get { return itemCount; } set { SetProperty(ref itemCount, value); } }

        /// <summary>
        /// 总页数
        /// </summary>
        private int pageCount;
        public int PageCount { get { return pageCount; } set { SetProperty(ref pageCount, value); } }

        /// <summary>
        /// 查询关键字
        /// </summary>
        private string key;
        public string Key { get { return key; } set { SetProperty(ref key, value); } }

        /// <summary>
        /// 展示列配置
        /// </summary>
        private List<ScmColumnInfo> columns;
        public List<ScmColumnInfo> Columns { get { return columns; } protected set { SetProperty(ref columns, value); } }

        /// <summary>
        /// 查询。pageIndex 大于 0 时跳转到指定页，否则刷新当前页。
        /// </summary>
        /// <param name="pageIndex"></param>
        public Task SearchAsync(int pageIndex = 0)
        {
            if (pageIndex > 0)
            {
                PageIndex = pageIndex;
            }
            return ReloadAsync();
        }

        /// <summary>
        /// 首页
        /// </summary>
        public Task FirstPageAsync()
        {
            return SearchAsync(1);
        }

        /// <summary>
        /// 前一页
        /// </summary>
        public Task PrevPageAsync()
        {
            return SearchAsync(PageIndex > 1 ? PageIndex - 1 : 1);
        }

        /// <summary>
        /// 后一页
        /// </summary>
        public Task NextPageAsync()
        {
            if (PageCount < 1)
            {
                return Task.CompletedTask;
            }
            return SearchAsync(PageIndex < PageCount ? PageIndex + 1 : PageCount);
        }

        /// <summary>
        /// 尾页
        /// </summary>
        public Task EndPageAsync()
        {
            return SearchAsync(PageCount > 1 ? PageCount : 1);
        }

        /// <summary>
        /// 刷新当前页
        /// </summary>
        public async Task ReloadAsync()
        {
            try
            {
                var total = await LoadDataAsync();
                ItemCount = total;
                PageCount = PageItems > 0 ? (int)Math.Ceiling((double)total / PageItems) : 0;
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        /// <summary>
        /// 加载数据，返回数据总行数
        /// </summary>
        protected abstract Task<int> LoadDataAsync();

        /// <summary>
        /// 数据加载异常处理
        /// </summary>
        /// <param name="ex"></param>
        protected virtual void OnError(Exception ex)
        {
            Growl.Error(ex.Message);
        }
    }

    /// <summary>
    /// 分页列表视图模型基类（泛型）
    /// </summary>
    /// <typeparam name="T">明细类型</typeparam>
    public abstract class ScmPageGridDvo<T> : ScmPageGridDvo where T : ScmSearchResultItemDvo
    {
        /// <summary>
        /// 当前页数据列表
        /// </summary>
        public ObservableCollection<T> Items { get; } = new ObservableCollection<T>();

        protected sealed override async Task<int> LoadDataAsync()
        {
            var result = await LoadPageAsync();

            Items.Clear();
            if (result?.Items != null)
            {
                foreach (var item in result.Items)
                {
                    Items.Add(item);
                }
            }

            return result?.Total ?? 0;
        }

        /// <summary>
        /// 加载当前页数据。分页参数使用 PageIndex / PageItems / Key。
        /// </summary>
        protected abstract Task<ScmPageResult<T>> LoadPageAsync();
    }
}
