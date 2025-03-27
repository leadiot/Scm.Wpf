using Com.Scm.Sys.Menu;
using Com.Scm.Utils;
using HandyControl.Controls;
using MahApps.Metro.IconPacks;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Com.Scm.Wpf.Views.Uc
{
    /// <summary>
    /// UcSideMenuView.xaml 的交互逻辑
    /// </summary>
    public partial class UcGuidView : UserControl
    {
        private ObservableCollection<MenuDto> MenuList = new ObservableCollection<MenuDto>();
        private Dictionary<string, List<SideMenuItem>> _MenuItems = new Dictionary<string, List<SideMenuItem>>();
        private List<SideMenuItem> _LastItems;

        public UcGuidView()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        public void Init(ScmClient client)
        {
            var dtoList = client.Menu;
            foreach (var rootDto in dtoList.Where(a => a.pid == 0).OrderBy(a => a.od))
            {
                MenuList.Add(rootDto);

                var itemList = dtoList.Where(a => a.pid == rootDto.id).OrderBy(a => a.od).ToList();
                if (itemList.Count < 1)
                {
                    continue;
                }

                var items = new List<MenuDto>();
                var menus = new List<SideMenuItem>();
                foreach (var itemDto in itemList)
                {
                    items.Add(itemDto);

                    var menu = new SideMenuItem();
                    menu.Header = itemDto.namec;
                    menu.Icon = GetIcon(itemDto.icon);
                    MbMenu.Items.Add(menu);
                    menus.Add(menu);

                    GenMenu(menu, itemDto, dtoList);
                }
                rootDto.children = items;
                _MenuItems[rootDto.codec] = menus;
            }

            LvMenu.ItemsSource = MenuList;
        }

        private void GenMenu(SideMenuItem menu, MenuDto parent, List<MenuDto> list)
        {
            var subList = list.Where(a => a.pid == parent.id).OrderBy(a => a.od).ToList();
            if (subList.Count < 1)
            {
                return;
            }

            var items = new List<MenuDto>();
            foreach (var dto in subList)
            {
                items.Add(dto);

                GenMenu(menu, dto, list);
            }
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
    }
}
