using Com.Scm.Enums;
using Com.Scm.Utils;
using SqlSugar;

namespace Com.Scm.Dao
{
    public class ScmDataDao : ScmDao
    {
        /// <summary>
        /// 数据状态
        /// </summary>
        [SugarColumn(ColumnDataType = "tinyint")]
        public ScmRowStatusEnum row_status { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public long update_time { get; set; }

        /// <summary>
        /// 记录时间
        /// </summary>
        public long create_time { get; set; }

        public override void PrepareCreate()
        {
            base.PrepareCreate();

            row_status = ScmRowStatusEnum.Enabled;

            update_time = TimeUtils.GetUnixTime();
            create_time = update_time;
        }

        public override void PrepareUpdate()
        {
            base.PrepareUpdate();

            update_time = TimeUtils.GetUnixTime();
        }
    }
}
