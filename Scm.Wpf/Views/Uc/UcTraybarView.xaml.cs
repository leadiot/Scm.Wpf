using Com.Scm.Utils;
using Com.Scm.Wpf.Dvo;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Com.Scm.Wpf.Views.Uc
{
    /// <summary>
    /// UcInfobarView.xaml 的交互逻辑
    /// </summary>
    public partial class UcTraybarView : UserControl
    {
        private DispatcherTimer _Timer;
        private InfobarDvo _Dvo;

        public UcTraybarView()
        {
            InitializeComponent();
        }

        public void Init(ScmWindow window)
        {
            _Dvo = new InfobarDvo();
            this.DataContext = _Dvo;

            _Timer = new DispatcherTimer();
            _Timer.Interval = new TimeSpan(0, 0, 1);
            _Timer.Tick += Timer_Tick;
            _Timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            _Dvo.Time = TimeUtils.FormatDataTime(now);
        }

        public void ShowInfo(string info)
        {
            _Dvo.Info = info;
        }
    }

    public partial class InfobarDvo : ScmDvo
    {
        [ObservableProperty]
        private string info;

        [ObservableProperty]
        private string time;
    }
}