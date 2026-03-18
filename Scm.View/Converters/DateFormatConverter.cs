using Com.Scm.Utils;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Com.Scm.Converter
{
    public class DateFormatConverter : IValueConverter
    {
        // 源数据（DateTime）→ 界面展示（string）
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 空值判断
            if (value == null || value == DependencyProperty.UnsetValue)
            {
                return string.Empty;
            }

            // 类型转换
            if (value is long date)
            {
                var time = TimeUtils.GetDateTimeFromUnixTimeStamp(date);
                return TimeUtils.FormatDataTime(date);
            }

            return value.ToString();
        }

        // 界面展示（string）→ 源数据（DateTime）（双向绑定时生效，如DataGrid可编辑列）
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 空值判断
            //if (string.IsNullOrEmpty(value?.ToString()))
            //{
            //    return DependencyProperty.UnsetValue;
            //}

            //// 反向转换：字符串转DateTime
            //if (DateTime.TryParse(value.ToString(), culture, DateTimeStyles.None, out var date))
            //{
            //    return date;
            //}

            //// 转换失败返回原值或抛出异常
            //return DependencyProperty.UnsetValue;
            return 0;
        }
    }
}
