using Com.Scm.Dvo.Menu;

namespace Com.Scm.Actions
{
    /// <summary>
    /// 视图事件
    /// </summary>
    public class ViewAction : AAction
    {
        public override void Execute(ScmMenuDvo dvo)
        {
            if (Window == null)
            {
                return;
            }

            var view = dvo.View;
            if (string.IsNullOrEmpty(view))
            {
                return;
            }

            Window.ShowView(dvo.Codec, dvo.Namec, view, dvo.Args, dvo.Module, dvo.KeepAlive);
        }
    }
}
