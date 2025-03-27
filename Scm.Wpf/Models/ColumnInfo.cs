using System.Text.RegularExpressions;

namespace Com.Scm.Wpf.Models
{
    public class ColumnInfo
    {
        /// <summary>
        /// 列类型
        /// </summary>
        public ColumnType Type { get; set; }

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
        public ColumnAlign Align { get; set; }

        /// <summary>
        /// 格式化
        /// </summary>
        public ColumnFormat Format { get; set; }

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
    }
    public class SizeUom
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

        public static SizeUom Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new SizeUom { IsNone = true };
            }

            value = value.Trim();
            if (value == "*")
            {
                return new SizeUom { IsFill = true };
            }

            var match = Regex.Match(value, @"(\d+)([^\d]*)");
            if (!match.Success)
            {
                return new SizeUom { IsNone = true };
            }

            var width = match.Groups[1].Value;
            var uom = match.Groups[2].Value.Trim();

            return new SizeUom { Width = double.Parse(width), Unit = uom, IsFixed = true };
        }
    }

    public enum ColumnType
    {
        Text,
        CheckBox,
        ComboBox,
        Template,
        Status
    }

    public enum ColumnFormat
    {
        None,
        Number,
        DateTime
    }

    public enum ColumnAlign
    {
        None,
        Left,
        Center,
        Right,
        Stretch
    }
}
