using Com.Scm.Wpf.Dto;
using MahApps.Metro.IconPacks;
using System.Windows.Controls;

namespace Com.Scm.Wpf.Views.Uc
{
    /// <summary>
    /// UcMenubarView.xaml 的交互逻辑
    /// </summary>
    public partial class UcMenubarView : UserControl
    {
        private List<WpfMenuDto> _MenuList = new List<WpfMenuDto>();

        public UcMenubarView()
        {
            InitializeComponent();
        }

        public void Init(ScmWindow owner, List<WpfMenuDto> menuList)
        {
            foreach (var itemDto in menuList.Where(a => a.pid == 0).OrderBy(a => a.od))
            {
                _MenuList.Add(itemDto);

                var menu = new MenuItem();
                menu.Header = itemDto.namec;
                menu.Icon = GetIcon(itemDto.icon);
                menu.Tag = itemDto;
                MbMenu.Items.Add(menu);

                GenMenu(menu, itemDto, menuList);
            }
        }

        private int GenMenu(MenuItem parent, WpfMenuDto dto, List<WpfMenuDto> list)
        {
            var subList = list.Where(a => a.pid == dto.id).OrderBy(a => a.od).ToList();
            if (subList.Count < 1)
            {
                return 0;
            }

            var items = new List<WpfMenuDto>();
            foreach (var itemDto in subList)
            {
                items.Add(itemDto);

                var menu = new MenuItem();
                menu.Header = itemDto.namec;
                menu.Icon = GetIcon(itemDto.icon);
                menu.Tag = itemDto;
                parent.Items.Add(menu);

                var qty = GenMenu(menu, itemDto, list);
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

            var dto = menu.Tag as WpfMenuDto;
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
