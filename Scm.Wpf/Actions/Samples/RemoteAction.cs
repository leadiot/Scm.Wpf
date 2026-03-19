using Com.Scm.Wpf.Dvo.Menu;

namespace Com.Scm.Wpf.Actions.Samples
{
    public class RemoteAction : AAction
    {
        public override void Execute(ScmMenuDvo dvo)
        {
            if (Window == null)
            {
                return;
            }

            Window.ShowView(dvo.Codec, dvo.Namec, "Com.Scm.Wpf.Views.Samples.Remote.MainView");
        }
    }
}
