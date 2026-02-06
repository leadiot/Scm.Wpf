using Com.Scm.Wpf.Dvo;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Com.Scm.Wpf.Models
{
    public partial class Modules : ScmDvo
    {
        /// <summary>
        /// 模块图标代码
        /// </summary>
        [ObservableProperty]
        private string code;
        /// <summary>
        /// 模块名称
        /// </summary>
        private string name;
        /// <summary>
        /// 权限值
        /// </summary>
        private int auth;
        /// <summary>
        /// 模块命名空间
        /// </summary>
        private string typeName;
    }
    /// <summary>
    /// 模块UI组件
    /// </summary>
    public partial class ModuleUIComponent : Modules
    {
        /// <summary>
        /// 页面内容
        /// </summary>
        [ObservableProperty]
        private object body;
    }
}
