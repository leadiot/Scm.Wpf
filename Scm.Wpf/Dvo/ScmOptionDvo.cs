using CommunityToolkit.Mvvm.ComponentModel;

namespace Com.Scm.Wpf.Dvo
{
    public partial class ScmOptionDvo<T> : ScmDvo
    {
        /// <summary>
        /// 选项标签
        /// </summary>
        [ObservableProperty]
        private string label;
        /// <summary>
        /// 选项的值
        /// </summary>
        [ObservableProperty]
        private T value;
        /// <summary>
        /// 是否禁用
        /// </summary>
        [ObservableProperty]
        private bool disabled;
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class TextOptionDvo : ScmOptionDvo<string>
    {
    }

    /// <summary>
    /// 应用资源
    /// </summary>
    public partial class ResOptionDvo : ScmOptionDvo<long>
    {
        /// <summary>
        /// 类别
        /// </summary>
        [ObservableProperty]
        private int cat;
        /// <summary>
        /// 上级ID
        /// </summary>
        [ObservableProperty]
        private long parentId;
    }

    /// <summary>
    /// 数据字典
    /// </summary>
    public partial class DicOptionDvo : ScmOptionDvo<int>
    {
        /// <summary>
        /// 类别
        /// </summary>
        [ObservableProperty]
        private int cat;
        /// <summary>
        /// 键
        /// </summary>
        [ObservableProperty]
        private string codec;
    }
}
