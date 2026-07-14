using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Com.Scm.Nas.Image
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        // 图片相关变量
        private ObservableCollection<ScmFileDvo> FileList = new ObservableCollection<FileDvo>(); // 已打开的图片列表
        private FileDvo _FileDvo;
        private BitmapSource _Image;
        private int _Index = -1; // 当前显示的图片索引

        // 支持的图片格式
        private readonly string imageFilter = "图片文件 (*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tiff)|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tiff|所有文件 (*.*)|*.*";

        public MainWindow()
        {
            InitializeComponent();
            LbFile.ItemsSource = FileList;
        }

        public void Init(string file, List<string> imageList)
        {
            if (imageList == null)
            {
                return;
            }

            Load(file, imageList);
        }

        private void Load(string file)
        {
            if (string.IsNullOrWhiteSpace(file) || !File.Exists(file))
            {
                return;
            }

            var dir = FileUtils.GetDir(file);
            var files = Directory.GetFiles(dir);
            Load(file, files);
        }

        private void Load(string file, IEnumerable<string> files)
        {
            var idx = -1;
            foreach (var tmp in files)
            {
                var ext = Path.GetExtension(tmp);
                if (!NasHelper.IsImage(ext))
                //if (!_Engine.IsReadableFile(ext))
                {
                    continue;
                }

                idx += 1;
                FileList.Add(new FileDvo { Text = System.IO.Path.GetFileName(tmp), Path = tmp });
                if (file.Equals(tmp, StringComparison.OrdinalIgnoreCase))
                {
                    _Index = idx;
                }
            }
            if (_Index < 0)
            {
                _Index = 0;
            }

            GdOpen.Visibility = Visibility.Collapsed;
            GdViewer.Visibility = Visibility.Visible;

            ChangeSelected(_Index);
            ShowImage(FileList[_Index]);
        }

        #region
        private void BtOpen_Click(object sender, RoutedEventArgs e)
        {
            Open();
        }
        #endregion

        #region 鼠标事件
        private Point _LastPoint;
        private bool _Moving;
        private bool _Dragging;
        private void BdImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _LastPoint = e.GetPosition(BdImage);
            _Moving = true;
            _Dragging = false;
            PbImage.CaptureMouse();
            e.Handled = true;
        }

        private void BdImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_Moving)
            {
                _Moving = false;
            }
            if (!_Dragging)
            {
                var visible = PlTool.Visibility == Visibility.Visible;
                if (visible)
                {
                    PlTool.Visibility = Visibility.Collapsed;
                    PlGuid.Visibility = Visibility.Collapsed;
                }
                else
                {
                    PlTool.Visibility = Visibility.Visible;
                    PlGuid.Visibility = Visibility.Visible;
                }
            }
            PbImage.ReleaseMouseCapture();
            e.Handled = true;
        }

        private void BdImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Released)
            {
                if (_Moving)
                {
                    Point currentPoint = e.GetPosition(BdImage);  //不能用 inside，必须用outside
                    System.Windows.Vector v = currentPoint - _LastPoint;
                    TransformGroup tg = PlImage.RenderTransform as TransformGroup;
                    tg.Children.Add(new TranslateTransform(v.X, v.Y));  //centerX和centerY用外部包装元素的坐标，不能用内部被变换的Canvas元素的坐标
                    _LastPoint = currentPoint;
                    _Dragging = true;
                }
            }
            e.Handled = true;
        }

        private void BdImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var point = e.GetPosition(BdImage);  //不能用 inside，必须用outside
            //var point = e.MouseDevice.GetPosition(PbImage);
            var scale = e.Delta * 0.001 + 1.0;
            ZoomImage(point, scale);
            e.Handled = true;
        }
        #endregion

        #region 按钮事件
        private void BtScreen_Click(object sender, RoutedEventArgs e)
        {
            Screen();
        }

        private void BtCopyImage_Click(object sender, RoutedEventArgs e)
        {
            CopyImage();
        }

        private void BtSaveas_Click(object sender, RoutedEventArgs e)
        {
            Saveas();
        }

        private void BtResize_Click(object sender, RoutedEventArgs e)
        {
            Resize();
        }

        private void BtZoomIn_Click(object sender, RoutedEventArgs e)
        {
            ZoomIn();
        }

        private void BtZoomOut_Click(object sender, RoutedEventArgs e)
        {
            ZoomOut();
        }

        private void BtFit_Click(object sender, RoutedEventArgs e)
        {
            ChangeFit(PbImage.Stretch);
            //FitSize();
        }

        private void BtRotateL_Click(object sender, RoutedEventArgs e)
        {
            var point = new Point();
            point.X = BdImage.ActualWidth / 2;
            point.Y = BdImage.ActualHeight / 2;
            Rotate(point, -90);
        }

        private void BtRotateR_Click(object sender, RoutedEventArgs e)
        {
            var point = new Point();
            point.X = BdImage.ActualWidth / 2;
            point.Y = BdImage.ActualHeight / 2;
            Rotate(point, 90);
        }

        private void BtRotateH_Click(object sender, RoutedEventArgs e)
        {
            ResetTransform();
            Flop();
        }

        private void BtRotateV_Click(object sender, RoutedEventArgs e)
        {
            ResetTransform();
            Flip();
        }

        private void BtSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void BtImport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtEditor_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtConfig_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtDelete_Click(object sender, RoutedEventArgs e)
        {
            Delete();
        }
        #endregion

        #region 快捷菜单
        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiOpenFile_Click(object sender, RoutedEventArgs e)
        {
            Open();
        }

        /// <summary>
        /// 放大图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiZoomIn_Click(object sender, RoutedEventArgs e)
        {
            ZoomIn();
        }

        /// <summary>
        /// 缩小图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiZoomOut_Click(object sender, RoutedEventArgs e)
        {
            ZoomOut();
        }

        /// <summary>
        /// 重置图片大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiResetZoom_Click(object sender, RoutedEventArgs e)
        {
            ResetTransform();
        }

        /// <summary>
        /// 逆时针旋转（90度）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiRotateLeft_Click(object sender, RoutedEventArgs e)
        {
            //if (_Index >= 0)
            //{
            //    currentRotation -= 90;
            //    if (currentRotation < 0) currentRotation += 360;
            //    UpdateImageTransform();
            //}
        }

        /// <summary>
        /// 顺时针旋转（90度）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiRotateRight_Click(object sender, RoutedEventArgs e)
        {
            //if (_Index >= 0)
            //{
            //    currentRotation += 90;
            //    if (currentRotation >= 360) currentRotation -= 360;
            //    UpdateImageTransform();
            //}
        }

        /// <summary>
        /// 上一张图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiLastFile_Click(object sender, RoutedEventArgs e)
        {
            LastImage();
        }

        /// <summary>
        /// 下一张图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiNextFile_Click(object sender, RoutedEventArgs e)
        {
            NextImage();
        }

        /// <summary>
        /// 导入图库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiImport_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiSave_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 另存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiSaveas_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 调整尺寸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiReszie_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 复制文件名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiCopyName_Click(object sender, RoutedEventArgs e)
        {
            CopyName();
        }

        /// <summary>
        /// 复制文件路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiCopyPath_Click(object sender, RoutedEventArgs e)
        {
            CopyPath();
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiCopyFile_Click(object sender, RoutedEventArgs e)
        {
            CopyFile();
        }

        /// <summary>
        /// 复制图像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiCopyImage_Click(object sender, RoutedEventArgs e)
        {
            CopyImage();
        }

        /// <summary>
        /// 使用系统默认应用打开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiOpenSys_Click(object sender, RoutedEventArgs e)
        {
            OpenSys();
        }

        /// <summary>
        /// 打开文件路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiBrowser_Click(object sender, RoutedEventArgs e)
        {
            OsHelper.OpenFileLocation(_FileDvo.Path);
        }

        /// <summary>
        /// 选择打开应用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiOther_Click(object sender, RoutedEventArgs e)
        {
            OsHelper.OpenSelector(_FileDvo.Path);
        }

        /// <summary>
        /// 设为壁纸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiWallPaper_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiDelete_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 导航按钮
        private void BtLast_Click(object sender, RoutedEventArgs e)
        {
            LastImage();
        }

        private void BtNext_Click(object sender, RoutedEventArgs e)
        {
            NextImage();
        }

        private void LbFile_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var file = LbFile.SelectedItem as FileDvo;
            if (file == null)
            {
                return;
            }

            _Index = LbFile.SelectedIndex;
            ShowImage(file);
        }

        #endregion

        #region 窗体事件
        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            var data = e.Data;
            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects &= ~DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            var data = e.Data;
            var files = data.GetData(DataFormats.FileDrop) as string[];
            if (files == null || files.Length < 1)
            {
                return;
            }

            Load(files[0], files);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Left)
            {
                LastImage();
                return;
            }

            if (e.Key == Key.Down || e.Key == Key.Right)
            {
                NextImage();
                return;
            }

            if (e.Key == Key.F12)
            {
                //Import();
                return;
            }

            if (e.Key == Key.F11)
            {
                Screen();
                return;
            }

            if (e.Key == Key.Delete)
            {
                Delete();
                return;
            }

            if (e.Key == Key.OemPlus)
            {
                ZoomIn();
                return;
            }

            if (e.Key == Key.OemMinus)
            {
                ZoomOut();
                return;
            }

            if (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Save();
                return;
            }

            if (e.Key == Key.T && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Saveas();
                return;
            }

            if (e.Key == Key.O && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                OpenSys();
            }
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 打开图片
        /// </summary>
        private void Open()
        {
            OpenFileDialog openDialog = new OpenFileDialog()
            {
                Filter = imageFilter,
                Title = "选择图片文件",
                RestoreDirectory = true,
                Multiselect = false // 支持多选图片
            };

            if (openDialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                // 清空原有列表，添加新选择的图片
                FileList.Clear();
                Load(openDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开图片失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChangeSelected(int idx)
        {
            this.Dispatcher.Invoke(delegate ()
            {
                LbFile.SelectedIndex = idx;
            });
        }

        /// <summary>
        /// 上一个图片
        /// </summary>
        private void LastImage()
        {
            if (FileList.Count < 1)
            {
                return;
            }

            if (_Index <= 0)
            {
                _Index = FileList.Count;
            }
            _Index -= 1;
            LbFile.SelectedIndex = _Index;
            LbFile.ScrollIntoView(LbFile.SelectedItem);
            //ShowImage(Files[_Index].Path);
        }

        /// <summary>
        /// 下一个图片
        /// </summary>
        private void NextImage()
        {
            if (FileList.Count < 1)
            {
                return;
            }

            if (_Index >= FileList.Count - 1)
            {
                _Index = -1;
            }
            _Index += 1;
            LbFile.SelectedIndex = _Index;
            LbFile.ScrollIntoView(LbFile.SelectedItem);
            //ShowImage(Files[_Index].Path);
        }

        /// <summary>
        /// 放大
        /// </summary>
        private void ZoomIn()
        {
            var point = new Point();
            point.X = BdImage.ActualWidth / 2;
            point.Y = BdImage.ActualHeight / 2;
            ZoomImage(point, 1.12);
        }

        /// <summary>
        /// 缩小
        /// </summary>
        private void ZoomOut()
        {
            var point = new Point();
            point.X = BdImage.ActualWidth / 2;
            point.Y = BdImage.ActualHeight / 2;
            ZoomImage(point, 0.88);
        }

        /// <summary>
        /// 绽放图片
        /// </summary>
        /// <param name="point"></param>
        /// <param name="scale"></param>
        private void ZoomImage(Point point, double scale)
        {
            TransformGroup tg = PlImage.RenderTransform as TransformGroup;
            if (PbImage.ActualHeight * tg.Value.M22 >= 200 || PbImage.ActualWidth * tg.Value.M11 >= 200)
            {
                tg.Children.Add(new ScaleTransform(scale, scale, point.X, point.Y));
                return;
            }

            //当图像像素宽度和高度小于200的时候只能放大，不再缩小
            if (scale > 1.0)
            {
                tg.Children.Add(new ScaleTransform(scale, scale, point.X, point.Y));
            }
        }

        /// <summary>
        /// 调整图片大小
        /// </summary>
        private void Resize()
        {
            //var window = new ResizeWindow();
            //window.Init(this);
            //window.ShowImage(_Image);
            //window.ShowDialog();
        }

        /// <summary>
        /// 垂直翻转
        /// </summary>
        private void Flip()
        {
            //_Image.Flip(FlipOption.Clockwise);
            var scale = new ScaleTransform();
            scale.ScaleY = -1;
            _Image = new TransformedBitmap(_Image, scale);
            PbImage.Source = _Image;
        }

        /// <summary>
        /// 水平翻转
        /// </summary>
        private void Flop()
        {
            var scale = new ScaleTransform();
            scale.ScaleX = -1;

            _Image = new TransformedBitmap(_Image, scale);
            PbImage.Source = _Image;
        }

        /// <summary>
        /// 旋转图片
        /// </summary>
        /// <param name="point"></param>
        /// <param name="angle"></param>
        private void Rotate(Point point, int angle)
        {
            RotateTransform rotateTransform = new RotateTransform(angle);
            rotateTransform.CenterX = point.X;
            rotateTransform.CenterY = point.Y;

            var tg = PlImage.RenderTransform as TransformGroup;
            tg.Children.Add(rotateTransform);
        }

        private void ShowImage(FileDvo file)
        {
            if (_FileDvo == file)
            {
                return;
            }

            _FileDvo = file;
            ChangeEnabled(true);
            ResetTransform();

            ShowImage(file.Path, file.Text);
        }

        private void ShowImage(string file, string name)
        {
            var image = new BitmapImage();
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = fs;
                image.EndInit();
            }
            image.Freeze();

            _Image = image;

            FitSize();
            PbImage.Source = _Image;

            ShowTitle(name);
        }

        /// <summary>
        /// 重置变换
        /// </summary>
        private void ResetTransform()
        {
            var tg = PlImage.RenderTransform as TransformGroup;
            tg.Children.Clear();
        }

        /// <summary>
        /// 适应大小
        /// </summary>
        private void FitSize()
        {
            var stretch = Stretch.Uniform;
            if (_Image.Width > BdImage.ActualWidth || _Image.Height > BdImage.ActualHeight)
            {
                stretch = Stretch.None;
            }
            ChangeFit(stretch);
        }

        private void ChangeFit(Stretch stretch)
        {
            if (stretch == Stretch.None)
            {
                PbImage.Stretch = Stretch.Uniform;
                PiFit.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.FitToScreenOutline;
                BtFit.ToolTip = "实际大小";
            }
            else
            {
                PbImage.Stretch = Stretch.None;
                PiFit.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.FitToPageOutline;
                BtFit.ToolTip = "适应窗口";
            }
        }

        private void ShowTitle(string fileName)
        {
            //Title = $"{ClientEnv.APP_NAME_EN} {ClientEnv.APP_VIEWER_CN} - {fileName}";
            Title = fileName;
        }

        private bool _IsFullscreen;
        private WindowState _LastWindowState;
        /// <summary>
        /// 全屏显示
        /// </summary>
        private void Screen()
        {
            if (_IsFullscreen)
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = _LastWindowState;
                _IsFullscreen = false;
                //PiScreen.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.Fullscreen;
                //BtScreen.ToolTip = "全屏查看";
            }
            else
            {
                _LastWindowState = this.WindowState;

                this.WindowStyle = WindowStyle.None;
                if (this.WindowState == WindowState.Maximized)
                    this.WindowState = WindowState.Minimized;
                this.WindowState = WindowState.Maximized;
                _IsFullscreen = true;
                //PiScreen.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.FullscreenExit;
                //BtScreen.ToolTip = "退出全屏";
            }
        }

        /// <summary>
        /// 复制文件名称
        /// </summary>
        private void CopyName()
        {
            Helper.OsHelper.CopyText(_FileDvo.Text);
        }

        /// <summary>
        /// 复制文件路径
        /// </summary>
        private void CopyPath()
        {
            Helper.OsHelper.CopyText(_FileDvo.Path);
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        private void CopyFile()
        {
            Helper.OsHelper.CopyFile(_FileDvo.Path);
        }

        /// <summary>
        /// 复制图片
        /// </summary>
        private void CopyImage()
        {
            Helper.OsHelper.CopyImage(_Image);
        }

        /// <summary>
        /// 保存
        /// </summary>
        private void Save()
        {
            //_Image.Save(_FileDvo.FilePath);
        }

        //private SaveasDvo _SaveasDvo;

        /// <summary>
        /// 另存
        /// </summary>
        private void Saveas()
        {
            //if (_SaveasDvo == null)
            //{
            //    _SaveasDvo = new SaveasDvo();
            //}

            //var window = new SaveasWindow();
            //window.Owner = this;
            //window.ShowData(_Image, _SaveasDvo);

            //var result = window.ShowDialog();
            //if (result != true)
            //{
            //    return;
            //}

            //var exts = _SaveasDvo.SelectedExt.Value;
            //var fileExts = Path.GetExtension(_FileDvo.Text);
            //if (fileExts.ToLower() != exts.ToLower())
            //{
            //    _Image.Convert(_SaveasDvo.SelectedExt.Value);
            //}
            //var path = _SaveasDvo.SelectedDir.Value;
            //var file = Path.GetFileNameWithoutExtension(_FileDvo.Text) + _SaveasDvo.SelectedExt.Value;
            //_Image.Save(Path.Combine(path, file));
        }

        /// <summary>
        /// 删除
        /// </summary>
        private void Delete()
        {
        }

        /// <summary>
        /// 使用系统默认应用打开
        /// </summary>
        private void OpenSys()
        {
            Scm.Helper.OsHelper.OpenFile(_FileDvo.Path);
        }

        /// <summary>
        /// 改变交互使能
        /// </summary>
        /// <param name="enabled"></param>
        private void ChangeEnabled(bool enabled)
        {
            //BtScreen.IsEnabled = enabled;
            BtImport.IsEnabled = enabled;
            BtEditor.IsEnabled = enabled;
            BtSave.IsEnabled = enabled;
            BtSaveas.IsEnabled = enabled;
            BtCopyImage.IsEnabled = enabled;
            BtZoomIn.IsEnabled = enabled;
            BtZoomOut.IsEnabled = enabled;
            BtFit.IsEnabled = enabled;
            BtRotateL.IsEnabled = enabled;
            BtRotateR.IsEnabled = enabled;
            BtRotateH.IsEnabled = enabled;
            BtRotateV.IsEnabled = enabled;
            BtResize.IsEnabled = enabled;
            BtDelete.IsEnabled = enabled;

            MiLastFile.IsEnabled = enabled;
            MiNextFile.IsEnabled = enabled;
            MiImport.IsEnabled = enabled;
            MiSave.IsEnabled = enabled;
            MiSaveas.IsEnabled = enabled;
            MiCopy.IsEnabled = enabled;
            MiOpenas.IsEnabled = enabled;
            MiWallPaper.IsEnabled = enabled;
            MiDelete.IsEnabled = enabled;
        }
        #endregion
    }
}
