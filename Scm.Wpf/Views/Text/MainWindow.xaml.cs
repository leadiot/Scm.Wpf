using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace Com.Scm.Views.Text
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        // 记录当前编辑的文件路径
        private string currentFilePath = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            // 绑定快捷键（增强用户体验）
            BindShortcuts();
        }

        public void Init(string file)
        {
            LoadText(file);
        }

        #region 菜单点击事件
        // 新建文件
        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            // 如果有未保存的内容，提示用户
            if (PromptSaveIfNeeded())
            {
                txtEditor.Clear();
                currentFilePath = string.Empty;
                Title = "WPF文本编辑器 - 未命名";
            }
        }

        // 打开文件
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            if (!PromptSaveIfNeeded())
            {
                return;
            }

            OpenFileDialog openDialog = new OpenFileDialog()
            {
                Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*",
                Title = "打开文本文件",
                RestoreDirectory = true
            };

            if (openDialog.ShowDialog() != true)
            {
                return;
            }

            LoadText(openDialog.FileName);
        }

        // 保存文件
        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            // 如果是新文件（未保存过），执行另存为
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveAsFile_Click(sender, e);
                return;
            }

            SaveText(currentFilePath, txtEditor.Text);
        }

        // 另存为
        private void SaveAsFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog()
            {
                Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*",
                Title = "保存文本文件",
                RestoreDirectory = true,
                DefaultExt = "txt"
            };

            if (saveDialog.ShowDialog() != true)
            {
                return;
            }

            currentFilePath = saveDialog.FileName;
            SaveText(currentFilePath, txtEditor.Text);
        }

        // 退出程序
        private void ExitApp_Click(object sender, RoutedEventArgs e)
        {
            if (PromptSaveIfNeeded())
            {
                Application.Current.Shutdown();
            }
        }

        // 撤销
        private void UndoEdit_Click(object sender, RoutedEventArgs e)
        {
            if (txtEditor.CanUndo)
            {
                txtEditor.Undo();
            }
        }

        // 重做
        private void RedoEdit_Click(object sender, RoutedEventArgs e)
        {
            if (txtEditor.CanRedo)
            {
                txtEditor.Redo();
            }
        }

        // 复制
        private void CopyText_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtEditor.SelectedText))
            {
                Clipboard.SetText(txtEditor.SelectedText);
            }
        }

        // 剪切
        private void CutText_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtEditor.SelectedText))
            {
                txtEditor.Cut();
            }
        }

        // 粘贴
        private void PasteText_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                txtEditor.Paste();
            }
        }

        // 全选
        private void SelectAllText_Click(object sender, RoutedEventArgs e)
        {
            txtEditor.SelectAll();
        }

        private void MiWrap_Click(object sender, RoutedEventArgs e)
        {
            if (MiWrap.IsChecked)
            {
                MiWrap.IsChecked = false;
                txtEditor.TextWrapping = TextWrapping.NoWrap;
            }
            else
            {
                MiWrap.IsChecked = true;
                txtEditor.TextWrapping = TextWrapping.Wrap;
            }
        }
        #endregion

        #region 辅助方法
        // 绑定快捷键
        private void BindShortcuts()
        {
            //// Ctrl+N 新建
            //var newCmd = new KeyBinding(ApplicationCommands.New, Key.N, ModifierKeys.Control);
            //newCmd.Command = new ScmCommand(NewFile_Click, null);
            //this.InputBindings.Add(newCmd);

            //// Ctrl+O 打开
            //var openCmd = new KeyBinding(ApplicationCommands.Open, Key.O, ModifierKeys.Control);
            //openCmd.Command = (s, e) => OpenFile_Click(s, e);
            //this.InputBindings.Add(openCmd);

            //// Ctrl+S 保存
            //var saveCmd = new KeyBinding(ApplicationCommands.Save, Key.S, ModifierKeys.Control);
            //saveCmd.Command = (s, e) => SaveFile_Click(s, e);
            //this.InputBindings.Add(saveCmd);

            //// Ctrl+Z 撤销
            //var undoCmd = new KeyBinding(ApplicationCommands.Undo, Key.Z, ModifierKeys.Control);
            //undoCmd.Command = (s, e) => UndoEdit_Click(s, e);
            //this.InputBindings.Add(undoCmd);
        }

        // 如果有未保存的内容，提示用户保存
        private bool PromptSaveIfNeeded()
        {
            // 简单判断：如果文本框有内容且文件路径为空，或文本内容与文件内容不一致，视为未保存
            if (string.IsNullOrEmpty(txtEditor.Text) && string.IsNullOrEmpty(currentFilePath))
            {
                return true;
            }

            if (!string.IsNullOrEmpty(currentFilePath) && File.Exists(currentFilePath))
            {
                string fileContent = File.ReadAllText(currentFilePath);
                if (txtEditor.Text == fileContent)
                {
                    return true;
                }
            }

            // 弹出提示框
            var result = MessageBox.Show("当前文件有未保存的内容，是否保存？", "提示",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    SaveFile_Click(null, null);
                    return true;
                case MessageBoxResult.No:
                    return true;
                case MessageBoxResult.Cancel:
                    return false;
                default:
                    return false;
            }
        }

        // 窗口关闭时触发（防止用户误关）
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!PromptSaveIfNeeded())
            {
                e.Cancel = true; // 取消关闭
            }
            base.OnClosing(e);
        }
        #endregion

        private async void LoadText(string path)
        {
            currentFilePath = path;
            Title = $"WPF文本编辑器 - {Path.GetFileName(currentFilePath)}";

            var info = new FileInfo(path);
            if (info.Length > NasEnv.MAX_CHUNK_SIZE)
            {
                MessageWindow.ShowDialog(this, "文件内容过大！");
                return;
            }

            try
            {
                txtEditor.Text = await File.ReadAllTextAsync(path);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"打开文件失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveText(string file, string text)
        {
            try
            {
                // 保存内容到当前文件
                File.WriteAllText(file, text);
                Title = $"WPF文本编辑器 - {Path.GetFileName(currentFilePath)}";
                MessageBox.Show("文件保存成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"保存文件失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
