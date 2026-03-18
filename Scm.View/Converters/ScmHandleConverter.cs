using Com.Scm.Enums;
using Com.Scm.Utils;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Com.Scm.Converter
{
    public class ScmHandleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 空值判断
            if (value == null || value == DependencyProperty.UnsetValue)
            {
                return string.Empty;
            }

            // 类型转换
            if (value is ScmHandleEnum date)
            {
                return date.ToDescription();
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ScmHandleEnum.None;
        }
    }
}