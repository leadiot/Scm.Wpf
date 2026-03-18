using Com.Scm.Wpf.Dvo.Menu;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Com.Scm.Wpf
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
            LoadTestData();
            TvView.ItemsSource = Items;
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
}