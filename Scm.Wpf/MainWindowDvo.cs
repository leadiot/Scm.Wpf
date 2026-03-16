using Com.Scm.Sys.Menu;
using Com.Scm.Utils;
using Com.Scm.Wpf.Dvo.Menu;
using Com.Scm.Wpf.Models;
using Com.Scm.Wpf.Views.Home;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;

namespace Com.Scm.Wpf.Dvo
{
    public class MainWindowDvo : ScmDvo
    {
        private MainWindow _Window;

        public ICommand OpenFileCommand { get; }
        public ICommand OpenDataCommand { get; }


        public ObservableCollection<MenuDvo> MenuList { get; set; } = new ObservableCollection<MenuDvo>();
        public ObservableCollection<TabModel> TabModels { get; set; } = new ObservableCollection<TabModel>();

        private int tabIndex;
        public int TabIndex { get { return tabIndex; } set { SetProperty(ref tabIndex, value); } }

        private HomeView _HomeView;

        public MainWindowDvo()
        {
            OpenFileCommand = new ScmCommand(OnOpenFile);
            OpenDataCommand = new ScmCommand(OnOpenData);
        }

        public void Init(MainWindow window, List<MenuDto> menuList)
        {
            _Window = window;

            InitTestMenu(menuList);

            foreach (var itemDto in menuList)
            {
                var itemDvo = MenuDvo.FromDto(itemDto);
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
                LogUtils.Error(exception);
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
                LogUtils.Error(exception);
            }
        }

        private void InitTestMenu(List<MenuDto> menuList)
        {
            var menu = new MenuDto();
            menu.id = 1;
            menu.codec = "root";
            menu.namec = "Home";
            menuList.Add(menu);

            var item = new MenuDto();
            item.id = 10;
            item.codec = "demo";
            item.namec = "工具演示";
            item.pid = menu.id;
            item.uri = "Com.Scm.Wpf.Actions.ViewAction";
            item.view = "Com.Scm.Wpf.Views.Demo.DemoView";
            menuList.Add(item);

            item = new MenuDto();
            item.id = 11;
            item.codec = "demo-native";
            item.namec = "本地数据";
            item.pid = menu.id;
            item.uri = "Com.Scm.Wpf.Actions.ViewAction";
            item.view = "Com.Scm.Wpf.Views.Samples.Native.MainView";
            menuList.Add(item);

            item = new MenuDto();
            item.id = 12;
            item.codec = "demo-remote";
            item.namec = "远程数据";
            item.pid = menu.id;
            item.uri = "Com.Scm.Wpf.Actions.ViewAction";
            item.view = "Com.Scm.Wpf.Views.Samples.Remote.MainView";
            menuList.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec"></param>
        /// <param name="namec"></param>
        /// <param name="viewClass"></param>
        /// <param name="useCache"></param>
        public void ShowView(string codec, string namec, string viewClass, bool useCache = false)
        {
            for (int i = 0; i < TabModels.Count; i++)
            {
                if (TabModels[i].Code == codec)
                {
                    TabIndex = i;
                    return;
                }
            }

            LogUtils.Info($"MainWindow-ShowView:viewClass[{viewClass}],useCache:[{useCache}]");
            if (string.IsNullOrWhiteSpace(viewClass))
            {
                return;
            }

            var view = Assembly.GetEntryAssembly().CreateInstance(viewClass) as ScmView;
            if (view == null)
            {
                LogUtils.Error("MainWindow-ShowView:创建视图失败-" + viewClass);
                return;
            }

            view.Init(_Window);

            TabModel tabs = new TabModel();
            tabs.Code = codec;
            tabs.Header = namec;
            tabs.Content = view.GetView();

            TabModels.Add(tabs);
            TabIndex = TabModels.Count - 1;
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

        /// <summary>
        /// 显示首页
        /// </summary>
        public void ShowHomeView()
        {
            if (_HomeView == null)
            {
                _HomeView = new HomeView();
                _HomeView.Init(_Window);
            }
            ShowView("home", "首页", _HomeView);
        }
    }
}
