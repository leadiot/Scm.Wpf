using Com.Scm.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Com.Scm.Wpf.Dvo.Samples
{
    public partial class SearchParamsDvo : ScmSearchPageDvo
    {
        [ObservableProperty]
        private string key;

        [ObservableProperty]
        private ScmStatusEnum status;

        [ObservableProperty]
        private bool drawer;

        [ObservableProperty]
        private List<SearchResultDataDvo> items = new List<SearchResultDataDvo>();

        [RelayCommand]
        public void Search(object obj)
        {
        }
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
