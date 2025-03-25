using CommunityToolkit.Mvvm.ComponentModel;

namespace Com.Scm.Wpf.Dvo.Samples
{
    public partial class SearchResultDvo : ScmDataDvo
    {
        [ObservableProperty]
        private string codec;

        [ObservableProperty]
        private string namec;

        [ObservableProperty]
        private bool isChecked;
    }
}
