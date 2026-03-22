using Com.Scm.Dvo.Menu;

namespace Com.Scm.Actions.Samples
{
    public class NativeAction : AAction
    {
        public override void Execute(ScmMenuDvo dvo)
        {
            if (Window == null)
            {
                return;
            }

            Window.ShowView(dvo.Codec, dvo.Namec, "Com.Scm.Views.Samples.Native.MainView");
        }
    }
}
