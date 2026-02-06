using Com.Scm.Wpf.Dvo.Menu;

namespace Com.Scm.Wpf.Actions.Samples
{
    public class NativeAction : AAction
    {
        public override void Execute(MenuDvo dvo)
        {
            if (Owner == null)
            {
                return;
            }

            Owner.ShowView("", "", "Com.Scm.Wpf.Views.Samples.Native.MainView");
        }
    }
}
