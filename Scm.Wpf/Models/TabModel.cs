using Com.Scm.Wpf.Dvo;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace Com.Scm.Wpf.Models
{
    public partial class TabModel : ScmDvo
    {
        [ObservableProperty]
        private string header;

        [ObservableProperty]
        private FrameworkElement content;

        [ObservableProperty]
        private string code;
    }
}
