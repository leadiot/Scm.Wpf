using Com.Scm.Wpf.Dvo;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Text;

namespace Com.Scm.Wpf.Views.Tasks
{
    public partial class TaskDvo : ScmDvo
    {
        public ObservableCollection<ATask> TaskList = new ObservableCollection<ATask>();

        [ObservableProperty]
        private ATask task;

        [ObservableProperty]
        private StringBuilder logs = new StringBuilder();
    }
}
