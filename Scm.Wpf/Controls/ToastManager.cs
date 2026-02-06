using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Com.Scm.Controls
{
    /// <summary>
    /// Toast消息提示管理器
    /// 负责Toast的创建、显示、管理和销毁
    /// </summary>
    public static class ToastManager
    {
        // Toast容器（全局唯一）
        private static readonly List<FrameworkElement> _toastQueue = new List<FrameworkElement>();
        private static readonly Dictionary<ToastType, Brush> _typeBrushes = new Dictionary<ToastType, Brush>
        {
            { ToastType.Info, new SolidColorBrush(Color.FromArgb(255, 44, 62, 80)) },
            { ToastType.Success, new SolidColorBrush(Color.FromArgb(255, 39, 174, 96)) },
            { ToastType.Warning, new SolidColorBrush(Color.FromArgb(255, 241, 196, 15)) },
            { ToastType.Error, new SolidColorBrush(Color.FromArgb(255, 231, 76, 60)) }
        };

        /// <summary>
        /// 显示Toast消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="type">消息类型</param>
        /// <param name="autoCloseSeconds">自动关闭时间（秒），0表示不自动关闭</param>
        /// <param name="width">Toast宽度（默认300）</param>
        public static void ShowToast(Panel control, string message, ToastType type = ToastType.Info, int autoCloseSeconds = 3, double width = 300)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var container = GetToastContainer(control);

                // 创建Toast元素
                var toast = CreateToastElement(message, type, width);

                // 添加到队列并显示
                _toastQueue.Add(toast);
                container.Children.Insert(0, toast);
                //UpdateToastLayout(container);

                // 自动关闭逻辑
                if (autoCloseSeconds > 0)
                {
                    var timer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromSeconds(autoCloseSeconds)
                    };
                    timer.Tick += (sender, e) =>
                    {
                        RemoveToast(container, toast);
                        timer.Stop();
                    };
                    timer.Start();
                }
            });
        }

        private static StackPanel GetToastContainer(Panel panel)
        {
            foreach (var child in panel.Children)
            {
                if (child is StackPanel stackPanel && stackPanel.Name == "ToastContainer")
                {
                    return stackPanel;
                }
            }

            var stackPanell = new StackPanel();
            stackPanell.Name = "ToastContainer";
            stackPanell.HorizontalAlignment = HorizontalAlignment.Center;
            stackPanell.Margin = new Thickness(0, 0, 0, 20);
            panel.Children.Add(stackPanell);
            return stackPanell;
        }

        /// <summary>
        /// 创建单个Toast元素
        /// </summary>
        private static FrameworkElement CreateToastElement(string message, ToastType type, double width)
        {
            var border = new ToastControl();
            border.SetBackground(_typeBrushes[type]);
            border.SetMessage(message);
            border.Width = width;

            //// Toast外层容器
            //var border = new Border
            //{
            //    Width = width,
            //    CornerRadius = new CornerRadius(6),
            //    Background = _typeBrushes[type],
            //    Margin = new Thickness(0, 0, 0, 10), // 与下一个Toast的间距
            //    Padding = new Thickness(15)
            //};

            //// 添加阴影效果
            //border.Effect = new DropShadowEffect
            //{
            //    BlurRadius = 10,
            //    Color = Colors.Black,
            //    Opacity = 0.3,
            //    ShadowDepth = 3
            //};

            //var dockPanel = new DockPanel();
            //var button = new Button();
            //var iconPack = new PackIconMaterial()
            //{
            //    Kind = PackIconMaterialKind.Close,
            //    Foreground = Brushes.Red
            //};
            //button.Content = iconPack;
            //button.Padding = new Thickness(0);
            ////button.Margin = new Thickness(0);
            //button.Cursor = Cursors.Hand;
            //button.VerticalAlignment = VerticalAlignment.Center;
            //button.SetValue(DockPanel.DockProperty, Dock.Right);
            //dockPanel.Children.Add(button);

            //// 消息文本
            //var textBlock = new TextBlock
            //{
            //    Text = message,
            //    Foreground = Brushes.White,
            //    FontSize = 14,
            //    TextWrapping = TextWrapping.Wrap,
            //    VerticalAlignment = VerticalAlignment.Center,
            //    Margin = new Thickness(0, 5, 0, 5)
            //};
            //dockPanel.Children.Add(textBlock);

            //border.Child = dockPanel;

            // 添加淡入动画
            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300)
            };
            border.BeginAnimation(UIElement.OpacityProperty, fadeIn);

            return border;
        }

        /// <summary>
        /// 更新Toast布局（重新排列所有Toast）
        /// </summary>
        //private static void UpdateToastLayout(StackPanel container)
        //{
        //    // 反向遍历队列，保证最新的Toast在最上方
        //    var tempList = new List<FrameworkElement>(_toastQueue);
        //    tempList.Reverse();
        //    foreach (var toast in tempList)
        //    {
        //        container.Children.Add(toast);
        //    }
        //}

        /// <summary>
        /// 移除指定Toast（带动画）
        /// </summary>
        private static void RemoveToast(StackPanel container, FrameworkElement toast)
        {
            if (!_toastQueue.Contains(toast)) return;

            // 淡出动画
            var fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300)
            };
            fadeOut.Completed += (sender, e) =>
            {
                _toastQueue.Remove(toast);
                //UpdateToastLayout(container);

                // 如果队列为空，隐藏容器
                if (_toastQueue.Count == 0)
                {
                    //_toastContainer.Hide();
                }
            };
            toast.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }

        #region 快捷方法
        /// <summary>
        /// 显示成功Toast
        /// </summary>
        public static void ShowSuccess(Panel control, string message, int autoCloseSeconds = 3, double width = 300)
        {
            ShowToast(control, message, ToastType.Success, autoCloseSeconds, width);
        }

        /// <summary>
        /// 显示错误Toast
        /// </summary>
        public static void ShowError(Panel control, string message, int autoCloseSeconds = 5, double width = 300)
        {
            ShowToast(control, message, ToastType.Error, autoCloseSeconds, width);
        }

        /// <summary>
        /// 显示警告Toast
        /// </summary>
        public static void ShowWarning(Panel control, string message, int autoCloseSeconds = 4, double width = 300)
        {
            ShowToast(control, message, ToastType.Warning, autoCloseSeconds, width);
        }
        #endregion
    }

    /// <summary>
    /// Toast消息类型
    /// </summary>
    public enum ToastType
    {
        Info,    // 普通信息
        Success, // 成功
        Warning, // 警告
        Error    // 错误
    }
}
