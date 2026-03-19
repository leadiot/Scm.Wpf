using Com.Scm.Dvo.Menu;

namespace Com.Scm.Actions
{
    public abstract class AAction
    {
        public abstract void Execute(ScmMenuDvo dto);

        public ScmWindow Window { get; set; }

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
                return def;
            }

            return val.Trim();
        }
    }
}
