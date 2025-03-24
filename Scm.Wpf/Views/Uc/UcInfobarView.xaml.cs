using Com.Scm.Utils;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Com.Scm.Wpf.Views.Uc
{
    /// <summary>
    /// UcInfobarView.xaml 的交互逻辑
    /// </summary>
    public partial class UcInfobarView : UserControl
    {
        private DispatcherTimer _Timer;

        public UcInfobarView()
        {
            InitializeComponent();
        }

        public void Init()
        {
            _Timer = new DispatcherTimer();
            _Timer.Interval = new TimeSpan(0, 0, 1);
            _Timer.Tick += Timer_Tick;
            _Timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            LcTime.Text = TimeUtils.FormatDataTime(now);
        }
    }
}