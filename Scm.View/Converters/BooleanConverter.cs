using System.Globalization;
using System.Windows.Data;

namespace Com.Scm.Converter
{
    public class BooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isTrue)
            {
                return isTrue ? "启用" : "禁用";
            }
            return "未知状态";
        }

        // 双向转换：文字转布尔值
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                return text.Equals("启用", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}