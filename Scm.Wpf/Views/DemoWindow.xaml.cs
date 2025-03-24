using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace Com.Scm.Wpf.Views
{
    /// <summary>
    /// DemoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DemoWindow : HandyControl.Controls.Window
    {
        public ObservableCollection<MenuItemTtt> MenuItems { get; set; }

        public DemoWindow()
        {
            InitializeComponent();
        }
    }

    public class MenuItemTtt
    {
        public string Title { get; set; }
        public Geometry Icon { get; set; }
        public FrameworkElement Content { get; set; }
    }
}
