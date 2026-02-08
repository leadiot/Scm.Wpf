using Com.Scm.Enums;
using Com.Scm.Wpf.Dvo;

namespace Com.Scm.Wpf.Views.Samples.Remote
{
    public class SearchParamsDvo : ScmSearchParamsDvo
    {
        private string key;
        public string Key { get { return key; } set { SetProperty(ref key, value); } }

        private ScmRowStatusEnum status;
        public ScmRowStatusEnum Status { get { return status; } set { SetProperty(ref status, value); } }

        private bool drawer;
        public bool Drawer { get { return drawer; } set { SetProperty(ref drawer, value); } }
    }

    public class SearchResult1Dvo : ScmSearchResultDvo<SearchResultDataDvo>
    {

    }

    public class SearchResultDataDvo : ScmSearchResultItemDvo
    {
        private string codec;

        private string namec;
    }
}
