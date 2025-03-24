using CommunityToolkit.Mvvm.ComponentModel;

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
    }

    public class ScmDataDvo : ScmDvo
    {
        public long update_time { get; set; }
        public long update_user { get; set; }

        public long create_time { get; set; }
        public long create_user { get; set; }
    }
}
