using Com.Scm.Sys.Menu;
using Com.Scm.Utils;
using Com.Scm.Wpf.Actions;
using Com.Scm.Wpf.Dvo.Menu;
using HandyControl.Controls;
using MahApps.Metro.IconPacks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Com.Scm.Wpf.Views.Uc
{
    /// <summary>
    /// UcSideMenuView.xaml 的交互逻辑
    /// </summary>
    public partial class UcGuidView : UserControl
    {
        private ScmWindow _Owner;
        private ObservableCollection<ScmMenuDvo> MenuList = new ObservableCollection<ScmMenuDvo>();
        private List<SideMenuItem> _LastItems;

        public UcGuidView()
        {
            InitializeComponent();
        }

        public void Init(ScmWindow owner, List<MenuDto> menuList)
        {
            _Owner = owner;

            foreach (var itemDto in menuList.Where(a => a.pid == 0).OrderBy(a => a.od))
            {
                var itemDvo = ScmMenuDvo.FromDto(itemDto);

                var menu = new SideMenuItem();
                menu.Header = itemDto.namec;
                menu.Icon = GetIcon(itemDto.icon, 16);
                menu.Tag = itemDvo;
                menu.FontWeight = FontWeights.Bold;
                //menu.Background = Brushes.Transparent;
                MbMenu.Items.Add(menu);

                GenMenu(menu, itemDvo, menuList);
            }
        }

        private void GenMenu(SideMenuItem parentMenu, ScmMenuDvo parentDto, List<MenuDto> list)
        {
            var subList = list.Where(a => a.pid == parentDto.Id).OrderBy(a => a.od).ToList();
            if (subList.Count < 1)
            {
                return;
            }

            var items = new List<ScmMenuDvo>();
            foreach (var itemDto in subList)
            {
                var itemDvo = ScmMenuDvo.FromDto(itemDto);
                items.Add(itemDvo);

                var menu = new SideMenuItem();
                menu.Header = itemDto.namec;
                menu.Icon = GetIcon(itemDto.icon, 12);
                menu.Tag = itemDvo;
                menu.FontWeight = FontWeights.Normal;
                //menu.Background = Brushes.Transparent;
                parentMenu.Items.Add(menu);

                GenMenu(menu, itemDvo, list);
                if (menu.Items.Count < 1)
                {
                    menu.Selected += Menu_Selected;
                }
            }
        }

        private void Menu_Selected(object sender, System.Windows.RoutedEventArgs e)
        {
            var item = sender as SideMenuItem;
            if (item == null)
            {
                return;
            }

            var dvo = item.Tag as ScmMenuDvo;
            if (dvo == null)
            {
                return;
            }

            var action = dvo.Action;
            if (action == null)
            {
                if (dvo.Loaded)
                {
                    return;
                }

                dvo.Loaded = true;
                if (string.IsNullOrEmpty(dvo.Uri))
                {
                    return;
                }

                Type type = Type.GetType(dvo.Uri);
                if (type == null)
                {
                    return;
                }
                var obj = Activator.CreateInstance(type);
                if (!(obj is AAction))
                {
                    return;
                }

                action = (AAction)obj;
                action.Window = _Owner;
                dvo.Action = action;
            }

            action.Execute(dvo);
        }

        private PackIconMaterial GetIcon(string icon, int size)
        {
            if (!string.IsNullOrEmpty(icon))
            {
                var kind = PackIconMaterialKind.Menu;
                if (Enum.TryParse<PackIconMaterialKind>(icon, out kind))
                {
                    return new PackIconMaterial { Kind = kind, FontSize = size };
                }
            }
            return new PackIconMaterial { Kind = PackIconMaterialKind.Menu, FontSize = size, RenderSize = new System.Windows.Size(size, size) };
        }

        private void LvMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var dto = LvMenu.SelectedItem as MenuDto;
            //if (dto == null)
            //{
            //    return;
            //}

            //if (_LastItems != null)
            //{
            //    foreach (var item in _LastItems)
            //    {
            //        item.Visibility = System.Windows.Visibility.Collapsed;
            //    }
            //}

            //_LastItems = _MenuItems[dto.codec];
            //if (_LastItems != null)
            //{
            //    foreach (var item in _LastItems)
            //    {
            //        item.Visibility = System.Windows.Visibility.Visible;
            //    }
            //}
        }

        private void BtMenu_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //if (MbMenu.Visibility == System.Windows.Visibility.Visible)
            //{
            //    MbMenu.Visibility = System.Windows.Visibility.Collapsed;
            //    PiMenu.Kind = PackIconMaterialKind.ChevronDoubleRight;
            //    return;
            //}

            //MbMenu.Visibility = System.Windows.Visibility.Visible;
            //PiMenu.Kind = PackIconMaterialKind.ChevronDoubleLeft;
        }
    }
}
