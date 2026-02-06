using Com.Scm.Sys.Menu;
using Com.Scm.Wpf.Dvo.Menu;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Com.Scm.Wpf.Dvo
{
    public partial class MainViewModel : ScmDvo
    {
        [ObservableProperty]
        private ObservableCollection<MenuDvo> menuList = new ObservableCollection<MenuDvo>();

        public void Init(List<MenuDto> menuList)
        {
            InitTestMenu(menuList);

            foreach (var itemDto in menuList)
            {
                var itemDvo = MenuDvo.FromDto(itemDto);
                this.MenuList.Add(itemDvo);
            }
        }

        private void NavChanged(object o)
        {
            string typeName;
            string tabName;
            if (o.ToString() == "home")
            {
                typeName = "Com.Scm.Wpf.Views.Home.HomeView";
                tabName = "首页";
            }
            else
            {
                var values = (object[])o;
                typeName = values[0].ToString();
                tabName = values[1].ToString();
            }

            Type type = Type.GetType(typeName);
            if (type == null)
            {
                return;
            }
        }

        private void InitTestMenu(List<MenuDto> menuList)
        {
            var menu = new MenuDto();
            menu.id = 1;
            menu.codec = "root";
            menu.namec = "Home";
            menuList.Add(menu);

            var item = new MenuDto();
            item.id = 10;
            item.codec = "userInfo";
            item.namec = "用户信息";
            item.pid = menu.id;
            menuList.Add(item);
        }
    }
}
