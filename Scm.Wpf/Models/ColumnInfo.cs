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
        public string Format { get; set; }

        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool Hidden { get; set; }
    }

    public enum ColumnType
    {
        Text,
        CheckBox,
        ComboBox,
        Template
    }

    public enum ColumnAlign
    {
        None,
        Left,
        Center,
        Right
    }
}
