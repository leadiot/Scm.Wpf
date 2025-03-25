using Com.Scm.Enums;
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
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
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
        private ScmStatusEnum _status;
        public ScmStatusEnum row_status { get { return _status; } set { SetProperty(ref _status, value); } }

        public long update_time { get; set; }
        public long update_user { get; set; }
        public string update_name { get; set; }

        public long create_time { get; set; }
        public long create_user { get; set; }
        public string create_name { get; set; }
    }
}
