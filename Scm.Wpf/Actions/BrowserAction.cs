using Com.Scm.Helper;
using Com.Scm.Dvo.Menu;

namespace Com.Scm.Actions
{
    /// <summary>
    /// 浏览器事项
    /// </summary>
    public class BrowserAction : AAction
    {
        public override void Execute(ScmMenuDvo dvo)
        {
            if (Window == null)
            {
                return;
            }

            //var args = dvo.Args;
            //if (string.IsNullOrWhiteSpace(args))
            //{
            //    return;
            //}

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

            OsHelper.Browse(dvo.View);
        }
    }
}
