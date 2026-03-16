using Com.Scm.Enums;
using Com.Scm.Wpf.Dvo;

namespace Com.Scm.Wpf.Views.Samples.Native
{
    public class SearchParamsDvo : ScmSearchParamsDvo
    {
        private string _key;
        public string Key { get { return _key; } set { SetProperty(ref _key, value); } }

        private ScmRowStatusEnum _status;
        public ScmRowStatusEnum Status { get { return _status; } set { SetProperty(ref _status, value); } }
    }
}
