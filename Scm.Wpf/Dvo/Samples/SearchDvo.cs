using Com.Scm.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Com.Scm.Wpf.Dvo.Samples
{
    public partial class SearchDvo : ScmDvo
    {
        [ObservableProperty]
        private string key;

        [ObservableProperty]
        private ScmStatusEnum status;

        [ObservableProperty]
        private bool drawer;

        [ObservableProperty]
        private List<SearchResultDvo> items = new List<SearchResultDvo>();

        [RelayCommand]
        public void Search(object obj)
        {

        }
    }
}
