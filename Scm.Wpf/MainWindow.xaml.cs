using Com.Scm.Dvo;
using Com.Scm.Sys.Config;
using Com.Scm.Sys.Menu;
using Com.Scm.Utils;
using Com.Scm.Wpf.Actions;
using Com.Scm.Wpf.Dto;
using Com.Scm.Wpf.Dto.Login;
using Com.Scm.Wpf.Views.Home;
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
    /// 服务地址
    /// </summary>
    public const string SERVER_URL = "http://api.c-scm.net";
    /// <summary>
    /// 接口地址
    /// </summary>
    public const string API_URL = SERVER_URL + "/api";

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
    public ScmUserInfo User { get; private set; }
    /// <summary>
    /// 用户菜单
    /// </summary>
    public List<MenuDto> Menu { get; private set; }

    private static Dictionary<string, List<ResOptionDvo>> _Dic = new Dictionary<string, List<ResOptionDvo>>();
    private static Dictionary<string, ConfigDto> _Cfg = new Dictionary<string, ConfigDto>();

    public MainWindow()
    {
        InitializeComponent();
    }

    public void Init(LoginResult result, List<WpfMenuDto> menus)
    {
        //this.DataContext = new MenuDvo();
        foreach (var menu in menus)
        {
            menu.Action = GetAction(menu);
        }

        UcMenu.Init(this, menus);
        //UcGuid.Init(this, menus);
        UcGuid.Visibility = System.Windows.Visibility.Collapsed;
        UcInfo.Init();

        Show();

        ShowHomeView();
    }

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

        var head = new Dictionary<string, string>();
        head["Accesstoken"] = _AccessToken;
        head["Appkey"] = _AppKey;

        var response = await HttpUtils.GetObjectAsync<ScmListResponse<ResOptionDvo>>(url, body, head);
        if (response == null)
        {
            return null;
        }
        if (response.Code != 200)
        {
            ShowAlert(response.Message);
            return null;
        }

        var dic = response.data;
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

        var head = new Dictionary<string, string>();
        head["Accesstoken"] = _AccessToken;
        head["Appkey"] = _AppKey;

        var response = await HttpUtils.GetObjectAsync<ScmDataResponse<ConfigDto>>(url, body, head);
        if (response == null)
        {
            return null;
        }
        if (response.Code != 200)
        {
            ShowAlert(response.Message);
            return null;
        }

        var dic = response.data;
        if (useCache)
        {
            _Cfg[key] = dic;
        }
        return dic;
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

        if (head == null)
        {
            head = new Dictionary<string, string>();
        }
        head["Accesstoken"] = _AccessToken;
        head["Appkey"] = _AppKey;

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
    public async Task<string> PostJsonStringAsync(string url, string body = null, Dictionary<string, string> head = null)
    {
        url = GenUrl(url);

        if (head == null)
        {
            head = new Dictionary<string, string>();
        }
        head["Accesstoken"] = _AccessToken;
        head["Appkey"] = _AppKey;

        return await HttpUtils.PostJsonStringAsync(url, null, head);
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

        if (head == null)
        {
            head = new Dictionary<string, string>();
        }
        head["Accesstoken"] = _AccessToken;
        head["Appkey"] = _AppKey;

        var json = body != null ? body.ToJsonString() : null;
        var response = await HttpUtils.PostJsonObjectAsync<ScmDataResponse<T>>(url, json, head);
        if (response == null)
        {
            return default;
        }
        if (response.Success)
        {
            ShowAlert(response.Message);
            return default;
        }

        return response.data;
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

        if (head == null)
        {
            head = new Dictionary<string, string>();
        }
        head["Accesstoken"] = _AccessToken;
        head["Appkey"] = _AppKey;

        var response = await HttpUtils.PostJsonObjectAsync<ScmDataResponse<T>>(url, body, head);
        if (response == null)
        {
            return default;
        }
        if (response.Success)
        {
            ShowAlert(response.Message);
            return default;
        }

        return response.data;
    }

    /// <summary>
    /// GET请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task<string> GetFormStringAsync(string url, Dictionary<string, string> body = null, Dictionary<string, string> head = null)
    {
        url = GenUrl(url);

        if (head == null)
        {
            head = new Dictionary<string, string>();
        }
        head["Accesstoken"] = _AccessToken;
        head["Appkey"] = _AppKey;

        return await HttpUtils.GetStringAsync(url, body, head);
    }

    /// <summary>
    /// GET请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task<T> GetFormObjectAsync<T>(string url, Dictionary<string, string> body = null, Dictionary<string, string> head = null)
    {
        url = GenUrl(url);

        if (head == null)
        {
            head = new Dictionary<string, string>();
        }
        head["Accesstoken"] = _AccessToken;
        head["Appkey"] = _AppKey;

        var response = await HttpUtils.GetObjectAsync<ScmDataResponse<T>>(url, body, head);
        if (response == null)
        {
            return default;
        }
        if (response.Success)
        {
            ShowAlert(response.Message);
            return default;
        }

        return response.data;
    }

    #endregion

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
        return API_URL + url;
    }

    public void ShowInfo(string message)
    {

    }

    /// <summary>
    /// 显示提示（弹窗，但不是需要交互）
    /// </summary>
    public void ShowToast(string message)
    {

    }

    /// <summary>
    /// 显示提示（弹窗，中止当前操作）
    /// </summary>
    public void ShowAlert(string message)
    {

    }
}