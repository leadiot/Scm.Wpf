using Com.Scm.Dao;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace Com.Scm;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private static Mutex _Mutex;
    private static bool _IsFirstInstance;
    private static string _CurrentAppPath;

    protected override void OnStartup(StartupEventArgs e)
    {
        _CurrentAppPath = Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty;
        var mutexName = GetPathBasedMutexName();

        _IsFirstInstance = IsFirstInstance(mutexName);
        if (!_IsFirstInstance)
        {
            ActivateExistingInstance();
            Shutdown();
            return;
        }

        base.OnStartup(e);

        var splashWindow = new SplashWindow();
        splashWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        SqlHelper.Close();

        if (_IsFirstInstance && _Mutex != null)
        {
            _Mutex.ReleaseMutex();
            _Mutex.Dispose();
        }

        base.OnExit(e);
    }

    private static string GetPathBasedMutexName()
    {
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(_CurrentAppPath));
        return $"Com.Scm.Wpf.SingleInstance_{Convert.ToHexString(hash)}";
    }

    private static bool IsFirstInstance(string mutexName)
    {
        _Mutex = new Mutex(true, mutexName, out bool createdNew);
        return createdNew;
    }

    private static void ActivateExistingInstance()
    {
        try
        {
            var currentProcess = Process.GetCurrentProcess();
            var processes = Process.GetProcessesByName(currentProcess.ProcessName);

            foreach (var process in processes)
            {
                if (process.Id != currentProcess.Id)
                {
                    var processPath = process.MainModule?.FileName ?? string.Empty;
                    if (string.Equals(processPath, _CurrentAppPath, StringComparison.OrdinalIgnoreCase))
                    {
                        var handle = process.MainWindowHandle;
                        if (handle != IntPtr.Zero)
                        {
                            if (IsIconic(handle))
                            {
                                ShowWindow(handle, SW_RESTORE);
                            }
                            SetForegroundWindow(handle);
                        }
                        break;
                    }
                }
            }
        }
        catch
        {
        }
    }

    #region Win32 API
    private const int SW_RESTORE = 9;

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool IsIconic(IntPtr hWnd);
    #endregion
}

