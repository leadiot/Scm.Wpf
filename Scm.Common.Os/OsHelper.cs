using Microsoft.Win32;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Com.Scm.Nas.Helper
{
    public class OsHelper
    {
        public static string GetText()
        {
            return Clipboard.GetText();
        }

        public static void CopyText(string text)
        {
            //Clipboard.SetDataObject(text, true);
            Clipboard.SetText(text);
        }

        public static BitmapSource GetImage()
        {
            return Clipboard.GetImage();
        }

        public static void CopyImage(BitmapSource image)
        {
            Clipboard.SetImage(image);
        }

        public static void CopyFile(BitmapSource image)
        {
            Clipboard.SetImage(image);
        }

        public static void CopyFile(params string[] files)
        {
            //// 核心代码：创建 DataObject 并指定格式为 FileDrop
            //DataObject dataObject = new DataObject();
            //dataObject.SetData(DataFormats.FileDrop, files);

            var list = new StringCollection();
            list.AddRange(files);
            // 也可以直接用一个静态方法 (WPF 4.0+)
            Clipboard.SetFileDropList(list);

            //// 放入剪贴板 (true 表示即使程序退出，数据也保留)
            //Clipboard.SetDataObject(dataObject, true);
        }

        public static string GetDownloadFolderPath()
        {
            try
            {
                // 从注册表读取下载目录路径（Windows 标准位置）
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders");

                if (key != null)
                {
                    object value = key.GetValue("{374DE290-123F-4565-9164-39C4925E467B}");
                    if (value != null && !string.IsNullOrEmpty(value.ToString()))
                    {
                        return value.ToString();
                    }
                }
            }
            catch
            {
                // 读取失败时返回备用路径
            }

            // 兜底：如果读不到注册表，返回 我的文档\Downloads
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Downloads");
        }
    }
}
