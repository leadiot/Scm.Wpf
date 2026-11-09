using Com.Scm.Dvo;
using Com.Scm.Models;

namespace Com.Scm.Models
{
    /// <summary>
    /// 列设置项
    /// </summary>
    public class PageGridColumnOption : ScmDvo
    {
        /// <summary>
        /// 列元数据
        /// </summary>
        public ScmColumnInfo Info { get; private set; }

        /// <summary>
        /// 列标题
        /// </summary>
        public string Label
        {
            get { return string.IsNullOrEmpty(Info.Label) ? Info.Value : Info.Label; }
        }

        private bool _visible;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (SetProperty(ref _visible, value))
                {
                    Info.Hidden = !value;
                }
            }
        }

        private bool _canMoveUp;
        /// <summary>
        /// 是否可上移
        /// </summary>
        public bool CanMoveUp { get { return _canMoveUp; } set { SetProperty(ref _canMoveUp, value); } }

        private bool _canMoveDown;
        /// <summary>
        /// 是否可下移
        /// </summary>
        public bool CanMoveDown { get { return _canMoveDown; } set { SetProperty(ref _canMoveDown, value); } }

        public PageGridColumnOption(ScmColumnInfo info)
        {
            Info = info;
            _visible = !info.Hidden;
        }
    }
}
