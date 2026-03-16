using Microsoft.Win32;
using System.Windows;
using System.Windows.Threading;

namespace Com.Scm.Views.Audio
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        // 计时器：用于实时刷新播放进度条和当前播放时间
        private DispatcherTimer _Timer;
        private bool _Playing;

        public MainWindow()
        {
            InitializeComponent();

            // 初始化播放器和计时器
            InitPlayer();
        }

        /// <summary>
        /// 初始化播放器配置和计时器
        /// </summary>
        private void InitPlayer()
        {
            // 设置默认音量为80%
            MpPlayer.Volume = 0.8;

            // 初始化计时器，每隔100毫秒刷新一次进度
            _Timer = new DispatcherTimer();
            _Timer.Interval = TimeSpan.FromMilliseconds(100);
            _Timer.Tick += PlayTimer_Tick;

            // 初始状态：部分按钮可用，状态提示默认文本
            TxtStatus.Text = "请选择音乐文件开始播放";
        }

        #region 事件处理
        /// <summary>
        /// 选择音乐文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtSelect_Click(object sender, RoutedEventArgs e)
        {
            Open();
        }

        /// <summary>
        /// 播放按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (_Playing)
            {
                Pause();
                _Playing = false;
            }
            else
            {
                Play();
                _Playing = true;
            }
        }

        /// <summary>
        /// 暂停按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            Pause();
        }

        /// <summary>
        /// 停止按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        /// <summary>
        /// 音量调节事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SliVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // 滑块值实时绑定播放器音量（取值0~1）
            //MpPlayer.Volume = SliVolume.Value;
        }

        private void SliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeProgress(SliProgress.Value);
        }

        private void PlayTimer_Tick(object sender, EventArgs e)
        {
            UpdateProgress();
        }
        #endregion

        #region 辅助方法：更新音频总时长
        private void Open()
        {
            // 创建文件选择对话框
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // 设置筛选条件：只显示支持的音频格式
            openFileDialog.Filter = "音频文件 (*.mp3;*.wav;*.wma)|*.mp3;*.wav;*.wma|所有文件 (*.*)|*.*";
            openFileDialog.Title = "选择要播放的音乐";

            // 如果用户选择了文件并点击确定
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // 给播放器赋值音乐文件路径
                    MpPlayer.Source = new Uri(openFileDialog.FileName);
                    // 加载音乐文件后，立即停止（防止自动播放）
                    MpPlayer.Stop();
                    // 更新状态提示
                    TxtStatus.Text = $"已选择：{System.IO.Path.GetFileName(openFileDialog.FileName)}，点击播放按钮开始播放";
                    // 刷新音频总时长显示
                    UpdateTotalTime();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"加载音乐失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #region 播放/暂停/停止 核心控制事件
        private void Play()
        {
            // 判空：防止未选择文件点击播放
            if (MpPlayer.Source == null)
            {
                MessageBox.Show("请先选择音乐文件！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // 开始播放音频
            MpPlayer.Play();
            // 启动计时器，实时刷新进度
            _Timer.Start();
            // 更新状态提示
            TxtStatus.Text = "正在播放 ▶️";
        }

        private void Pause()
        {
            if (MpPlayer.Source != null)
            {
                // 暂停播放音频
                MpPlayer.Pause();
                // 暂停计时器
                _Timer.Stop();
                TxtStatus.Text = "已暂停 ⏸️";
            }
        }

        private void Stop()
        {
            if (MpPlayer.Source != null)
            {
                // 停止播放：会重置播放进度到0
                MpPlayer.Stop();
                // 停止计时器
                _Timer.Stop();
                // 重置进度条和时间显示
                SliProgress.Value = 0;
                TxtCurrentTime.Text = "00:00";
                TxtStatus.Text = "已停止 ⏹️";
            }
        }
        #endregion

        #region 播放进度条事件：手动拖动跳转播放进度
        private void ChangeProgress(double progress)
        {
            // 判空+判断音频是否加载完成（防止拖动时报错）
            if (MpPlayer.Source == null || MpPlayer.NaturalDuration.HasTimeSpan == false)
            {
                return;
            }

            // 手动拖动进度条时，暂停计时器防止冲突
            _Timer.Stop();

            // 计算拖动后的播放位置：总时长 * 进度条百分比
            double totalSeconds = MpPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            double currentSeconds = totalSeconds * (progress / 100);
            MpPlayer.Position = TimeSpan.FromSeconds(currentSeconds);

            // 更新当前播放时间文本
            TxtCurrentTime.Text = MpPlayer.Position.ToString(@"mm\:ss");

            // 如果播放器处于播放状态，重新启动计时器
            if (MpPlayer.CanPause)
            {
                _Timer.Start();
            }
        }

        /// <summary>
        /// 更新时长
        /// </summary>
        private void UpdateProgress()
        {
            // 判空+判断音频是否加载完成
            if (MpPlayer.Source == null || MpPlayer.NaturalDuration.HasTimeSpan == false)
            {
                return;
            }

            // 计算当前播放进度的百分比，赋值给进度条
            double progressValue = 100 * MpPlayer.Position.TotalSeconds / MpPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            SliProgress.Value = progressValue;

            // 实时刷新当前播放时间
            TxtCurrentTime.Text = MpPlayer.Position.ToString(@"mm\:ss");

            // 播放完成时的处理：重置状态
            if (MpPlayer.Position >= MpPlayer.NaturalDuration.TimeSpan)
            {
                MpPlayer.Stop();
                _Timer.Stop();
                SliProgress.Value = 0;
                TxtCurrentTime.Text = "00:00";
                TxtStatus.Text = "播放完成 ✔️";
            }
        }
        #endregion

        #region 计时器事件：实时刷新播放进度（核心）
        private void UpdateTotalTime()
        {
            // 延迟加载：确保音频文件加载完成后获取总时长
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (MpPlayer.NaturalDuration.HasTimeSpan)
                {
                    TxtTotalTime.Text = MpPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
                }
            }));
        }
        #endregion

        #endregion

        private void BtnRandom_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
