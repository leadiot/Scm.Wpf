using Com.Scm.Api;
using Com.Scm.Config;
using Com.Scm.Sys.Config;
using Com.Scm.Sys.Menu;
using Com.Scm.Utils;
using Com.Scm.Wpf.Actions;
using Com.Scm.Wpf.Dto;
using Com.Scm.Wpf.Dto.Login;
using Com.Scm.Wpf.Dvo;
using HandyControl.Controls;
using System.Reflection;

namespace Com.Scm.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : HandyControl.Controls.Window, ScmWindow
{
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
    /// 数据字典
    /// </summary>
    private static Dictionary<string, List<ResOptionDvo>> _Dic = new Dictionary<string, List<ResOptionDvo>>();
    /// <summary>
    /// 系统配置
    /// </summary>
    private static Dictionary<string, ConfigDto> _Cfg = new Dictionary<string, ConfigDto>();

    private MainWindowDvo _Dvo;

    public MainWindow()
    {
        InitializeComponent();
    }

    public async Task Init(AppSettings appSettings, ScmTerminal scmTerminal)
    {

    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="result"></param>
    /// <param name="menus"></param>
    public void Init(LoginResult result, List<MenuDto> menus)
    {
        UserInfo = result.UserInfo;
        _AccessToken = result.AccessToken;
        _AppKey = "";

        _Dvo = new MainWindowDvo();
        _Dvo.Init(menus);
        this.DataContext = _Dvo;

        UcMenu.Init(this, menus);
        UcGuid.Init(this, menus);
        //UcGuid.Visibility = System.Windows.Visibility.Collapsed;
        UcTray.Init();

        Show();

        _Dvo.ShowHomeView();
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
            action.Owner = this;
        }
        return action;
    }

    #region ScmWindow 接口实现
    public void ShowView(string codec, string namec, string viewClass, bool useCache = true)
    {
        _Dvo.ShowView(codec, namec, viewClass);
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

    protected string GenUrl(string url)
    {
        return AppSettings.Instance.EnvConfig.GetApiUrl(url);
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