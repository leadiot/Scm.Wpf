using Com.Scm.Dvo;
using Com.Scm.Dvo.Menu;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Com.Scm
{
    /// <summary>
    /// DemoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DemoWindow : HandyControl.Controls.Window
    {
        private ObservableCollection<ItemViewModel> _items = new ObservableCollection<ItemViewModel>();
        public ObservableCollection<ItemViewModel> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        public DemoWindow()
        {
            InitializeComponent();

            this.DataContext = new MainViewModel();

            //LoadTestData();
            //TvView.ItemsSource = Items;
        }

        private void LoadTestData()
        {
            Items = new ObservableCollection<ItemViewModel>()
            {
                new ItemViewModel
                {
                    DisplayText="中国人",
                    Children=new ObservableCollection<ItemViewModel>
                    {
                        new ItemViewModel{DisplayText="马云"},
                        new ItemViewModel{DisplayText="马化腾"},
                        new ItemViewModel{
                            DisplayText="WPF UI作者",
                            Children=new ObservableCollection<ItemViewModel>(){
                                new ItemViewModel{ DisplayText="身价：100亿"},
                                new ItemViewModel{ DisplayText="老婆数：100个"},
                            }
                        },
                    }
                },
                new ItemViewModel
                {
                    DisplayText="歪果人",
                    Children=new ObservableCollection<ItemViewModel>
                    {
                        new ItemViewModel{DisplayText="乔布斯"},
                        new ItemViewModel{DisplayText="巴菲特"},
                    }
                },
                new ItemViewModel
                {
                    DisplayText="中国人",
                    Children=new ObservableCollection<ItemViewModel>
                    {
                        new ItemViewModel{DisplayText="马云"},
                        new ItemViewModel{DisplayText="马化腾"},
                        new ItemViewModel{DisplayText="WPF UI作者"},
                    }
                },
                new ItemViewModel
                {
                    DisplayText="歪果人",
                    Children=new ObservableCollection<ItemViewModel>
                    {
                        new ItemViewModel{DisplayText="乔布斯"},
                        new ItemViewModel{DisplayText="巴菲特"},
                    }
                },
                new ItemViewModel
                {
                    DisplayText="中国人",
                    Children=new ObservableCollection<ItemViewModel>
                    {
                        new ItemViewModel{DisplayText="马云"},
                        new ItemViewModel{DisplayText="马化腾"},
                        new ItemViewModel{DisplayText="WPF UI作者"},
                    }
                },
                new ItemViewModel
                {
                    DisplayText="歪果人",
                    Children=new ObservableCollection<ItemViewModel>
                    {
                        new ItemViewModel{DisplayText="乔布斯"},
                        new ItemViewModel{DisplayText="巴菲特"},
                    }
                },
                new ItemViewModel
                {
                    DisplayText="中国人",
                    Children=new ObservableCollection<ItemViewModel>
                    {
                        new ItemViewModel{DisplayText="马云"},
                        new ItemViewModel{DisplayText="马化腾"},
                        new ItemViewModel{DisplayText="WPF UI作者"},
                    }
                },
                new ItemViewModel
                {
                    DisplayText="歪果人",
                    Children=new ObservableCollection<ItemViewModel>
                    {
                        new ItemViewModel{DisplayText="乔布斯"},
                        new ItemViewModel{DisplayText="巴菲特"},
                    }
                },
            };
        }

        private void NavigationMenu_MenuItemSelected(object sender, ScmMenuDvo e)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class ItemViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public string DisplayText { get; set; }
        public ObservableCollection<ItemViewModel> Children { get; set; } = new ObservableCollection<ItemViewModel>();
    }

    public class TreeItemMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var left = 0.0;
            UIElement element = value as TreeViewItem;
            while (element != null && element.GetType() != typeof(TreeView))
            {
                element = (UIElement)VisualTreeHelper.GetParent(element);
                if (element is TreeViewItem)
                    left += 18.0;
            }
            return new Thickness(left, 0, 0, 0);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class MenuItemViewModel : ScmDvo
    {
        private string _header;
        public string Header { get { return _header; } set { SetProperty(ref _header, value); } }

        private string _icon;
        public string Icon { get { return _icon; } set { SetProperty(ref _icon, value); } }

        private bool _isExpanded;
        public bool IsExpanded { get { return _isExpanded; } set { SetProperty(ref _isExpanded, value); } }

        private bool _isSelected;
        public bool IsSelected { get { return _isSelected; } set { SetProperty(ref _isSelected, value); } }

        private ObservableCollection<MenuItemViewModel> _children;
        public ObservableCollection<MenuItemViewModel> Children { get { return _children; } set { SetProperty(ref _children, value); } }

        // 命令：处理点击展开/选中
        public ICommand ToggleCommand { get; }

        public MenuItemViewModel()
        {
            Children = new ObservableCollection<MenuItemViewModel>();
            ToggleCommand = new ScmCommand(ExecuteToggle);
        }

        private void ExecuteToggle()
        {
            if (Children != null && Children.Count > 0)
            {
                // 如果有子菜单，切换展开状态
                IsExpanded = !IsExpanded;
                // 可选：展开时自动选中父节点，或者不选中
                // IsSelected = true; 
            }
            else
            {
                // 如果是叶子节点，则设为选中
                IsSelected = true;

                // 在这里可以触发导航逻辑，例如发布消息让主区域切换页面
                // Messenger.Default.Send(new NavigationMessage(this.Header));
            }
        }
    }

    public partial class MainViewModel : ScmDvo
    {
        public ObservableCollection<MenuItemViewModel> MenuItems { get; }

        public MainViewModel()
        {
            MenuItems = new ObservableCollection<MenuItemViewModel>
            {
                new MenuItemViewModel { Header = "仪表盘", Icon = "📊" },
                new MenuItemViewModel
                {
                    Header = "系统管理",
                    Icon = "⚙️",
                    IsExpanded = true,
                    Children = new ObservableCollection<MenuItemViewModel>
                    {
                        new MenuItemViewModel { Header = "用户管理", Icon = "👥" },
                        new MenuItemViewModel
                        {
                            Header = "权限配置",
                            Icon = "🔒",
                            Children = new ObservableCollection<MenuItemViewModel>
                            {
                                new MenuItemViewModel { Header = "角色列表", Icon = "📋" },
                                new MenuItemViewModel { Header = "菜单设置", Icon = "☰" }
                            }
                        }
                    }
                },
                new MenuItemViewModel { Header = "报表中心", Icon = "📈" },
                new MenuItemViewModel { Header = "退出系统", Icon = "🚪" }
            };
        }
    }
}