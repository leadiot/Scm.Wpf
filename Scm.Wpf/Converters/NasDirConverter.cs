using Com.Scm.Nas;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Com.Scm.Converter
{
    public class NasDirConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 空值判断
            if (value == null || value == DependencyProperty.UnsetValue)
            {
                return string.Empty;
            }

            // 类型转换
            if (value is NasDirEnums date)
            {
                if (date == NasDirEnums.Upload)
                {
                    return "ArrowUpBoldBox";
                }
                if (date == NasDirEnums.Download)
                {
                    return "ArrowDownBoldBox";
                }
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return NasDirEnums.None;
        }
    }
}
