using System.Text.RegularExpressions;
using System.Windows.Data;

namespace Com.Scm.Wpf.Models
{
    public class ScmColumnInfo
    {
        /// <summary>
        /// 列类型
        /// </summary>
        public ScmColumnType Type { get; set; }

        /// <summary>
        /// 列标题
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 列属性
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 列宽
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// 列宽（最小）
        /// </summary>
        public string MinWidth { get; set; }

        /// <summary>
        /// 列宽（最大）
        /// </summary>
        public string MaxWidth { get; set; }

        /// <summary>
        /// 对齐方式
        /// </summary>
        public ScmColumnAlign Align { get; set; }

        /// <summary>
        /// 格式化
        /// </summary>
        public ScmColumnFormat Format { get; set; }

        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// 是否导出
        /// </summary>
        public bool Export { get; set; } = true;

        /// <summary>
        /// 是否只读
        /// </summary>
        public bool ReadOnly { get; set; } = true;

        public BindingMode Mode { get; set; } = BindingMode.OneWay;

        public IValueConverter Converter { get; set; }

        public string Foreground { get; set; }

        public string Background { get; set; }
    }

    public class ScmColumnSize
    {
        /// <summary>
        /// 数值
        /// </summary>
        public double Width { get; set; } = 100;

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 未指定
        /// </summary>
        public bool IsNone { get; private set; }

        /// <summary>
        /// 填充
        /// </summary>
        public bool IsFill { get; private set; }

        /// <summary>
        /// 自动
        /// </summary>
        public bool IsAuto { get; private set; }

        /// <summary>
        /// 指定
        /// </summary>
        public bool IsFixed { get; private set; }

        public static ScmColumnSize Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new ScmColumnSize { IsNone = true };
            }

            value = value.Trim();
            if (value == "*")
            {
                return new ScmColumnSize { IsFill = true };
            }

            var match = Regex.Match(value, @"(\d+)([^\d]*)");
            if (!match.Success)
            {
                return new ScmColumnSize { IsNone = true };
            }

            var width = match.Groups[1].Value;
            var uom = match.Groups[2].Value.Trim();

            return new ScmColumnSize { Width = double.Parse(width), Unit = uom, IsFixed = true };
        }
    }

    public enum ScmColumnType
    {
        Text,
        CheckBox,
        ComboBox,
        Template,
        Status
    }

    public enum ScmColumnFormat
    {
        None,
        Icon,
        Number,
        DateTime
    }

    public enum ScmColumnAlign
    {
        None,
        Left,
        Center,
        Right,
        Stretch
    }
}
