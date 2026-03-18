using Com.Scm.Enums;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Com.Scm.Converter
{
    public class ScmResultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 空值判断
            if (value == null || value == DependencyProperty.UnsetValue)
            {
                return string.Empty;
            }

            // 类型转换
            if (value is ScmResultEnum date)
            {
                switch (date)
                {
                    case ScmResultEnum.Failure:
                        return "失败";
                    case ScmResultEnum.Success:
                        return "成功";
                    default:
                        return "-";
                }
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ScmResultEnum.None;
        }
    }
}
