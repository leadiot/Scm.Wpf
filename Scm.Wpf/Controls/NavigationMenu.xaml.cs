using Com.Scm.Wpf.Dvo.Menu;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Com.Scm.Wpf.Controls
{
    public partial class NavigationMenu : UserControl
    {
        public static readonly DependencyProperty MenusProperty = DependencyProperty.Register(
            "Menus", typeof(IEnumerable<ScmMenuDvo>), typeof(NavigationMenu), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedItem", typeof(ScmMenuDvo), typeof(NavigationMenu), new PropertyMetadata(null));

        public event EventHandler<ScmMenuDvo> MenuItemSelected;

        public IEnumerable<ScmMenuDvo> Menus
        {
            get { return (IEnumerable<ScmMenuDvo>)GetValue(MenusProperty); }
            set { SetValue(MenusProperty, value); }
        }

        public ScmMenuDvo SelectedItem
        {
            get { return (ScmMenuDvo)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public NavigationMenu()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem treeViewItem && treeViewItem.DataContext is ScmMenuDvo menuItem)
            {
                // 自动展开子菜单
                treeViewItem.IsExpanded = true;

                SelectedItem = menuItem;
                MenuItemSelected?.Invoke(this, menuItem);
            }
        }
    }

    public class StringToVisibilityConverter : IValueConverter
    {
        public static readonly StringToVisibilityConverter Instance = new StringToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CollectionToVisibilityConverter : IValueConverter
    {
        public static readonly CollectionToVisibilityConverter Instance = new CollectionToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<ScmMenuDvo> list && list.Count > 0)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
