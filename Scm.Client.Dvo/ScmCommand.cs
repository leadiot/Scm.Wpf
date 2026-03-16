using System.Windows.Input;

namespace Com.Scm.Wpf.Dvo
{
    public class ScmCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        private Action<object> _Excute { get; set; }

        private Predicate<object> _CanExcute { get; set; }

        public ScmCommand(Action<object> ExcuteMethod, Predicate<object> CanExcuteMethod = null)
        {
            _Excute = ExcuteMethod;
            _CanExcute = CanExcuteMethod;
        }

        public bool CanExecute(object parameter)
        {
            return _CanExcute != null && _CanExcute(parameter);
        }

        public void Execute(object parameter)
        {
            if (_Excute != null)
            {
                _Excute(parameter);
            }
        }
    }
}
