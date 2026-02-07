using Com.Scm.Wpf.Dvo;
using System.Collections.ObjectModel;
using System.Text;

namespace Com.Scm.Wpf.Views.Tasks
{
    public partial class MainDvo : ScmDvo
    {
        public ObservableCollection<ATask> TaskList { get; set; } = new ObservableCollection<ATask>();

        private ATask task;
        public ATask Task { get { return task; } set { SetProperty(ref task, value); } }

        private StringBuilder logs = new StringBuilder();
        public StringBuilder Logs { get { return logs; } set { SetProperty(ref logs, value); } }
    }
}
