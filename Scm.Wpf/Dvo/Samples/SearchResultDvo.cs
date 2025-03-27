using CommunityToolkit.Mvvm.ComponentModel;

namespace Com.Scm.Wpf.Dvo.Samples
{
    public partial class SearchResultDvo : ScmGridDvo
    {
        [ObservableProperty]
        private string codec;

        [ObservableProperty]
        private string namec;
    }
}
