using Com.Scm.Sys.Menu;
using Com.Scm.Utils;
using Com.Scm.Wpf.Dto;
using HandyControl.Controls;
using MahApps.Metro.IconPacks;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Com.Scm.Wpf.Views.Uc
{
    /// <summary>
    /// UcSideMenuView.xaml 的交互逻辑
    /// </summary>
    public partial class UcGuidView : UserControl
    {
        private ScmWindow _Owner;
        private ObservableCollection<MenuDto> MenuList = new ObservableCollection<MenuDto>();
        private Dictionary<string, List<SideMenuItem>> _MenuItems = new Dictionary<string, List<SideMenuItem>>();
        private List<SideMenuItem> _LastItems;

        public UcGuidView()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        public void Init(ScmWindow owner, List<WpfMenuDto> menuList)
        {
            _Owner = owner;

            foreach (var rootDto in menuList.Where(a => a.pid == 0).OrderBy(a => a.od))
            {
                MenuList.Add(rootDto);

                var itemList = menuList.Where(a => a.pid == rootDto.id).OrderBy(a => a.od).ToList();
                if (itemList.Count < 1)
                {
                    continue;
                }

                //var items = new List<WpfMenuDto>();
                var menus = new List<SideMenuItem>();
                foreach (var itemDto in itemList)
                {
                    //items.Add(itemDto);

                    var menu = new SideMenuItem();
                    menu.Header = itemDto.namec;
                    menu.Icon = GetIcon(itemDto.icon);
                    menu.Tag = itemDto;
                    MbMenu.Items.Add(menu);
                    menus.Add(menu);

                    var qty = GenMenu(menu, itemDto, menuList);
                    if (qty == 0)
                    {
                        menu.Selected += Menu_Selected;
                    }
                }
                //rootDto.children = items;
                _MenuItems[rootDto.codec] = menus;
            }

            LvMenu.ItemsSource = MenuList;
        }

        private int GenMenu(SideMenuItem parentMenu, WpfMenuDto parentDto, List<WpfMenuDto> list)
        {
            var subList = list.Where(a => a.pid == parentDto.id).OrderBy(a => a.od).ToList();
            if (subList.Count < 1)
            {
                return 0;
            }

            var items = new List<WpfMenuDto>();
            foreach (var itemDto in subList)
            {
                items.Add(itemDto);

                var menu = new SideMenuItem();
                menu.Header = itemDto.namec;
                menu.Icon = GetIcon(itemDto.icon);
                menu.Tag = itemDto;
                parentMenu.Items.Add(menu);

                var qty = GenMenu(menu, itemDto, list);
                if (qty == 0)
                {
                    menu.Selected += Menu_Selected;
                }
            }
            return items.Count;
        }

        private void Menu_Selected(object sender, System.Windows.RoutedEventArgs e)
        {
            var item = sender as SideMenuItem;
            if (item == null)
            {
                return;
            }

            var dto = item.Tag as WpfMenuDto;
            if (dto == null)
            {
                return;
            }

            var action = dto.Action;
            if (action == null)
            {
                return;
            }

            action.Execute(dto);
        }

        private PackIconMaterial GetIcon(string icon)
        {
            if (!string.IsNullOrEmpty(icon))
            {
                var kind = PackIconMaterialKind.Menu;
                if (Enum.TryParse<PackIconMaterialKind>(icon, out kind))
                {
                    return new PackIconMaterial { Kind = kind };
                }
            }
            return new PackIconMaterial { Kind = PackIconMaterialKind.Menu };
        }

        private void LvMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dto = LvMenu.SelectedItem as MenuDto;
            if (dto == null)
            {
                return;
            }

            if (_LastItems != null)
            {
                foreach (var item in _LastItems)
                {
                    item.Visibility = System.Windows.Visibility.Collapsed;
                }
            }

            _LastItems = _MenuItems[dto.codec];
            if (_LastItems != null)
            {
                foreach (var item in _LastItems)
                {
                    item.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        private void BtMenu_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MbMenu.Visibility == System.Windows.Visibility.Visible)
            {
                MbMenu.Visibility = System.Windows.Visibility.Collapsed;
                PiMenu.Kind = PackIconMaterialKind.ChevronDoubleRight;
                return;
            }

            MbMenu.Visibility = System.Windows.Visibility.Visible;
            PiMenu.Kind = PackIconMaterialKind.ChevronDoubleLeft;
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Grid gridtemp = (Grid)btn.Template.FindName("gridtemp", btn);
            Popup menuPop = (Popup)gridtemp.FindName("menuPop");
            menuPop.IsOpen = true;
        }
    }
}
