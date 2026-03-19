using Com.Scm.Helper;
using Com.Scm.Sys.Menu;
using Com.Scm.Dvo.Menu;
using Com.Scm.Models;
using Dm.util;
using NLog;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Com.Scm.Dvo
{
    public class MainWindowDvo : ScmDvo
    {
        private static readonly ILogger _Logger = LogManager.GetCurrentClassLogger();

        private MainWindow _Window;

        public ICommand OpenFileCommand { get; }
        public ICommand OpenDataCommand { get; }


        public ObservableCollection<ScmMenuDvo> MenuList { get; set; } = new ObservableCollection<ScmMenuDvo>();
        public ObservableCollection<TabModel> TabModels { get; set; } = new ObservableCollection<TabModel>();

        private int tabIndex;
        public int TabIndex { get { return tabIndex; } set { SetProperty(ref tabIndex, value); } }

        private bool isBlink = false;
        public bool IsBlink { get { return isBlink; } set { SetProperty(ref isBlink, value); } }

        private Visibility trayVisibility = Visibility.Visible;
        public Visibility TrayVisibility { get { return trayVisibility; } set { SetProperty(ref trayVisibility, value); } }

        public MainWindowDvo()
        {
            OpenFileCommand = new ScmCommand(OnOpenFile);
            OpenDataCommand = new ScmCommand(OnOpenData);
        }

        public void Init(MainWindow window, List<MenuDto> menuList)
        {
            _Window = window;

            if (menuList.isEmpty())
            {
                InitNasMenu(menuList);
                //InitTestMenu(menuList);
            }

            foreach (var itemDto in menuList)
            {
                var itemDvo = ScmMenuDvo.FromDto(itemDto);
                this.MenuList.Add(itemDvo);
            }
        }

        private void OnOpenFile(object sender)
        {
            try
            {
                OsHelper.OpenFolder(ScmClientEnv.FileDir);
            }
            catch (Exception exception)
            {
                _Logger.Error(exception);
            }
        }

        private void OnOpenData(object sender)
        {
            try
            {
                OsHelper.OpenFolder(ScmClientEnv.DataDir);
            }
            catch (Exception exception)
            {
                _Logger.Error(exception);
            }
        }

        private void InitNasMenu(List<MenuDto> menuList)
        {
            var item = new MenuDto();
            item.id = 1;
            item.icon = "Home";
            item.codec = "root";
            item.namec = "Home";
            item.uri = "Com.Scm.Actions.ViewAction";
            item.view = "Com.Scm.Views.Home.MainView";
            menuList.Add(item);

            item = new MenuDto();
            item.id = 21;
            item.icon = "FolderSync";
            item.codec = "nas-folder";
            item.namec = "目录管理";
            item.uri = "Com.Scm.Actions.ViewAction";
            item.view = "Com.Scm.Nas.Views.Folder.FolderView";
            item.module = "Nas.Wpf";
            menuList.Add(item);

            item = new MenuDto();
            item.id = 22;
            item.icon = "FileSync";
            item.codec = "nas-sync";
            item.namec = "同步日志";
            item.uri = "Com.Scm.Actions.ViewAction";
            item.view = "Com.Scm.Nas.Views.Sync.SyncView";
            item.module = "Nas.Wpf";
            menuList.Add(item);

            item = new MenuDto();
            item.id = 23;
            item.icon = "Folder";
            item.codec = "nas-native";
            item.namec = "本地目录";
            item.uri = "Com.Scm.Actions.ViewAction";
            item.view = "Com.Scm.Nas.Views.Native.NativeView";
            item.module = "Nas.Wpf";
            menuList.Add(item);

            item = new MenuDto();
            item.id = 24;
            item.icon = "Cloud";
            item.codec = "nas-remote";
            item.namec = "远端目录";
            item.uri = "Com.Scm.Actions.ViewAction";
            item.view = "Com.Scm.Nas.Views.Remote.RemoteView";
            item.module = "Nas.Wpf";
            menuList.Add(item);


            item = new MenuDto();
            item.id = 13;
            item.icon = "InformationBox";
            item.codec = "scm-about";
            item.namec = "关于软件";
            item.uri = "Com.Scm.Actions.ViewAction";
            item.view = "Com.Scm.Views.About.MainView";
            menuList.Add(item);
        }

        private void InitTestMenu(List<MenuDto> menuList)
        {
            var menu = new MenuDto();
            menu.id = 1;
            menu.icon = "Home";
            menu.codec = "root";
            menu.namec = "Home";
            menuList.Add(menu);

            var item = new MenuDto();
            item.id = 10;
            item.icon = "Responsive";
            item.codec = "demo";
            item.namec = "工具演示";
            item.pid = menu.id;
            item.uri = "Com.Scm.Actions.ViewAction";
            item.view = "Com.Scm.Views.Demo.DemoView";
            menuList.Add(item);

            item = new MenuDto();
            item.id = 11;
            item.icon = "DatabaseMarker";
            item.codec = "demo-native";
            item.namec = "本地数据";
            item.pid = menu.id;
            item.uri = "Com.Scm.Actions.ViewAction";
            item.view = "Com.Scm.Views.Samples.Native.MainView";
            item.module = "Scm.Samples";
            menuList.Add(item);

            item = new MenuDto();
            item.id = 12;
            item.icon = "DatabaseSync";
            item.codec = "demo-remote";
            item.namec = "远程数据";
            item.pid = menu.id;
            item.uri = "Com.Scm.Actions.ViewAction";
            item.view = "Com.Scm.Views.Samples.Remote.MainView";
            item.module = "Scm.Samples";
            menuList.Add(item);

            item = new MenuDto();
            item.id = 13;
            item.icon = "InformationBox";
            item.codec = "scm-about";
            item.namec = "关于软件";
            item.pid = menu.id;
            item.uri = "Com.Scm.Actions.ViewAction";
            item.view = "Com.Scm.Views.About.MainView";
            menuList.Add(item);

            menu = new MenuDto();
            menu.id = 20;
            menu.icon = "Harddisk";
            menu.codec = "nas";
            menu.namec = "松果云盘";
            //menu.pid = menu.id;
            menuList.Add(menu);

            item = new MenuDto();
            item.id = 21;
            item.icon = "FolderSync";
            item.codec = "nas-folder";
            item.namec = "目录管理";
            item.pid = menu.id;
            item.uri = "Com.Scm.Actions.ViewAction";
            item.view = "Com.Scm.Nas.Views.Folder.FolderView";
            item.module = "Nas.Wpf";
            menuList.Add(item);

            item = new MenuDto();
            item.id = 22;
            item.icon = "FileSync";
            item.codec = "nas-sync";
            item.namec = "同步日志";
            item.pid = menu.id;
            item.uri = "Com.Scm.Actions.ViewAction";
            item.view = "Com.Scm.Nas.Views.Sync.SyncView";
            item.module = "Nas.Wpf";
            menuList.Add(item);

            item = new MenuDto();
            item.id = 23;
            item.icon = "Folder";
            item.codec = "nas-native";
            item.namec = "本地目录";
            item.pid = menu.id;
            item.uri = "Com.Scm.Actions.ViewAction";
            item.view = "Com.Scm.Nas.Views.Native.NativeView";
            item.module = "Nas.Wpf";
            menuList.Add(item);

            item = new MenuDto();
            item.id = 24;
            item.icon = "Cloud";
            item.codec = "nas-remote";
            item.namec = "远端目录";
            item.pid = menu.id;
            item.uri = "Com.Scm.Actions.ViewAction";
            item.view = "Com.Scm.Nas.Views.Remote.RemoteView";
            item.module = "Nas.Wpf";
            menuList.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec"></param>
        /// <param name="namec"></param>
        /// <param name="view"></param>
        /// <param name="useCache"></param>
        public void ShowView(string codec, string namec, string view, string args = null, string module = null, bool useCache = false)
        {
            for (int i = 0; i < TabModels.Count; i++)
            {
                if (TabModels[i].Code == codec)
                {
                    TabIndex = i;
                    return;
                }
            }

            _Logger.Info($"MainWindow-ShowView:viewClass[{view}],useCache:[{useCache}]");
            if (string.IsNullOrWhiteSpace(view))
            {
                return;
            }

            Assembly assembly = null;
            if (!string.IsNullOrWhiteSpace(module))
            {
                assembly = Assembly.Load(module);
            }
            if (assembly == null)
            {
                assembly = Assembly.GetExecutingAssembly();
            }

            var scmView = assembly.CreateInstance(view) as ScmView;
            if (scmView == null)
            {
                _Logger.Error("MainWindow-ShowView:创建视图失败-" + view);
                return;
            }

            scmView.Init(_Window);

            TabModel tabs = new TabModel();
            tabs.Code = codec;
            tabs.Header = namec;
            tabs.Content = scmView.GetView();

            var index = TabModels.Count;
            TabModels.Add(tabs);

            TabIndex = index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec"></param>
        /// <param name="namec"></param>
        /// <param name="control"></param>
        public void ShowView(string codec, string namec, UserControl control)
        {
            for (int i = 0; i < TabModels.Count; i++)
            {
                if (TabModels[i].Code == codec)
                {
                    TabIndex = i;
                    return;
                }
            }

            TabModel tabs = new TabModel();
            tabs.Code = codec;
            tabs.Header = namec;
            tabs.Content = control;

            TabModels.Add(tabs);
            TabIndex = TabModels.Count - 1;
        }
    }
}
