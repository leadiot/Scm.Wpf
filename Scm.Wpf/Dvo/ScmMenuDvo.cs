using Com.Scm.Enums;
using Com.Scm.Sys.Menu;
using Com.Scm.Wpf.Actions;
using System.Windows;

namespace Com.Scm.Wpf.Dvo.Menu
{
    public class ScmMenuDvo : ScmDvo
    {
        private ScmClientTypeEnum client;
        public ScmClientTypeEnum Client { get { return client; } set { SetProperty(ref client, value); } }

        private ScmMenuTypesEnum types;
        public ScmMenuTypesEnum Types { get { return types; } set { SetProperty(ref types, value); } }

        private string lang;
        public string Lang { get { return lang; } set { SetProperty(ref lang, value); } }

        private string codec;
        public string Codec { get { return codec; } set { SetProperty(ref codec, value); } }

        private string namec;
        public string Namec { get { return namec; } set { SetProperty(ref namec, value); } }

        private long pid;
        public long Pid { get { return pid; } set { SetProperty(ref pid, value); } }

        private string uri;
        public string Uri { get { return uri; } set { SetProperty(ref uri, value); } }

        private string view;
        public string View { get { return view; } set { SetProperty(ref view, value); } }

        private string icon;
        public string Icon { get { return icon; } set { SetProperty(ref icon, value); } }

        private string active;
        public string Active { get { return active; } set { SetProperty(ref active, value); } }

        private string color;
        public string Color { get { return color; } set { SetProperty(ref color, value); } }

        private int layer = 1;
        public int Layer { get { return layer; } set { SetProperty(ref layer, value); } }

        private int od = 1;
        public int Od { get { return od; } set { SetProperty(ref od, value); } }

        private bool visible;
        public bool Visible { get { return visible; } set { SetProperty(ref visible, value); } }

        private bool enabled;
        public bool Enabled { get { return enabled; } set { SetProperty(ref enabled, value); } }

        private bool fullpage;
        public bool Fullpage { get { return fullpage; } set { SetProperty(ref fullpage, value); } }

        private bool keepAlive;
        public bool KeepAlive { get { return keepAlive; } set { SetProperty(ref keepAlive, value); } }

        public List<ScmMenuDvo> children;

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

        public void Add(ScmMenuDvo dto)
        {
            if (children == null)
            {
                children = new List<ScmMenuDvo>();
            }

            children.Add(dto);
        }

        public static ScmMenuDvo FromDto(MenuDto dto)
        {
            var dvo = new ScmMenuDvo();
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
