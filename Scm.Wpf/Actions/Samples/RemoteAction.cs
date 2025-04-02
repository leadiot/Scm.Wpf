using Com.Scm.Sys.Menu;

namespace Com.Scm.Wpf.Actions.Samples
{
    public class RemoteAction : AAction
    {
        public override void Execute(MenuDto dto)
        {
            if (Owner == null)
            {
                return;
            }

            Owner.ShowView("Com.Scm.Wpf.Views.Samples.Remote.MainView");
        }
    }
}
