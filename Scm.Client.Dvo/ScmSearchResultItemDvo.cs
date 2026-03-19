using Com.Scm.Enums;
using Com.Scm.Utils;

namespace Com.Scm.Dvo
{
    /// <summary>
    /// 查询结果明细
    /// </summary>
    public class ScmSearchResultItemDvo : ScmDvo
    {
        private bool _checked;
        public bool IsChecked { get { return _checked; } set { SetProperty(ref _checked, value); } }

        private ScmRowStatusEnum _status;
        public ScmRowStatusEnum row_status { get { return _status; } set { SetProperty(ref _status, value); } }

        private long _update_time;
        public long update_time { get { return _update_time; } set { SetProperty(ref _update_time, value); } }
        public long update_user { get; set; }
        public string update_name { get; set; }

        private long _create_time;
        public long create_time { get { return _create_time; } set { _create_time = value; } }
        public long create_user { get; set; }
        public string create_name { get; set; }

        public bool IsEnabled
        {
            get
            {
                return _status == ScmRowStatusEnum.Enabled;
            }
            set
            {
                SetProperty(ref _status, value ? ScmRowStatusEnum.Enabled : ScmRowStatusEnum.Disabled);
            }
        }

        public DateTime UpdateTime
        {
            get
            {
                return TimeUtils.GetDateTimeFromUnixTimeStamp(_update_time);
            }
            set
            {
                SetProperty(ref _update_time, TimeUtils.GetUnixTime(value));
            }
        }

        public DateTime CreateTime
        {
            get
            {
                return TimeUtils.GetDateTimeFromUnixTimeStamp(_create_time);
            }
            set
            {
                SetProperty(ref _create_time, TimeUtils.GetUnixTime(value));
            }
        }
    }
}
