using Com.Scm.Sys.Menu;
using Com.Scm.Wpf.Dvo.Menu;
using MahApps.Metro.IconPacks;
using System.Windows.Controls;

namespace Com.Scm.Wpf.Views.Uc
{
    /// <summary>
    /// UcMenubarView.xaml 的交互逻辑
    /// </summary>
    public partial class UcMenubarView : UserControl
    {
        public UcMenubarView()
        {
            InitializeComponent();
        }

        public void Init(ScmWindow owner, List<MenuDto> menuList)
        {
            foreach (var itemDto in menuList.Where(a => a.pid == 0).OrderBy(a => a.od))
            {
                var itemDvo = ScmMenuDvo.FromDto(itemDto);

                var menu = new MenuItem();
                menu.Header = itemDto.namec;
                menu.Icon = GetIcon(itemDto.icon);
                menu.Tag = itemDvo;
                MbMenu.Items.Add(menu);

                GenMenu(menu, itemDvo, menuList);
            }
        }

        private int GenMenu(MenuItem parent, ScmMenuDvo dvo, List<MenuDto> list)
        {
            var subList = list.Where(a => a.pid == dvo.Id).OrderBy(a => a.od).ToList();
            if (subList.Count < 1)
            {
                return 0;
            }

            var items = new List<ScmMenuDvo>();
            foreach (var itemDto in subList)
            {
                var itemDvo = ScmMenuDvo.FromDto(itemDto);
                items.Add(itemDvo);

                var menu = new MenuItem();
                menu.Header = itemDto.namec;
                menu.Icon = GetIcon(itemDto.icon);
                menu.Tag = itemDvo;
                parent.Items.Add(menu);

                var qty = GenMenu(menu, itemDvo, list);
                if (qty == 0)
                {
                    menu.Click += Menu_Click;
                }
            }

            return items.Count;
        }

        private void Menu_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var menu = sender as MenuItem;
            if (menu == null)
            {
                return;
            }

            var dto = menu.Tag as ScmMenuDvo;
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

            return null;
            //return new PackIconMaterial { Kind = PackIconMaterialKind.None };
        }
    }
}
