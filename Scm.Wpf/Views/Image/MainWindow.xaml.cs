using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Com.Scm.Views.Image
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        // 图片相关变量
        private List<string> imageFileList = new List<string>(); // 已打开的图片列表
        private int currentImageIndex = -1; // 当前显示的图片索引
        private double currentScale = 1.0; // 当前缩放比例（1.0=100%）
        private int currentRotation = 0; // 当前旋转角度（0/90/180/270）
        private bool isFitToWindow = false; // 是否适应窗口显示

        // 支持的图片格式
        private readonly string imageFilter = "图片文件 (*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tiff)|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tiff|所有文件 (*.*)|*.*";

        public MainWindow()
        {
            InitializeComponent();
            BindShortcuts(); // 绑定快捷键
            UpdateImageInfo(); // 初始化信息栏
        }

        public void Init(string file, List<string> imageList)
        {
            imageFileList.Clear();
            imageFileList.AddRange(imageList);
            currentImageIndex = 0;
            for (var i = 0; i < imageFileList.Count; i += 1)
            {
                if (file.Equals(imageFileList[i], StringComparison.OrdinalIgnoreCase))
                {
                    currentImageIndex = i;
                    break;
                }
            }
            currentScale = 1.0;
            currentRotation = 0;
            isFitToWindow = false;

            // 加载并显示第一张图片
            LoadAndDisplayImage(imageFileList[currentImageIndex]);
        }

        #region 菜单点击事件
        // 打开图片
        private void OpenImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog()
            {
                Filter = imageFilter,
                Title = "选择图片文件",
                RestoreDirectory = true,
                Multiselect = true // 支持多选图片
            };

            if (openDialog.ShowDialog() == true)
            {
                try
                {
                    // 清空原有列表，添加新选择的图片
                    imageFileList.Clear();
                    imageFileList.AddRange(openDialog.FileNames);
                    currentImageIndex = 0;
                    currentScale = 1.0;
                    currentRotation = 0;
                    isFitToWindow = false;

                    // 加载并显示第一张图片
                    LoadAndDisplayImage(imageFileList[currentImageIndex]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"打开图片失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // 退出程序
        private void ExitApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // 放大图片
        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (currentImageIndex >= 0)
            {
                currentScale += 0.1; // 每次放大10%
                UpdateImageTransform();
            }
        }

        // 缩小图片
        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (currentImageIndex >= 0 && currentScale > 0.1)
            {
                currentScale -= 0.1; // 每次缩小10%
                UpdateImageTransform();
            }
        }

        // 重置图片大小
        private void ResetZoom_Click(object sender, RoutedEventArgs e)
        {
            if (currentImageIndex >= 0)
            {
                currentScale = 1.0;
                isFitToWindow = false;
                UpdateImageTransform();
            }
        }

        // 适应窗口显示
        private void FitToWindow_Click(object sender, RoutedEventArgs e)
        {
            if (currentImageIndex >= 0)
            {
                isFitToWindow = !isFitToWindow;
                if (isFitToWindow)
                {
                    // 计算适应窗口的缩放比例
                    var img = imgDisplay.Source as BitmapImage;
                    if (img != null)
                    {
                        double scaleX = imgScrollViewer.ActualWidth / img.PixelWidth;
                        double scaleY = imgScrollViewer.ActualHeight / img.PixelHeight;
                        currentScale = Math.Min(scaleX, scaleY);
                    }
                }
                else
                {
                    currentScale = 1.0;
                }
                UpdateImageTransform();
            }
        }

        // 逆时针旋转（90度）
        private void RotateLeft_Click(object sender, RoutedEventArgs e)
        {
            if (currentImageIndex >= 0)
            {
                currentRotation -= 90;
                if (currentRotation < 0) currentRotation += 360;
                UpdateImageTransform();
            }
        }

        // 顺时针旋转（90度）
        private void RotateRight_Click(object sender, RoutedEventArgs e)
        {
            if (currentImageIndex >= 0)
            {
                currentRotation += 90;
                if (currentRotation >= 360) currentRotation -= 360;
                UpdateImageTransform();
            }
        }

        // 上一张图片
        private void PrevImage_Click(object sender, RoutedEventArgs e)
        {
            if (imageFileList.Count > 1 && currentImageIndex > 0)
            {
                currentImageIndex--;
                currentScale = 1.0;
                currentRotation = 0;
                isFitToWindow = false;
                LoadAndDisplayImage(imageFileList[currentImageIndex]);
            }
        }

        // 下一张图片
        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            if (imageFileList.Count > 1 && currentImageIndex < imageFileList.Count - 1)
            {
                currentImageIndex++;
                currentScale = 1.0;
                currentRotation = 0;
                isFitToWindow = false;
                LoadAndDisplayImage(imageFileList[currentImageIndex]);
            }
        }
        #endregion

        #region 辅助方法
        // 绑定快捷键
        private void BindShortcuts()
        {
            // Ctrl+O 打开
            AddKeyBinding(Key.O, ModifierKeys.Control, OpenImage_Click);
            // Ctrl++ 放大
            AddKeyBinding(Key.Add, ModifierKeys.Control, ZoomIn_Click);
            AddKeyBinding(Key.O, ModifierKeys.Control, ZoomIn_Click);
            // Ctrl+- 缩小
            AddKeyBinding(Key.Subtract, ModifierKeys.Control, ZoomOut_Click);
            AddKeyBinding(Key.I, ModifierKeys.Control, ZoomOut_Click);
            // Ctrl+0 重置
            AddKeyBinding(Key.D0, ModifierKeys.Control, ResetZoom_Click);
            // 左箭头 上一张
            AddKeyBinding(Key.Left, ModifierKeys.None, PrevImage_Click);
            // 右箭头 下一张
            AddKeyBinding(Key.Right, ModifierKeys.None, NextImage_Click);
        }

        // 辅助添加快捷键
        private void AddKeyBinding(Key key, ModifierKeys modifiers, ExecutedRoutedEventHandler handler)
        {
            KeyBinding kb = new KeyBinding

            {
                Key = key,
                Modifiers = modifiers,
                Command = new RoutedCommand()
            };

            var binding = new CommandBinding(kb.Command, handler);
            this.CommandBindings.Add(binding);
            this.InputBindings.Add(kb);
        }

        // 加载并显示图片
        private void LoadAndDisplayImage(string filePath)
        {
            try
            {
                // 创建图片源（避免文件被锁定）
                BitmapImage bitmap = new BitmapImage();
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = fs;
                    bitmap.EndInit();
                }
                bitmap.Freeze(); // 冻结以提升性能

                imgDisplay.Source = bitmap;
                UpdateImageTransform();
                UpdateImageInfo();

                // 更新窗口标题
                Title = $"WPF图片查看器 - {Path.GetFileName(filePath)}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载图片失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 更新图片变换（缩放+旋转）
        private void UpdateImageTransform()
        {
            if (imgDisplay.Source == null) return;

            TransformGroup transformGroup = new TransformGroup();

            // 缩放变换
            ScaleTransform scaleTransform = new ScaleTransform(currentScale, currentScale);
            transformGroup.Children.Add(scaleTransform);

            // 旋转变换（围绕图片中心旋转）
            RotateTransform rotateTransform = new RotateTransform(
                currentRotation,
                imgDisplay.Source.Width / 2,
                imgDisplay.Source.Height / 2);
            transformGroup.Children.Add(rotateTransform);

            imgDisplay.RenderTransform = transformGroup;
            UpdateImageInfo();
        }

        // 更新图片信息栏
        private void UpdateImageInfo()
        {
            if (currentImageIndex < 0 || imageFileList.Count == 0 || imgDisplay.Source == null)
            {
                txtImageInfo.Text = "未打开图片 | WPF图片查看器";
                return;
            }

            var img = imgDisplay.Source as BitmapImage;
            string filePath = imageFileList[currentImageIndex];
            string fileName = Path.GetFileName(filePath);
            long fileSize = new FileInfo(filePath).Length / 1024; // 转换为KB

            // 计算旋转后的尺寸
            int width = img.PixelWidth;
            int height = img.PixelHeight;
            if (currentRotation % 180 != 0)
            {
                (width, height) = (height, width); // 旋转90/270度时宽高互换
            }

            // 拼接信息
            txtImageInfo.Text =
                $"文件：{fileName} | " +
                $"尺寸：{width}×{height} (旋转{currentRotation}°) | " +
                $"大小：{fileSize} KB | " +
                $"缩放：{Math.Round(currentScale * 100, 1)}% | " +
                $"进度：{currentImageIndex + 1}/{imageFileList.Count}";
        }

        // 鼠标滚轮缩放
        private void ImgScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (currentImageIndex >= 0 && !isFitToWindow)
            {
                // 滚轮向上=放大，向下=缩小
                if (e.Delta > 0)
                {
                    currentScale += 0.05; // 每次放大5%
                }
                else
                {
                    currentScale = Math.Max(0.1, currentScale - 0.05); // 最小缩放到10%
                }
                UpdateImageTransform();
            }
        }
        #endregion
    }
}
