using Com.Scm.Wpf.Views.Tasks.CdPost;
using System.Windows;
using System.Windows.Controls;

namespace Com.Scm.Wpf.Views.Tasks
{
    /// <summary>
    /// TaskView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : UserControl, ScmView
    {
        private ScmWindow _Owner;
        private MainDvo _Dvo;

        public MainView()
        {
            InitializeComponent();
        }

        public void Init(ScmWindow owner)
        {
            _Owner = owner;

            _Dvo = new MainDvo();
            this.DataContext = _Dvo;
        }

        public UserControl GetView()
        {
            return this;
        }

        private void BtRun_Click(object sender, RoutedEventArgs e)
        {
            //var task = _Dvo.Task;
            //if (task == null)
            //{
            //    return;
            //}
            var task = new CdPostTask();

            System.Threading.Tasks.Task.Run((Action)task.Run);
        }
    }
}
