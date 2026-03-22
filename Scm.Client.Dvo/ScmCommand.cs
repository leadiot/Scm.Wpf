using System.Windows.Input;

namespace Com.Scm.Dvo
{
    /// <summary>
    /// 一个基础的 ICommand 实现，允许通过委托来定义执行逻辑和条件判断。
    /// </summary>
    public class ScmCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        /// <summary>
        /// 创建一个不带参数的 RelayCommand。
        /// 注意：虽然内部存储为 Action<object?>，但使用时通常忽略参数。
        /// </summary>
        /// <param name="execute">执行逻辑</param>
        public ScmCommand(Action execute) : this(o => execute(), null)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));
        }

        /// <summary>
        /// 创建一个带 CanExecute 判断的 RelayCommand。
        /// </summary>
        /// <param name="execute">执行逻辑</param>
        /// <param name="canExecute">判断是否可以执行的逻辑</param>
        public ScmCommand(Action execute, Func<bool> canExecute)
            : this(o => execute(), canExecute != null ? o => canExecute() : null)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));
        }

        /// <summary>
        /// 通用构造函数，支持泛型参数对象。
        /// </summary>
        /// <param name="execute">执行逻辑，接收一个 object 参数</param>
        /// <param name="canExecute">判断逻辑，接收一个 object 参数</param>
        public ScmCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// 当 CanExecute 状态发生变化时触发（用于通知 UI 更新按钮的 Enabled 状态）。
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// 判断命令当前是否可以执行。
        /// </summary>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// 执行命令。
        /// </summary>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// 手动触发 CanExecuteChanged 事件。
        /// 当你需要动态改变按钮的可用状态（例如文本框输入变化后）时调用此方法。
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }

    /// <summary>
    /// 泛型版本，提供更强的类型安全性，避免在 ViewModel 中进行强制类型转换。
    /// 用法：new RelayCommand<string>(param => DoSomething(param))
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    public class ScmCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public ScmCommand(Action<T> execute) : this(execute, null) { }

        public ScmCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null) return true;

            // 处理参数类型不匹配的情况（例如 XAML 传了 null 但泛型是 int）
            if (parameter == null && default(T) != null)
            {
                // 如果 T 是值类型且参数为 null，通常视为无效，除非你有特殊逻辑
                // 这里简单返回 false 或根据需求调整
                return false;
            }

            return _canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}