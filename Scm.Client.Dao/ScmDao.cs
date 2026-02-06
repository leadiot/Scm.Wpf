using Com.Scm.Utils;
using SqlSugar;

namespace Com.Scm.Dao
{
    public class ScmDao
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long id { get; set; }

        public virtual void PrepareCreate()
        {
            if (!IsValidId())
            {
                id = NextId();
            }
        }

        public virtual void PrepareUpdate()
        {
        }

        public virtual void PrepareDelete()
        {
        }

        public bool IsValidId()
        {
            return id > 1000;
        }

        private static long _LastTime = 0;
        private static int _Index = 0;
        private static long NextId()
        {
            var now = TimeUtils.GetUnixTime(DateTime.UtcNow, false) * 1000;
            if (now > _LastTime)
            {
                _LastTime = now;
                _Index = 1;
            }
            else
            {
                _Index += 1;
            }

            return _LastTime + _Index;
        }
    }
}
