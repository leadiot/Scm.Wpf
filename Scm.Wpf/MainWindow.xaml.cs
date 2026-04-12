using Com.Scm.Actions;
using Com.Scm.Config;
using Com.Scm.Dao;
using Com.Scm.Dvo;
using Com.Scm.Helper;
using Com.Scm.Login;
using Com.Scm.Response;
using Com.Scm.Sys.Config;
using Com.Scm.Sys.Menu;
using Com.Scm.Utils;
using Com.Scm.Views;
using HandyControl.Controls;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace Com.Scm;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : HandyControl.Controls.Window, ScmWindow
{
    #region 成员属性
    /// <summary>
    /// 数据字典（线程安全）
    /// </summary>
    private static readonly ConcurrentDictionary<string, List<ResOptionDvo>> _Dic = new ConcurrentDictionary<string, List<ResOptionDvo>>();
    /// <summary>
    /// 系统配置（线程安全）
    /// </summary>
    private static readonly ConcurrentDictionary<string, ConfigDto> _Cfg = new ConcurrentDictionary<string, ConfigDto>();

    /// <summary>
    /// 客户端对象
    /// </summary>
    private ScmClient _Client;

    /// <summary>
    /// 当前用户
    /// </summary>
    private ScmToken _Token;

    /// <summary>
    /// 访问凭据
    /// </summary>
    private string _AccessToken;
    /// <summary>
    /// 应用代码
    /// </summary>
    private string _AppKey = "";

    private Scm.Views.Home.MainView _HomeView;
    private Scm.Views.Account.MainView _AccountView;

    private MainWindowDvo _Dvo;
    #endregion

    /// <summary>
    /// 构造函数
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 初始化方法
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    public void Init(ScmClient client, List<MenuDto> menuList)
    {
        _Client = client;

        this.Title = ScmClientEnv.ProductName;

        Growl.Register("ScmToast", GdToast);

        _AppKey = "";

        if (menuList == null)
        {
            menuList = new List<MenuDto>();
        }

        _Dvo = new MainWindowDvo();
        _Dvo.Init(this, menuList);
        this.DataContext = _Dvo;

        UcMenu.Init(this, menuList);
        UcGuid.Init(this, menuList);
        UcTray.Init(this);
        UcInfo.Init(this);

        Show();

        ShowTray();

        ShowHome();
    }

    #region ScmWindow 接口实现
    /// <summary>
    /// 显示提示（弹窗，但不是需要交互）
    /// </summary>
    public void ShowToast(string message, ToastType type = ToastType.Info)
    {
        LogUtils.Info("ShowToast:" + message);
        Growl.Info(message, "ScmToast");
        //ToastManager.ShowToast(GdToast, message, type);
    }

    /// <summary>
    /// 显示提示（标签，不需要交互）
    /// </summary>
    /// <param name="message"></param>
    public void ShowNotice(string message, string title = null)
    {
        LogUtils.Info("ShowInfo:" + message);
        UcTray.ShowInfo(message);
    }

    /// <summary>
    /// 显示提示（弹窗，中止当前操作）
    /// </summary>
    public void ShowAlert(string message, string title = null)
    {
        LogUtils.Info("ShowAlert:" + message);
        MessageWindow.ShowDialog(this, message, title);
    }

    public void ShowError(string message, string title = null)
    {
    }

    public bool? ShowConfirm(string message, string title = null)
    {
        return MessageWindow.ShowDialog(this, message, title);
    }

    public void ShowException(Exception exception, string title = null)
    {
        ExceptionWindow.ShowException(this, exception);
    }

    public void HideGuid()
    {
        WpfHelper.CreateWidthChangedAnimation(this.UcGuid, 200, 60, new TimeSpan(0, 0, 0, 0, 300));
    }

    public void ShowGuid()
    {
        WpfHelper.CreateWidthChangedAnimation(this.UcGuid, 60, 200, new TimeSpan(0, 0, 0, 0, 300));
    }

    public void HideMenu()
    {
        UcMenu.Visibility = Visibility.Collapsed;
    }

    public void ShowMenu()
    {
        UcMenu.Visibility = Visibility.Visible;
    }

    public void HideTray()
    {
        _Dvo.TrayVisibility = Visibility.Hidden;
    }

    public void ShowTray()
    {
        //_Tray = new NotifyIcon();
        //_Tray.Text = "Scm.Net";
        //_Tray.Visibility = Visibility.Visible;
        //_Tray.MouseDoubleClick += TiTask_MouseDoubleClick;

        //var menu = new ContextMenu();
        //_Tray.ContextMenu = menu;

        //var item = new MenuItem();
        //item.Name = "MiMain";
        //item.Header = "显示主窗口";
        //item.Click += MiMain_Click;
        //menu.Items.Add(item);

        //menu.Items.Add(new Separator());

        //item = new MenuItem();
        //item.Name = "MiLogout";
        //item.Header = "退出登录";
        //item.Click += MiLogout_Click;
        //menu.Items.Add(item);

        //menu.Items.Add(new Separator());

        //item = new MenuItem();
        //item.Name = "MiExit";
        //item.Header = "退出应用";
        //item.Click += MiExit_Click;
        //menu.Items.Add(item);

        _Dvo.TrayVisibility = Visibility.Visible;
    }

    public void ShowHome()
    {
        //if (_HomeView == null)
        //{
        //    _HomeView = new Scm.Views.Home.MainView();
        //    _HomeView.Init(this);
        //}
        //_Dvo.ShowView("home", "首页", _HomeView);

        UcGuid.SetSelected(0);
    }

    public void ShowAccount()
    {
        //if (_AccountView == null)
        //{
        //    _AccountView = new Scm.Views.Account.MainView();
        //    _AccountView.Init(this);
        //}
        //_Dvo.ShowView("account", "账户信息", _AccountView);
        var window = new AccountWindow();
        window.Init(this);
        window.ShowDialog();
    }

    public void ShowView(string codec, string namec, string view, string args = null, string module = null, bool useCache = true)
    {
        _Dvo.ShowView(codec, namec, view, args, module, useCache);
    }

    /// <summary>
    /// GET请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task<T> GetObjectAsync<T>(string url, Dictionary<string, string> body = null, Dictionary<string, string> head = null)
    {
        url = _Client.GetApiUrl(url);

        head = GetHeader(head);

        var response = await HttpUtils.GetObjectAsync<ScmApiDataResponse<T>>(url, body, head);
        if (response == null)
        {
            return default;
        }
        if (!response.Success)
        {
            ShowAlert(response.Message);
            return default;
        }

        return response.Data;
    }

    /// <summary>
    /// GET请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task<string> GetStringAsync(string url, Dictionary<string, string> body = null, Dictionary<string, string> head = null)
    {
        url = _Client.GetApiUrl(url);

        head = GetHeader(head);

        return await HttpUtils.GetStringAsync(url, body, head);
    }

    /// <summary>
    /// POST请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="body"></param>
    /// <param name="head"></param>
    /// <returns></returns>
    public async Task<T> PostFormObjectAsync<T>(string url, Dictionary<string, string> body = null, Dictionary<string, string> head = null)
    {
        url = _Client.GetApiUrl(url);

        head = GetHeader(head);

        var response = await HttpUtils.PostFormObjectAsync<ScmApiDataResponse<T>>(url, body, head);
        if (response == null)
        {
            return default;
        }
        if (!response.Success)
        {
            ShowAlert(response.Message);
            return default;
        }

        return response.Data;
    }

    /// <summary>
    /// POST请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="body"></param>
    /// <param name="head"></param>
    /// <returns></returns>
    public async Task<string> PostFormStringAsync(string url, Dictionary<string, string> body = null, Dictionary<string, string> head = null)
    {
        url = _Client.GetApiUrl(url);

        head = GetHeader(head);

        string json = body != null ? body.ToJsonString() : null;
        return await HttpUtils.PostJsonStringAsync(url, json, head);
    }

    /// <summary>
    /// POST请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="body"></param>
    /// <param name="head"></param>
    /// <returns></returns>
    public async Task<T> PostJsonObjectAsync<T>(string url, string body = null, Dictionary<string, string> head = null)
    {
        url = _Client.GetApiUrl(url);

        head = GetHeader(head);

        var response = await HttpUtils.PostJsonObjectAsync<ScmApiDataResponse<T>>(url, body, head);
        if (response == null)
        {
            return default;
        }
        if (!response.Success)
        {
            ShowAlert(response.Message);
            return default;
        }

        return response.Data;
    }

    /// <summary>
    /// POST请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="body"></param>
    /// <param name="head"></param>
    /// <returns></returns>
    public async Task<string> PostJsonStringAsync(string url, string body = null, Dictionary<string, string> head = null)
    {
        url = _Client.GetApiUrl(url);

        head = GetHeader(head);

        return await HttpUtils.PostJsonStringAsync(url, null, head);
    }

    public ScmAppInfo GetAppInfo(string code)
    {
        var info = _Client.GetAppInfo(code);
        if (info == null)
        {
            return null;
        }

        return info;
    }

    public async Task<ScmAppInfo> GetAppInfoAsync(string code)
    {
        var info = await _Client.GetAppInfoAsync(code);
        if (info == null)
        {
            return null;
        }

        return info;
    }

    public ScmVerInfo GetVerInfo(string code)
    {
        var info = _Client.GetVerInfo(code);
        if (info == null)
        {
            return null;
        }

        return info;
    }

    public async Task<ScmVerInfo> GetVerInfoAsync(string code)
    {
        var info = await _Client.GetVerInfoAsync(code);
        if (info == null)
        {
            return null;
        }

        return info;
    }

    public ScmClient GetClient()
    {
        return _Client;
    }

    public System.Windows.Window GetWindow()
    {
        return this;
    }
    #endregion

    #region 事件处理
    private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {

    }

    #region 托盘图标
    private void TiTask_MouseDoubleClick(object sender, RoutedEventArgs e)
    {
        ShowMainWindow();
    }

    private void MiMain_Click(object sender, RoutedEventArgs e)
    {
        ShowMainWindow();
    }

    private void MiLogout_Click(object sender, RoutedEventArgs e)
    {
        Logout();
    }

    private void MiExit_Click(object sender, RoutedEventArgs e)
    {
        Exit();
    }
    #endregion
    #endregion

    #region 私有方法
    /// <summary>
    /// 获取配置
    /// </summary>
    /// <param name="key"></param>
    /// <param name="useCache"></param>
    /// <returns></returns>
    public async Task<ConfigDto> ListCfgAsync(string key, bool useCache = true)
    {
        if (useCache && _Cfg.TryGetValue(key, out var cachedCfg))
        {
            return cachedCfg;
        }

        var url = _Client.GetApiUrl("/scmcfg/option/" + key);

        var body = new Dictionary<string, string>();
        //body["client"] = "20";

        var head = GetHeader(null);

        var response = await HttpUtils.GetObjectAsync<ScmApiDataResponse<ConfigDto>>(url, body, head);
        if (response == null)
        {
            return null;
        }
        if (response.Code != 200)
        {
            ShowAlert(response.Message);
            return null;
        }

        var dic = response.Data;
        if (useCache)
        {
            _Cfg.TryAdd(key, dic);
        }
        return dic;
    }

    /// <summary>
    /// 获取字典
    /// </summary>
    /// <param name="key"></param>
    /// <param name="useCache"></param>
    /// <returns></returns>
    public async Task<List<ResOptionDvo>> ListDicAsync(string key, bool useCache = true)
    {
        if (useCache && _Dic.TryGetValue(key, out var cachedDic))
        {
            return cachedDic;
        }

        var url = _Client.GetApiUrl("/scmdic/option/" + key);

        var body = new Dictionary<string, string>();
        //body["client"] = "20";

        var head = GetHeader(null);

        var response = await HttpUtils.GetObjectAsync<ScmApiListResponse<ResOptionDvo>>(url, body, head);
        if (response == null)
        {
            return null;
        }
        if (response.Code != 200)
        {
            ShowAlert(response.Message);
            return null;
        }

        var dic = response.Data;
        if (useCache)
        {
            _Dic.TryAdd(key, dic);
        }
        return dic;
    }

    private Dictionary<string, string> GetHeader(Dictionary<string, string> head)
    {
        if (head == null)
        {
            head = new Dictionary<string, string>();
        }

        head["ApiToken"] = _AccessToken;
        head["Appkey"] = _AppKey;
        return head;
    }

    /// <summary>
    /// 事件实例化
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public AAction GetAction(MenuDto dto)
    {
        var className = dto.uri;
        if (string.IsNullOrEmpty(className))
        {
            return null;
        }

        var action = Assembly.GetEntryAssembly().CreateInstance(className) as AAction;
        if (action != null)
        {
            action.Window = this;
        }
        return action;
    }

    /// <summary>
    /// 显示主窗口
    /// </summary>
    private void ShowMainWindow()
    {
        if (Visibility != Visibility.Visible)
        {
            Visibility = Visibility.Visible;
        }

        if (WindowState != WindowState.Normal)
        {
            WindowState = WindowState.Normal;
        }

        if (!IsActive)
        {
            this.Activate();
        }

        Show();
    }

    /// <summary>
    /// 显示认证窗口
    /// </summary>
    private void ShowAuthWindow()
    {
        if (AppSettings.Instance.Env.LoginMode == Enums.ScmLoginTypeEnum.Terminal)
        {
            var window1 = new TerminalWindow();
            window1.Show();
            window1.Init(AppSettings.Instance, new ScmTerminal(ScmClientEnv.DataDir));
            return;
        }

        var window2 = new OperatorWindow();
        window2.Show();
        window2.Init(AppSettings.Instance, new ScmOperator(ScmClientEnv.DataDir));
        return;
    }

    /// <summary>
    /// 退出应用处理
    /// </summary>
    public void Exit(bool confirm = true)
    {
        if (confirm)
        {
            var result = ShowConfirm("确认要退出应用吗？");
            if (result != true)
            {
                if (IsVisible)
                {
                    this.Activate();
                }
                return;
            }
        }

        SqlHelper.Close();
        Application.Current.Shutdown();
    }

    /// <summary>
    /// 退出登录处理
    /// </summary>
    public async void Logout()
    {
        var result = ShowConfirm("确认要退出当前登录用户吗？");
        if (result != true)
        {
            return;
        }

        _Client.Logout();

        ShowAuthWindow();
        Close();
    }

    /// <summary>
    /// 设置是否自行启动
    /// </summary>
    /// <param name="autoStart"></param>
    public void SetAutoStart(bool autoStart)
    {
        var appPath = Assembly.GetEntryAssembly()?.Location;
        if (appPath == null)
        {
            appPath = Process.GetCurrentProcess().MainModule?.FileName;
        }

        var appName = "Scm.Net";

        if (autoStart)
        {
            OsHelper.EnableStartup(appName, appPath);
        }
        else
        {
            OsHelper.DisableStartup(appName);
        }
    }
    #endregion
}