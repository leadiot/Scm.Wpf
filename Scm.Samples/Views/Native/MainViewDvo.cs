using Com.Scm.Wpf.Dvo;
using Com.Scm.Wpf.Models;
using System.Collections.ObjectModel;

namespace Com.Scm.Wpf.Views.Samples.Native
{
    public class MainViewDvo : ScmDvo
    {
        public ObservableCollection<ScmColumnInfo> Columns { get; set; }

        public MainViewDvo()
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
