using Com.Scm.Sys.Menu;

namespace Com.Scm.Wpf.Actions.Samples
{
    public class NativeAction : AAction
    {
        public override void Execute(MenuDto dto)
        {
            if (Owner == null)
            {
                return;
            }

            Owner.ShowView("Com.Scm.Wpf.Views.Samples.Native.MainView");
        }
    }
}
