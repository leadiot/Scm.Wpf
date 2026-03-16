using Com.Scm.Wpf.Dvo;

namespace Com.Scm
{
    public class ScmSearchResultDvo<T> : ScmDvo
    {
        private T _Items;
        public T Items { get { return _Items; } set { SetProperty(ref _Items, value); } }

        private int _Count;
        public int Count { get { return _Count; } set { SetProperty(ref _Count, value); } }

        private int _Total;
        public int Total { get { return _Total; } set { SetProperty(ref _Total, value); } }
    }
}
