using Com.Scm.Wpf.Dvo;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Com.Scm.Wpf.Models
{
    public partial class ModuleGroup : ScmDvo
    {
        /// <summary>
        /// 组名称
        /// </summary>
        [ObservableProperty]
        private string groupName;
        /// <summary>
        /// 收缩面板-模板
        /// </summary>
        [ObservableProperty]
        private bool contractionTemplate = true;
        /// <summary>
        /// 包含的子模块
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Modules> modules;
        /// <summary>
        /// Icon
        /// </summary>
        [ObservableProperty]
        private string icon;
    }
}
