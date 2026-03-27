using Microsoft.Win32;
using System.Diagnostics;
using System.Management;
using System.Net.NetworkInformation;

namespace Com.Scm.Helper
{
    public static class OsHelper
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        /// <returns></returns>
        public static string GetDeviceName()
        {
            return Environment.MachineName;
        }

        /// <summary>
        /// 获取操作系统详细信息
        /// </summary>
        /// <returns>操作系统信息对象</returns>
        public static OSInfo GetOSInfo()
        {
            OSInfo osInfo = new OSInfo();
            OperatingSystem os = Environment.OSVersion;

            // 基础版本信息
            osInfo.Version = os.Version.ToString();
            osInfo.Platform = os.Platform.ToString();

            // 判断系统位数（32/64位）
            osInfo.Bitness = Environment.Is64BitOperatingSystem ? 64 : 32;

            // 通过WMI获取更详细的系统名称
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption, ProductType FROM Win32_OperatingSystem");
                foreach (ManagementObject obj in searcher.Get())
                {
                    osInfo.FullName = obj["Caption"]?.ToString()?.Trim() ?? "未知系统";
                    // ProductType：1=工作站，2=域控制器，3=服务器
                    osInfo.IsServer = Convert.ToInt32(obj["ProductType"]) == 3;
                }
            }
            catch
            {
                osInfo.FullName = $"{os.VersionString} ({osInfo.Bitness}位)";
            }

            return osInfo;
        }

        /// <summary>
        /// 获取设备型号（硬件型号，如笔记本/服务器型号）
        /// </summary>
        /// <returns></returns>
        public static string GetDeviceModel()
        {
            try
            {
                // 方式1：读取Win32_ComputerSystem的Model属性（通用硬件型号）
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Model FROM Win32_ComputerSystem");
                foreach (ManagementObject obj in searcher.Get())
                {
                    string model = obj["Model"]?.ToString()?.Trim();
                    if (!string.IsNullOrEmpty(model) && model != "Default string")
                    {
                        return model;
                    }
                }

                // 方式2：读取Win32_BaseBoard的Product属性（主板型号，备用）
                searcher = new ManagementObjectSearcher("SELECT Product FROM Win32_BaseBoard");
                foreach (ManagementObject obj in searcher.Get())
                {
                    string product = obj["Product"]?.ToString()?.Trim();
                    if (!string.IsNullOrEmpty(product) && product != "To be filled by O.E.M.")
                    {
                        return product;
                    }
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 打开资源管理器并定位到指定文件夹
        /// </summary>
        /// <param name="url">文件夹路径</param>
        /// <returns>是否打开成功</returns>
        public static void Browse(string url)
        {
            // 启动资源管理器
            //Process.Start("explorer.exe", '"' + url + '"');
            Process.Start(new ProcessStartInfo(url)
            {
                UseShellExecute = true
            });
        }

        public static void OpenFile(string file, string args = null)
        {
            // 启动资源管理器
            //Process.Start("explorer.exe", '"' + url + '"');
            Process.Start(new ProcessStartInfo()
            {
                FileName = file,
                UseShellExecute = true
            });
        }

        /// <summary>
        /// 打开资源管理器并定位到指定文件夹
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        /// <returns>是否打开成功</returns>
        public static void OpenFolder(string folderPath)
        {
            // 启动资源管理器
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = folderPath,
                UseShellExecute = true // 必须设为true，兼容Windows系统
            });
        }

        /// <summary>
        /// 打开资源管理器并选中指定文件（精准定位）
        /// </summary>
        /// <param name="filePath">文件完整路径</param>
        /// <returns>是否打开成功</returns>
        public static void OpenFileLocation(string filePath)
        {
            // 关键参数：/select, + 文件完整路径（逗号后无空格）
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = $"/select,{filePath}",
                UseShellExecute = true
            });
        }

        /// <summary>
        /// 打开系统应用选择器
        /// </summary>
        /// <param name="file"></param>
        public static void OpenSelector(string file)
        {
            file = file.Replace("/", "\\");
            OpenFile("rundll32.exe", "shell32,OpenAs_RunDLL " + file);
        }

        // 注册表开机启动项路径（当前用户）
        private const string RunRegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

        /// <summary>
        /// 检查是否已设置开机启动
        /// </summary>
        /// <param name="appName">应用标识（自定义，如"MyWpfApp"）</param>
        /// <returns>是否开启开机启动</returns>
        public static bool IsStartupEnabled(string appName, string appPath)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RunRegistryPath, false))
            {
                if (key == null) return false;
                // 获取对应键值的应用路径
                var value = key.GetValue(appName)?.ToString();
                // 校验路径是否匹配当前应用
                return !string.IsNullOrEmpty(value) && value.Equals(appPath, StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// 开启开机启动
        /// </summary>
        /// <param name="appName">应用标识（自定义，如"MyWpfApp"）</param>
        /// <returns>是否设置成功</returns>
        public static bool EnableStartup(string appName, string appPath)
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RunRegistryPath, true))
                {
                    if (key == null) return false;
                    // 写入：键名=应用标识，值=应用完整路径（带引号，避免路径含空格）
                    key.SetValue(appName, $"\"{appPath}\"", RegistryValueKind.String);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 关闭开机启动
        /// </summary>
        /// <param name="appName">应用标识（自定义，如"MyWpfApp"）</param>
        /// <returns>是否取消成功</returns>
        public static bool DisableStartup(string appName)
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RunRegistryPath, true))
                {
                    if (key == null) return false;
                    // 删除对应键值
                    key.DeleteValue(appName, false);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// 获取有效物理网卡的MAC地址（过滤虚拟/隧道接口、全0地址）
        /// </summary>
        /// <returns>MAC地址列表（格式：00-1A-2B-3C-4D-5E）</returns>
        public static List<string> GetValidMacAddresses()
        {
            List<string> validMacs = new List<string>();
            try
            {
                // 获取本机所有网络接口
                IEnumerable<NetworkInterface> allInterfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (NetworkInterface ni in allInterfaces)
                {
                    // 过滤条件：
                    // 1. 网卡操作状态为启动（Up）
                    // 2. 网卡类型为物理网卡（以太网、无线局域网）
                    // 3. MAC地址不是全0（避免虚拟接口）
                    if (ni.OperationalStatus == OperationalStatus.Up
                        && (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                            || ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                        && ni.GetPhysicalAddress().GetAddressBytes().Any(b => b != 0))
                    {
                        // 将物理地址转换为 00-1A-2B 格式的字符串
                        string mac = BitConverter.ToString(ni.GetPhysicalAddress().GetAddressBytes());
                        validMacs.Add(mac);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取MAC地址失败：" + ex.Message);
            }
            return validMacs;
        }
    }

    /// <summary>
    /// 操作系统信息封装类
    /// </summary>
    public class OSInfo
    {
        public string FullName { get; set; } // 完整系统名称（如Windows 10 专业版）
        public string Version { get; set; } // 版本号（如10.0.19045）
        public string Platform { get; set; } // 平台（如Win32NT）
        public int Bitness { get; set; } // 系统位数（32/64）
        public bool IsServer { get; set; } // 是否为服务器系统
    }
}
