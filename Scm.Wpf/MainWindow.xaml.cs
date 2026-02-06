using Com.Scm.Api;
using Com.Scm.Config;
using Com.Scm.Sys.Config;
using Com.Scm.Utils;
using Com.Scm.Wpf.Actions;
using Com.Scm.Wpf.Dto;
using Com.Scm.Wpf.Dto.Login;
using Com.Scm.Wpf.Dvo;
using Com.Scm.Wpf.Views.Home;
using HandyControl.Controls;
using System.Reflection;

namespace Com.Scm.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : HandyControl.Controls.Window, ScmWindow
{
    private Dictionary<string, ScmView> _Views = new Dictionary<string, ScmView>();
    private HomeView _HomeView;

    /// <summary>
    /// 访问凭据
    /// </summary>
    private string _AccessToken;
    /// <summary>
    /// 应用代码
    /// </summary>
    private string _AppKey = "";

    /// <summary>
    /// 当前用户
    /// </summary>
    public ScmUserInfo UserInfo { get; private set; }
    /// <summary>
    /// 用户菜单
    /// </summary>
    public List<WpfMenuDto> MenuList { get; private set; }

    /// <summary>
    /// 数据字典
    /// </summary>
    private static Dictionary<string, List<ResOptionDvo>> _Dic = new Dictionary<string, List<ResOptionDvo>>();
    /// <summary>
    /// 系统配置
    /// </summary>
    private static Dictionary<string, ConfigDto> _Cfg = new Dictionary<string, ConfigDto>();

    private MainViewModel _Dvo;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void LoadTestMenu(List<WpfMenuDto> menuList)
    {
        menuList.Add(new WpfMenuDto { id = 1, codec = "1", namec = "test", uri = "" });
    }

    public async Task Init(AppSettings appSettings, ScmTerminal scmTerminal)
    {

    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="result"></param>
    /// <param name="menus"></param>
    public void Init(LoginResult result, List<WpfMenuDto> menus)
    {
        UserInfo = result.UserInfo;
        _AccessToken = result.AccessToken;
        _AppKey = "";
        MenuList = menus;
        LoadTestMenu(menus);

        _Dvo = new MainViewModel();
        _Dvo.Init();
        this.DataContext = _Dvo;

        foreach (var menu in menus)
        {
            menu.Action = GetAction(menu);
        }

        UcMenu.Init(this, menus);
        //UcGuid.Init(this, menus);
        UcGuid.Visibility = System.Windows.Visibility.Collapsed;
        UcTray.Init();

        Show();

        ShowHomeView();
    }

    /// <summary>
    /// 事件实例化
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public AAction GetAction(WpfMenuDto dto)
    {
        var className = dto.uri;
        if (string.IsNullOrEmpty(className))
        {
            return null;
        }

        var action = Assembly.GetEntryAssembly().CreateInstance(className) as AAction;
        if (action != null)
        {
            action.Owner = this;
        }
        return action;
    }

    #region ScmWindow 接口实现
    public void ShowView(string viewClass, bool useCache = true)
    {
        LogUtils.Info($"MainWindow-ShowView:viewClass[{viewClass}],useCache:[{useCache}]");
        if (string.IsNullOrWhiteSpace(viewClass))
        {
            return;
        }

        if (useCache)
        {
            if (_Views.ContainsKey(viewClass))
            {
                ShowView(_Views[viewClass]);
                return;
            }
        }

        var view = Assembly.GetEntryAssembly().CreateInstance(viewClass) as ScmView;
        if (view == null)
        {
            LogUtils.Error("MainWindow-ShowView:创建视图失败-" + viewClass);
            return;
        }

        view.Init(this);
        //var view = Activator.CreateInstance(viewClass) as ScmView;
        ShowView(view);

        if (useCache)
        {
            _Views[viewClass] = view;
        }
    }

    private void ShowView(ScmView view)
    {
        GdView.Children.Clear();

        if (view == null)
        {
            return;
        }

        var control = view.GetView();
        if (control != null)
        {
            GdView.Children.Add(control);
        }
    }

    private Dictionary<string, string> GetHeader(Dictionary<string, string> head)
    {
        if (head == null)
        {
            head = new Dictionary<string, string>();
        }

        head["Accesstoken"] = _AccessToken;
        head["Appkey"] = _AppKey;
        return head;
    }

    /// <summary>
    /// 获取字典
    /// </summary>
    /// <param name="key"></param>
    /// <param name="useCache"></param>
    /// <returns></returns>
    public async Task<List<ResOptionDvo>> ListDicAsync(string key, bool useCache = true)
    {
        if (useCache)
        {
            if (_Dic.ContainsKey(key))
            {
                return _Dic[key];
            }
        }

        var url = GenUrl("/scmdic/option/" + key);

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
            _Dic[key] = dic;
        }
        return dic;
    }

    /// <summary>
    /// 获取配置
    /// </summary>
    /// <param name="key"></param>
    /// <param name="useCache"></param>
    /// <returns></returns>
    public async Task<ConfigDto> ListCfgAsync(string key, bool useCache = true)
    {
        if (useCache)
        {
            if (_Cfg.ContainsKey(key))
            {
                return _Cfg[key];
            }
        }

        var url = GenUrl("/scmcfg/option/" + key);

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
            _Cfg[key] = dic;
        }
        return dic;
    }

    /// <summary>
    /// GET请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task<T> GetObjectAsync<T>(string url, Dictionary<string, string> body = null, Dictionary<string, string> head = null)
    {
        url = GenUrl(url);

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
        url = GenUrl(url);

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
        url = GenUrl(url);

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
        url = GenUrl(url);

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
        url = GenUrl(url);

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
        url = GenUrl(url);

        head = GetHeader(head);

        return await HttpUtils.PostJsonStringAsync(url, null, head);
    }
    #endregion

    /// <summary>
    /// 显示首页
    /// </summary>
    private void ShowHomeView()
    {
        GdView.Children.Clear();

        if (_HomeView == null)
        {
            _HomeView = new HomeView();
            _HomeView.Init(this);
        }
        GdView.Children.Add(_HomeView);
    }

    protected string GenUrl(string url)
    {
        return AppSettings.EnvConfig.GetApiUrl(url);
    }

    /// <summary>
    /// 显示提示（标签，不需要交互）
    /// </summary>
    /// <param name="message"></param>
    public void ShowInfo(string message)
    {
        LogUtils.Info("ShowInfo:" + message);
        UcTray.ShowInfo(message);
    }

    /// <summary>
    /// 显示提示（弹窗，但不是需要交互）
    /// </summary>
    public void ShowToast(string message)
    {
        LogUtils.Info("ShowToast:" + message);
        Growl.Info(message);
    }

    /// <summary>
    /// 显示提示（弹窗，中止当前操作）
    /// </summary>
    public void ShowAlert(string message)
    {
        LogUtils.Info("ShowAlert:" + message);
        MessageBox.Show(message);
    }

    private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {

    }

    private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
    {

    }
}