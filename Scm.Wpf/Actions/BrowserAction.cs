using Com.Scm.Utils;
using Com.Scm.Wpf.Dvo.Menu;

namespace Com.Scm.Wpf.Actions
{
    public class BrowserAction : AAction
    {
        public override void Execute(ScmMenuDvo dvo)
        {
            LogUtils.Debug("BrowserAction-Execute");

            if (Owner == null)
            {
                LogUtils.Info("BrowserAction-Execute: Owner is null");
                return;
            }

            //if (paramList == null)
            //{
            //    LogUtils.Info("BrowserAction-Execute: paramList is null");
            //    return;
            //}

            //var url = GetParam(paramList, "url");
            //if (string.IsNullOrEmpty(url))
            //{
            //    LogUtils.Info("BrowserAction-Execute: url param is null");
            //    return;
            //}

            //Owner.ShowView(url);
        }
    }
}
