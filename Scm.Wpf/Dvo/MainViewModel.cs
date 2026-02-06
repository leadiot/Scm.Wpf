using Com.Scm.Sys.Menu;
using Com.Scm.Wpf.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Com.Scm.Wpf.Dvo
{
    public partial class MainViewModel : ScmDvo
    {
        [ObservableProperty]
        private ObservableCollection<MenuDto> menuList;

        public void Init()
        {
            var menuList = InitTestMenu();
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

        private List<MenuDto> InitTestMenu()
        {
            var list = new List<MenuDto>();
            var menu = new WpfMenuDto();
            menu.id = 1;
            menu.codec = "root";
            menu.namec = "Home";

            var item = new WpfMenuDto();
            item.id = 10;
            item.codec = "userInfo";
            item.namec = "用户信息";
            item.pid = menu.id;
            menu.Add(item);
            list.Add(menu);

            return list;
        }
    }
}
