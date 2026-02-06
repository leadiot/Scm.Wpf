using Com.Scm.Enums;
using Com.Scm.Sys.Menu;
using Com.Scm.Wpf.Actions;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace Com.Scm.Wpf.Dvo.Menu
{
    public partial class MenuDvo : ScmDvo
    {
        [ObservableProperty]
        private ScmClientTypeEnum client;

        [ObservableProperty]
        private ScmMenuTypesEnum types;

        [ObservableProperty]
        private string lang;

        [ObservableProperty]
        private string codec;

        [ObservableProperty]
        private string namec;

        [ObservableProperty]
        private long pid;

        [ObservableProperty]
        private string uri;

        [ObservableProperty]
        private string view;

        [ObservableProperty]
        private string icon;

        [ObservableProperty]
        private string active;

        [ObservableProperty]
        private string color;

        [ObservableProperty]
        private int layer = 1;

        [ObservableProperty]
        private int od = 1;

        [ObservableProperty]
        private bool visible;

        [ObservableProperty]
        private bool enabled;

        [ObservableProperty]
        private bool fullpage;

        [ObservableProperty]
        private bool keepAlive;

        public List<MenuDvo> children;

        public AAction Action { get; set; }

        public FrameworkElement Element { get; set; }

        public void Add(MenuDvo dto)
        {
            if (children == null)
            {
                children = new List<MenuDvo>();
            }

            children.Add(dto);
        }

        public static MenuDvo FromDto(MenuDto dto)
        {
            var dvo = new MenuDvo();
            dvo.Id = dto.id;
            dvo.Client = dto.client;
            dvo.Types = dto.types;
            dvo.Codec = dto.codec;
            dvo.Namec = dto.namec;
            return dvo;
        }
    }
}
