using SqlSugar;

namespace Com.Scm.Dao
{
    [SugarTable("scm_ver")]
    public class ScmVerDao : ScmDao
    {
        /// <summary>
        /// 代码
        /// </summary>
        [SugarColumn(Length = 16)]
        public string key { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public int ver { get; set; }

        /// <summary>
        /// 发行日期
        /// </summary>
        public string date { get; set; }

        public long update_time { get; set; }

        public long create_time { get; set; }
    }
}
