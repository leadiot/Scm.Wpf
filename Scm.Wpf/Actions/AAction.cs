using Com.Scm.Wpf.Dvo.Menu;

namespace Com.Scm.Wpf.Actions
{
    public abstract class AAction
    {
        public abstract void Execute(MenuDvo dto);

        public ScmWindow Owner { get; set; }

        protected string GetParam(Dictionary<string, string> paramList, string key, string def = null)
        {
            if (paramList == null)
            {
                return def;
            }

            if (string.IsNullOrEmpty(key))
            {
                return def;
            }

            if (!paramList.ContainsKey(key))
            {
                return def;
            }

            var val = paramList[key];
            if (val == null)
            {
                return val;
            }

            val = val.Trim();
            return val;
        }
    }
}
