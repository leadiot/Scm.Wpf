using Com.Scm.Dvo;

namespace Com.Scm.Views.Samples.Remote
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
