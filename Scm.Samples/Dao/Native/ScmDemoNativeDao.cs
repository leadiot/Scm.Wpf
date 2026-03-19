using Com.Scm.Dao;
using Com.Scm.Enums;
using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace Com.Scm.Dao.Samples
{
    [SugarTable("scm_demo_native")]
    public class ScmDemoNativeDao : ScmDao
    {
        [StringLength(32)]
        public string codec { get; set; }

        [StringLength(32)]
        [SugarColumn(Length = 32)]
        public string namec { get; set; }

        [StringLength(256)]
        [SugarColumn(Length = 256, IsNullable = true)]
        public string remark { get; set; }

        [SugarColumn(ColumnDataType = "tinyint")]
        public ScmRowStatusEnum row_status { get; set; }
    }
}
