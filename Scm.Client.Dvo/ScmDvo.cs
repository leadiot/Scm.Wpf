using Com.Scm.Enums;
using Com.Scm.Utils;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Com.Scm.Wpf.Dvo
{
    public class ScmDvo : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        /// <summary>
        /// 对象ID
        /// </summary>
        public long Id { get; set; }

        private int _ErrorCode;
        /// <summary>
        /// 错误代码
        /// </summary>
        public int ErrorCode { get { return _ErrorCode; } set { SetProperty(ref _ErrorCode, value); } }

        private string _ErrorMessage;
        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get { return _ErrorMessage; } set { SetProperty(ref _ErrorMessage, value); } }

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

        public virtual bool IsValid()
        {
            return true;
        }

        #region 数据绑定
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanging([CallerMemberName] string? propertyName = null)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>([NotNullIfNotNull("newValue")] ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

            OnPropertyChanging(propertyName);
            field = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion

        #region 错误校验
        // 存储每个属性的错误信息
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        // INotifyDataErrorInfo接口实现
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// 是否有任何错误
        /// </summary>
        public bool HasErrors => _errors.Any(kv => kv.Value.Any());

        // 触发错误变更事件
        protected void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        // 添加单个属性的错误信息
        protected void AddError(string propertyName, string errorMessage)
        {
            if (!_errors.ContainsKey(propertyName))
            {
                _errors[propertyName] = new List<string>();
            }

            if (!_errors[propertyName].Contains(errorMessage))
            {
                _errors[propertyName].Add(errorMessage);
                OnErrorsChanged(propertyName);
                OnPropertyChanged($"{propertyName}Error");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public string GetFirstError(string propertyName)
        {
            if (_errors.TryGetValue(propertyName, out var errors) && errors.Any())
                return errors[0];
            return string.Empty;
        }

        // 获取指定属性的错误信息
        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName))
            {
                return Enumerable.Empty<string>();
            }
            return _errors[propertyName];
        }

        // 清除单个属性的错误信息
        protected void ClearError(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                _errors[propertyName].Clear();
                _errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
                OnPropertyChanged($"{propertyName}Error");
            }
        }

        protected void ClearErrors()
        {
            var keys = _errors.Keys.ToArray();
            _errors.Clear();
            foreach (var key in keys)
            {
                OnErrorsChanged(key);
                OnPropertyChanged($"{key}Error");
            }
        }

        /// <summary>
        /// 校验属性并更新错误信息（核心方法）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="validateFunc"></param>
        /// <returns></returns>
        protected bool ValidateProperty<T>(string propertyName, T value, Func<T, List<string>> validateFunc)
        {
            // 先清除旧错误
            ClearError(propertyName);

            // 执行自定义校验规则
            var errors = validateFunc(value);
            if (errors.Any())
            {
                // 添加新错误
                foreach (var error in errors)
                {
                    AddError(propertyName, error);
                }
                return false; // 校验不通过
            }
            return true; // 校验通过
        }

        protected virtual void ValidateAllProperties()
        {
            ClearErrors();

            var properties = GetType().GetProperties();
            if (properties == null || properties.Length < 1)
            {
                return;
            }

            foreach (var prop in properties)
            {
                var attrs = prop.GetCustomAttributes();
                if (attrs == null || attrs.Count() < 1)
                {
                    continue;
                }

                foreach (var attr in attrs)
                {
                    var v = attr as ValidationAttribute;
                    if (v == null)
                    {
                        continue;
                    }

                    if (!v.IsValid(prop.GetValue(this)))
                    {
                        AddError(prop.Name, v.ErrorMessage);
                    }
                }
            }
        }
        #endregion
    }

    //public partial class ScmDvo : ObservableValidator
    //{
    //    [ObservableProperty]
    //    private long id;

    //    [ObservableProperty]
    //    private int errorCode;
    //    [ObservableProperty]
    //    private string errorMessage;

    //    [ObservableProperty]
    //    private ScmLoginModeEnum mode;

    //    public virtual Dictionary<string, string> ToDictionary()
    //    {
    //        var dict = new Dictionary<string, string>();

    //        Type type = GetType();
    //        var properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
    //        foreach (PropertyInfo property in properties)
    //        {
    //            var obj = property.GetValue(this);
    //            if (obj == null)
    //            {
    //                continue;
    //            }
    //            dict[property.Name] = obj.ToString();
    //        }

    //        return dict;
    //    }

    //    public virtual bool IsValid()
    //    {
    //        return true;
    //    }
    //}

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
