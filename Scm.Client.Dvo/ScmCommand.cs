using System.Windows.Input;

namespace Com.Scm.Dvo
{
    /// <summary>
    /// 通用的RelayCommand实现（支持带参数/无参数，支持CanExecute判断）
    /// </summary>
    public class ScmCommand : ICommand
    {
        // 命令执行的逻辑（无参数）
        private readonly Action _Execute;
        // 命令执行的逻辑（带参数）
        private readonly Action<object> _ExecuteWithParam;
        // 判断命令是否可执行的逻辑
        private readonly Func<object, bool> _CanExecute;

        /// <summary>
        /// 构造函数（无参数执行逻辑）
        /// </summary>
        /// <param name="execute">无参数的执行逻辑</param>
        /// <param name="canExecute">可选：判断是否可执行的逻辑</param>
        public ScmCommand(Action execute, Func<bool> canExecute = null)
        {
            _Execute = execute ?? throw new ArgumentNullException(nameof(execute));
            if (canExecute != null)
            {
                _CanExecute = param => canExecute();
            }
        }

        /// <summary>
        /// 构造函数（带参数执行逻辑）
        /// </summary>
        /// <param name="execute">带参数的执行逻辑</param>
        /// <param name="canExecute">可选：判断是否可执行的逻辑（支持参数）</param>
        public ScmCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _ExecuteWithParam = execute ?? throw new ArgumentNullException(nameof(execute));
            _CanExecute = canExecute;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                // 根据构造函数类型，执行对应逻辑
                _Execute?.Invoke();
                _ExecuteWithParam?.Invoke(parameter);
            }
        }

        /// <summary>
        /// 判断命令是否可执行
        /// </summary>
        public bool CanExecute(object parameter)
        {
            // 无判断逻辑时，默认可执行
            return _CanExecute?.Invoke(parameter) ?? true;
        }

        /// <summary>
        /// 通知UI：命令可执行状态变更（需手动调用）
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// 手动触发CanExecuteChanged，更新UI状态（如按钮禁用/启用）
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
