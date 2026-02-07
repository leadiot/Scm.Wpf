using System.Windows;
using System.Windows.Controls;

namespace Com.Scm.Wpf.Helper
{
    /// <summary>
    /// PasswordBox绑定的附加属性类
    /// </summary>
    public static class PasswordBoxHelper
    {
        // 定义附加属性：BindablePassword
        public static readonly DependencyProperty BindablePasswordProperty =
            DependencyProperty.RegisterAttached(
                "BindablePassword",
                typeof(string),
                typeof(PasswordBoxHelper),
                new FrameworkPropertyMetadata(string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnBindablePasswordChanged,
                    CoerceBindablePassword));

        // 获取附加属性值
        public static string GetBindablePassword(DependencyObject obj)
        {
            return (string)obj.GetValue(BindablePasswordProperty);
        }

        // 设置附加属性值
        public static void SetBindablePassword(DependencyObject obj, string value)
        {
            obj.SetValue(BindablePasswordProperty, value);
        }

        // 附加属性值变更时的处理（从ViewModel到PasswordBox）
        private static void OnBindablePasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                // 移除原有事件监听，避免循环触发
                passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;

                // 只在值不同时更新Password（避免覆盖用户输入）
                if (passwordBox.Password != e.NewValue.ToString())
                {
                    passwordBox.Password = e.NewValue.ToString();
                }

                // 重新添加事件监听（从PasswordBox到ViewModel）
                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
            }
        }

        // PasswordBox密码变更时更新附加属性（从PasswordBox到ViewModel）
        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                // 禁用属性变更回调，避免循环
                SetCurrentValue(passwordBox, BindablePasswordProperty, passwordBox.Password);
            }
        }

        // 强制转换值（确保类型正确）
        private static object CoerceBindablePassword(DependencyObject d, object value)
        {
            if (d is PasswordBox passwordBox && passwordBox.Password != value.ToString())
            {
                return value.ToString();
            }
            return value;
        }

        // 辅助方法：设置值但不触发PropertyChanged（避免循环）
        private static void SetCurrentValue(DependencyObject d, DependencyProperty property, object value)
        {
            d.SetValue(property, value);
        }
    }
}
