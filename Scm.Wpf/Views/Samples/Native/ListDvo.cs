using Com.Scm.Wpf.Dvo;

namespace Com.Scm.Wpf.Views.Samples.Native
{
    public class ListDvo : ScmSearchResultDataDvo
    {
        private string _codec;
        public string codec { get { return _codec; } set { SetProperty(ref _codec, value); } }

        private string _namec;
        public string namec { get { return _namec; } set { SetProperty(ref _namec, value); } }

        private string _remark;
        public string remark { get { return _remark; } set { SetProperty(ref _remark, value); } }
    }
}
