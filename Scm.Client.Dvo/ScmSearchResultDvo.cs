using Com.Scm.Wpf.Dvo;

namespace Com.Scm
{
    /// <summary>
    /// 查询结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ScmSearchResultDvo<T> : ScmDvo
    {
        private List<T> _Items;
        public List<T> Items { get { return _Items; } set { SetProperty(ref _Items, value); } }

        private int _TotalItems;
        public int TotalItems { get { return _TotalItems; } set { SetProperty(ref _TotalItems, value); } }

        private int _TotalPages;
        public int TotalPages { get { return _TotalPages; } set { SetProperty(ref _TotalPages, value); } }
    }
}
