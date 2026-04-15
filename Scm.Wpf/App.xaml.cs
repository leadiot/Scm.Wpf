using Com.Scm.Dao;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Com.Scm;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private const string MutexName = "Com.Scm.Wpf.SingleInstance";
    private static Mutex _Mutex;
    private static bool _IsFirstInstance;

    protected override void OnStartup(StartupEventArgs e)
    {
        _IsFirstInstance = IsFirstInstance();
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

    private static bool IsFirstInstance()
    {
        _Mutex = new Mutex(true, MutexName, out bool createdNew);
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

