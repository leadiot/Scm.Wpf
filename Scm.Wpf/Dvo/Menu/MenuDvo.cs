using Com.Scm.Enums;

namespace Com.Scm.Wpf.Dvo.Menu
{
    public class MenuDvo : ScmDvo
    {
        public ScmClientTypeEnum client { get; set; }

        public MenuTypesEnum types { get; set; }

        public string lang { get; set; }

        public string codec { get; set; }

        public string namec { get; set; }

        public long pid { get; set; }

        public string uri { get; set; }

        public string view { get; set; }

        public string icon { get; set; }

        public string active { get; set; }

        public string color { get; set; }

        public int layer { get; set; } = 1;

        public int od { get; set; } = 1;

        public bool visible { get; set; }

        public bool enabled { get; set; }

        public bool fullpage { get; set; }

        public bool keepAlive { get; set; }

        public List<MenuDvo> children { get; set; }

        public void Add(MenuDvo dto)
        {
            if (children == null)
            {
                children = new List<MenuDvo>();
            }

            children.Add(dto);
        }
    }
}
