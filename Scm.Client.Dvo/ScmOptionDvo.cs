namespace Com.Scm.Dvo
{
    public partial class ScmOptionDvo<T> : ScmDvo
    {
        /// <summary>
        /// 选项标签
        /// </summary>
        private string label;
        public string Label { get { return label; } set { SetProperty(ref label, value); } }

        /// <summary>
        /// 选项的值
        /// </summary>
        private T value;
        public T Value { get { return value; } set { SetProperty(ref value, value); } }
        /// <summary>
        /// 是否禁用
        /// </summary>
        private bool disabled;
        public bool Disabled { get { return disabled; } set { SetProperty(ref disabled, value); } }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TextOptionDvo : ScmOptionDvo<string>
    {
    }

    /// <summary>
    /// 应用资源
    /// </summary>
    public class ResOptionDvo : ScmOptionDvo<long>
    {
        /// <summary>
        /// 类别
        /// </summary>
        private int cat;
        public int Cat { get { return cat; } set { SetProperty(ref cat, value); } }
        /// <summary>
        /// 上级ID
        /// </summary>
        private long parentId;
        public long ParentId { get { return parentId; } set { SetProperty(ref parentId, value); } }
    }

    /// <summary>
    /// 数据字典
    /// </summary>
    public partial class DicOptionDvo : ScmOptionDvo<int>
    {
        /// <summary>
        /// 类别
        /// </summary>
        private int cat;
        public int Cat { get { return cat; } set { SetProperty(ref cat, value); } }
        /// <summary>
        /// 键
        /// </summary>
        private string codec;
        public string Codec { get { return codec; } set { SetProperty(ref codec, value); } }
    }
}
