using Com.Scm.Enums;
using System.Globalization;
using System.Windows.Data;

namespace Com.Scm.Converter
{
    public class NasTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 类型转换
            if (value is ScmFileTypeEnum date)
            {
                if (date == ScmFileTypeEnum.Dir)
                {
                    return "Folder";
                }
                if (date == ScmFileTypeEnum.Doc)
                {
                    return "File";
                }
            }

            return "FileHidden";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ScmFileTypeEnum.None;
        }
    }
}
