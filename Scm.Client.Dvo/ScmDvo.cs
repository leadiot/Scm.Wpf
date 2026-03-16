using Com.Scm.Attributes;
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

        protected void OnPropertyChanging([CallerMemberName] string propertyName = null)
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
        public void ClearError(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                _errors[propertyName].Clear();
                _errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
                OnPropertyChanged($"{propertyName}Error");
            }
        }

        public void ClearErrors()
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
        public bool ValidateProperty<T>(string propertyName, T value, Func<T, List<string>> validateFunc)
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

        public virtual void ValidateAllProperties()
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

        /// <summary>
        /// 浅复制到指定对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <returns></returns>
        public static T FromDto<T>(object src) where T : class, new()
        {
            var dst = default(T);
            if (dst == null)
            {
                return dst;
            }

            var srcType = src.GetType();
            var srcName = srcType.FullName;
            if (srcName == "System.String"
                || srcName == "System.Int32"
                || srcName == "System.Int64"
                || srcName == "System.Double"
                || srcName == "System.Single"
                || srcName == "System.Char"
                || srcName == "System.DateTime")
            {
                return dst;
            }

            var dstType = dst.GetType();
            var dstProps = dstType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
            if (dstProps == null)
            {
                return dst;
            }

            foreach (var dstProp in dstProps)
            {
                var srcPropName = dstProp.Name;
                object srcPropValue = null;
                var attr = dstProp.GetCustomAttribute<ScmMappingAttribute>();
                if (attr != null)
                {
                    srcPropName = attr.Name ?? dstProp.Name;
                    srcPropValue = attr.Value;
                }

                var srcProp = srcType.GetProperty(srcPropName);
                if (srcProp == null)
                {
                    continue;
                }

                dstProp.SetValue(dst, srcProp.GetValue(src) ?? srcPropValue);
            }

            return dst;
        }

        public T ToDto<T>()
        {
            var dst = default(T);
            return ToDto(this, dst);
        }

        /// <summary>
        /// 浅复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        public static T ToDto<T>(object src, T dst)
        {
            if (dst == null)
            {
                return dst;
            }

            var srcType = src.GetType();
            var srcName = srcType.FullName;
            if (srcName == "System.String"
                || srcName == "System.Int32"
                || srcName == "System.Int64"
                || srcName == "System.Double"
                || srcName == "System.Single"
                || srcName == "System.Char"
                || srcName == "System.DateTime")
            {
                return dst;
            }

            var dstType = dst.GetType();
            var dstProps = dstType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
            if (dstProps == null)
            {
                return dst;
            }

            foreach (var dstProp in dstProps)
            {
                var srcPropName = dstProp.Name;
                object srcPropValue = null;
                var attr = dstProp.GetCustomAttribute<ScmMappingAttribute>();
                if (attr != null)
                {
                    srcPropName = attr.Name ?? dstProp.Name;
                    srcPropValue = attr.Value;
                }

                var srcProp = srcType.GetProperty(srcPropName);
                if (srcProp == null)
                {
                    continue;
                }

                dstProp.SetValue(dst, srcProp.GetValue(src) ?? srcPropValue);
            }

            return dst;
        }
    }

    #region MVVM
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
    #endregion
}
