using Com.Scm.Sys.Menu;
using MahApps.Metro.IconPacks;
using System.Windows.Controls;

namespace Com.Scm.Wpf.Views.Uc
{
    /// <summary>
    /// UcMenubarView.xaml 的交互逻辑
    /// </summary>
    public partial class UcMenubarView : UserControl
    {
        private List<MenuDto> _MenuList = new List<MenuDto>();

        public UcMenubarView()
        {
            InitializeComponent();
        }

        public void Init(ScmClient client)
        {
            var dtoList = client.Menu;
            foreach (var itemDto in dtoList.Where(a => a.pid == 0).OrderBy(a => a.od))
            {
                _MenuList.Add(itemDto);

                var menu = new MenuItem();
                menu.Header = itemDto.namec;
                menu.Icon = GetIcon(itemDto.icon);
                MbMenu.Items.Add(menu);

                GenMenu(menu, itemDto, dtoList);
            }
        }

        private void GenMenu(MenuItem parent, MenuDto dto, List<MenuDto> list)
        {
            var subList = list.Where(a => a.pid == dto.id).OrderBy(a => a.od).ToList();
            if (subList.Count < 1)
            {
                return;
            }

            var items = new List<MenuDto>();
            foreach (var itemDto in subList)
            {
                items.Add(itemDto);

                var menu = new MenuItem();
                menu.Header = itemDto.namec;
                menu.Icon = GetIcon(itemDto.icon);
                parent.Items.Add(menu);

                GenMenu(parent, itemDto, list);
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
    }
}
