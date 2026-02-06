using Com.Scm.Wpf.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;

namespace Com.Scm.Wpf.Dvo
{
    public partial class MainViewModel : ScmDvo
    {
        [ObservableProperty]
        private ObservableCollection<ModuleGroup> moduleGroups;

        [ObservableProperty]
        private ObservableCollection<TabModel> tabModels;

        [ObservableProperty]
        private int tabIndex;

        [ObservableProperty]
        private ObservableCollection<Modules> modules;

        [ObservableProperty]
        private FrameworkElement mainContent;

        public RelayCommand<object> changeContentCmd { get; set; }
        public RelayCommand ExpandMenuCmd { get; set; }

        public void Init()
        {
            ModuleGroups = new ObservableCollection<ModuleGroup>();
            TabModels = new ObservableCollection<TabModel>();
            Modules = new ObservableCollection<Modules>();
            Modules modulesModel = new Modules
            {
                Code = "\ue600",
                Name = "用户管理",
                TypeName = "system.user.User"
            };
            Modules.Add(modulesModel);
            TabIndex = 0;

            changeContentCmd = new RelayCommand<object>(NavChanged);
            ExpandMenuCmd = new RelayCommand(() => {
                for (int i = 0; i < ModuleGroups.Count; i++)
                {
                    var item = ModuleGroups[i];
                    item.ContractionTemplate = !item.ContractionTemplate;
                }
                //Messenger.Default.Send("", "ExpandMenu");
            });
            getMenu();
            NavChanged("home");
        }

        private void NavChanged(object o)
        {
            string typeName;
            string tabName;
            if (o.ToString() == "home")
            {
                typeName = "home";
                tabName = "首页";
            }
            else
            {
                var values = (object[])o;
                typeName = values[0].ToString();
                tabName = values[1].ToString();
            }

            Type type = Type.GetType("ZrClient.View." + typeName);
            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
            bool needAdd = true;
            for (int i = 0; i < TabModels.Count; i++)
            {
                if (TabModels[i].Code == o.ToString())
                {
                    TabIndex = i;
                    needAdd = false;
                    break;
                }
            }
            if (needAdd)
            {
                TabModel tabs = new TabModel();
                tabs.Header = tabName;
                tabs.Code = o.ToString();
                tabs.Content = (FrameworkElement)constructor.Invoke(null);
                TabModels.Add(tabs);
                TabIndex = TabModels.Count - 1;
            }
            this.MainContent = (FrameworkElement)constructor.Invoke(null);
        }

        private void getMenu()
        {
            MenuApi mApi = new MenuApi();
            Task.Run(new Action(async () => {
                var menu = await mApi.getGroup();
                ModuleGroups.Clear();
                foreach (var item in menu)
                {
                    ModuleGroups.Add(item);
                }
            }));
        }
    }
}
