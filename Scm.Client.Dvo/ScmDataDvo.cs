using Com.Scm.Enums;
using Com.Scm.Wpf.Dvo;

namespace Com.Scm
{
    public class ScmDataDvo : ScmDvo
    {
        private bool _checked;
        public bool Checked { get { return _checked; } set { SetProperty(ref _checked, value); } }

        private ScmRowStatusEnum _status;
        public ScmRowStatusEnum row_status { get { return _status; } set { SetProperty(ref _status, value); } }

        public long update_time { get; set; }
        public long update_user { get; set; }
        public string update_name { get; set; }

        public long create_time { get; set; }
        public long create_user { get; set; }
        public string create_name { get; set; }
    }
}
