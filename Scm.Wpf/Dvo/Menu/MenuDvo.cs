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

        /// <summary>
        /// 是否已加载
        /// </summary>
        public bool Loaded { get; set; }

        /// <summary>
        /// 菜单事件
        /// </summary>
        public AAction Action { get; set; }

        /// <summary>
        /// 事件参数
        /// </summary>
        public string Args { get; set; }

        /// <summary>
        /// 界面组件
        /// </summary>
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
            dvo.Pid = dto.pid;
            dvo.Uri = dto.uri;
            dvo.View = dto.view;
            dvo.Icon = dto.icon;
            dvo.Color = dto.color;
            dvo.Visible = dto.visible;
            dvo.Enabled = dto.enabled;
            dvo.KeepAlive = dto.keepAlive;
            return dvo;
        }
    }
}
