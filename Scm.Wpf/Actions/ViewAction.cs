using Com.Scm.Sys.Menu;
using Com.Scm.Utils;

namespace Com.Scm.Wpf.Actions
{
    public class ViewAction : AAction
    {
        public override void Execute(MenuDto dto)
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
            var view = dto.view;
            if (string.IsNullOrEmpty(view))
            {
                LogUtils.Info("ViewAction-Execute: view param is null");
                return;
            }

            //var cache = GetParam(paramList, "cache");
            //var useCache = "true".Equals(cache, StringComparison.OrdinalIgnoreCase);
            var useCache = dto.keepAlive;

            Owner.ShowView(view, useCache);
        }
    }
}
