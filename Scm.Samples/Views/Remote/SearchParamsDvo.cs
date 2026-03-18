using Com.Scm.Enums;
using Com.Scm.Wpf.Models;
using System.Collections.ObjectModel;

namespace Com.Scm.Wpf.Views.Samples.Remote
{
    public class SearchParamsDvo : ScmSearchParamsDvo
    {
        private string key;
        public string Key { get { return key; } set { SetProperty(ref key, value); } }

        private ScmRowStatusEnum status;
        public ScmRowStatusEnum Status { get { return status; } set { SetProperty(ref status, value); } }

        private bool drawer;
        public bool Drawer { get { return drawer; } set { SetProperty(ref drawer, value); } }

        public ObservableCollection<ScmColumnInfo> Columns { get; set; }

        public SearchParamsDvo()
        {
            Columns = new ObservableCollection<ScmColumnInfo>
            {
                new ScmColumnInfo { Type=ScmColumnType.Text, Label = "ID", Value = "Id",Hidden=true },
                new ScmColumnInfo { Type=ScmColumnType.CheckBox, Label = "", Value = "IsChecked", Width="70" },
                new ScmColumnInfo { Type=ScmColumnType.Text, Label = "系统编码", Value = "Codec" },
                new ScmColumnInfo { Type=ScmColumnType.Text, Label = "系统名称", Value = "Namec", Width="*", MinWidth="100" }
            };
        }
    }
}
