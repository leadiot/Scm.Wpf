using Com.Scm.Config;
using Com.Scm.Dvo.Login;
using Com.Scm.Wpf;

namespace Com.Scm.Login
{
    /// <summary>
    /// 设备绑定窗口的交互逻辑
    /// </summary>
    public partial class BindWindow : HandyControl.Controls.Window
    {
        private BindDvo _Dvo;
        private AppSettings _AppSettings;
        private ScmTerminal _ScmTerminal;

        public BindWindow()
        {
            InitializeComponent();
        }

        public void Init(AppSettings appSettings, ScmTerminal scmTerminal)
        {
            _AppSettings = appSettings;
            _ScmTerminal = scmTerminal;
            _Dvo = new BindDvo();

            this.DataContext = _Dvo;
        }

        private void BtVerify_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DoBindAsync();
        }

        /// <summary>
        /// 执行绑定操作
        /// </summary>
        private async void DoBindAsync()
        {
            var body = _Dvo.GetBind();
            if (body == null)
            {
                return;
            }

            _ScmTerminal.Init(_Dvo.Host);
            var result = await _ScmTerminal.BindAsync(body);
            if (!result)
            {
                return;
            }

            //var list = await _ScmTerminal.PostFormObjectAsync<List<NasResFileDao>>(NasEnv.InitUrl);

            //var configList = new List<NasCfgConfigDao>();
            //foreach (var item in list)
            //{
            //    var cfg = new NasCfgConfigDao();
            //    cfg.key = item.name;
            //    cfg.value = item.id.ToString();
            //    cfg.PrepareCreate();
            //    configList.Add(cfg);
            //}

            //var sqlClient = SqlHelper.GetSqlClient();
            //await sqlClient.Deleteable<NasCfgConfigDao>().ExecuteCommandAsync();
            //await sqlClient.Insertable(configList).ExecuteCommandAsync();

            ShowMain();
        }

        /// <summary>
        /// 显示主窗口
        /// </summary>
        /// <param name="client"></param>
        private async void ShowMain()
        {
            var nasWindow = new MainWindow();
            nasWindow.Show();
            await nasWindow.Init(_AppSettings, _ScmTerminal);
            this.Close();

            if (!nasWindow.IsActive)
            {
                nasWindow.Activate();
            }
        }
    }
}
