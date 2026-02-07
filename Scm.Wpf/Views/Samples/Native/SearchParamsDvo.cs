using Com.Scm.Enums;
using Com.Scm.Wpf.Dvo;

namespace Com.Scm.Wpf.Views.Samples.Native
{
    public class SearchParamsDvo : ScmSearchPageDvo
    {
        private string _key;
        public string key { get { return _key; } set { SetProperty(ref _key, value); } }

        private ScmRowStatusEnum _status;
        public ScmRowStatusEnum status { get { return _status; } set { SetProperty(ref _status, value); } }

        private bool _drawer;
        public bool drawer { get { return _drawer; } set { SetProperty(ref _drawer, value); } }
    }

    public class SearchResult1Dvo : ScmSearchResultDvo<SearchResultDataDvo>
    {

    }

    public class SearchResultDataDvo : ScmSearchResultDataDvo
    {
        private string codec;
        public string Codec { get { return codec; } set { SetProperty(ref codec, value); } }

        private string namec;
        public string Nodec { get { return namec; } set { SetProperty(ref namec, value); } }
    }
}
