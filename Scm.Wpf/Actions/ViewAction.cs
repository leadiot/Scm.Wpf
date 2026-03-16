using Com.Scm.Utils;
using Com.Scm.Wpf.Dvo.Menu;

namespace Com.Scm.Wpf.Actions
{
    public class ViewAction : AAction
    {
        public override void Execute(ScmMenuDvo dvo)
        {
            LogUtils.Debug("ViewAction-Execute");

            if (Owner == null)
            {
                LogUtils.Info("ViewAction-Execute: Owner is null");
                return;
            }

            //if (paramList == null)
            //{
            //    LogUtils.Info("ViewAction-Execute: paramList is null");
            //    return;
            //}

            //var view = GetParam(paramList, "view");
            var view = dvo.View;
            if (string.IsNullOrEmpty(view))
            {
                LogUtils.Info("ViewAction-Execute: view param is null");
                return;
            }

            //var cache = GetParam(paramList, "cache");
            //var useCache = "true".Equals(cache, StringComparison.OrdinalIgnoreCase);
            var useCache = dvo.KeepAlive;

            Owner.ShowView(dvo.Codec, dvo.Namec, view, useCache);
        }
    }
}
