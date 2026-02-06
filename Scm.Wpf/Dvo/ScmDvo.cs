using Com.Scm.Enums;
using Com.Scm.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection;

namespace Com.Scm.Wpf.Dvo
{
    //public class ScmDvo : INotifyPropertyChanged
    //{
    //    public event PropertyChangedEventHandler PropertyChanged;

    //    protected virtual void OnPropertyChanged(string propertyName)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }

    //    protected virtual void SetProperty(ref object property, object value)
    //    {
    //        if (property != value)
    //        {
    //            property = value;
    //            OnPropertyChanged(property.GetType().Name);
    //        }
    //    }
    //}

    public class ScmDvo : ObservableValidator
    {
        public long Id { get; set; }

        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public virtual bool IsValid()
        {
            return true;
        }

        //protected virtual void SetProperty(ref object property, object value)
        //{
        //    if (property != value)
        //    {
        //        property = value;
        //        OnPropertyChanged(property.GetType().Name);
        //    }
        //}

        public virtual Dictionary<string, string> ToDictionary()
        {
            var dict = new Dictionary<string, string>();

            Type type = GetType();
            var properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo property in properties)
            {
                var obj = property.GetValue(this);
                if (obj == null)
                {
                    continue;
                }
                dict[property.Name] = obj.ToString();
            }

            return dict;
        }
    }

    public class ScmDataDvo : ScmDvo
    {
        private ScmRowStatusEnum _status;
        public ScmRowStatusEnum row_status { get { return _status; } set { SetProperty(ref _status, value); } }

        public long update_time { get; set; }
        public long update_user { get; set; }
        public string update_name { get; set; }

        public long create_time { get; set; }
        public long create_user { get; set; }
        public string create_name { get; set; }
    }

    public class ScmSearchPageDvo : ScmDvo
    {
        private int _Page;
        /// <summary>
        /// 页索引
        /// </summary>
        public int Page { get { return _Page; } set { SetProperty(ref _Page, value); } }

        private int _Limit;
        /// <summary>
        /// 页面大小
        /// </summary>
        public int Limit { get { return _Limit; } set { SetProperty(ref _Limit, value); } }

        public void FirstPage()
        {
            Page = 1;
        }

        public virtual string GetPageUrl()
        {
            return "";
        }
    }

    public class ScmSearchResultDvo<T> : ScmDvo
    {
        private T _Items;
        public T Items { get { return _Items; } set { SetProperty(ref _Items, value); } }

        private int _Count;
        public int Count { get { return _Count; } set { SetProperty(ref _Count, value); } }

        private int _Total;
        public int Total { get { return _Total; } set { SetProperty(ref _Total, value); } }
    }

    public class ScmSearchResultDataDvo : ScmDvo
    {
        private bool _checked;
        public bool Checked { get { return _checked; } set { SetProperty(ref _checked, value); } }

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
