using Com.Scm.Wpf.Dvo;

namespace Com.Scm.Wpf.Views.Samples.Remote
{
    public class SearchResultDvo : ScmSearchResultDvo<SearchResultDataDvo>
    {
    }

    public class SearchResultDataDvo : ScmSearchResultItemDvo
    {
        private string codec;

        private string namec;
    }
}
