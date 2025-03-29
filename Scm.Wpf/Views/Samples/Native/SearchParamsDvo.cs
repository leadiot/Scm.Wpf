using Com.Scm.Enums;
using Com.Scm.Wpf.Dvo;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Com.Scm.Wpf.Views.Samples.Native
{
    public partial class SearchParamsDvo : ScmSearchPageDvo
    {
        [ObservableProperty]
        private string key;

        [ObservableProperty]
        private ScmStatusEnum status;

        [ObservableProperty]
        private bool drawer;
    }

    public partial class SearchResult1Dvo : ScmSearchResultDvo<SearchResultDataDvo>
    {

    }

    public partial class SearchResultDataDvo : ScmSearchResultDataDvo
    {
        [ObservableProperty]
        private string codec;

        [ObservableProperty]
        private string namec;
    }
}
