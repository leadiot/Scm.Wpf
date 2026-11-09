using Com.Scm.Dao;
using Com.Scm.Dao.Samples;
using Com.Scm.Dvo;
using Com.Scm.Models;
using SqlSugar;

namespace Com.Scm.Views.Samples.Native
{
    public class MainViewDvo : ScmPageGridDvo<SearchResultItemDvo>
    {
        private ScmWindow _Window;

        public void Init(ScmWindow window)
        {
            _Window = window;

            Columns = new List<ScmColumnInfo>
            {
                new ScmColumnInfo { Type=ScmColumnType.Text, Label = "ID", Value = "Id",Hidden=true },
                new ScmColumnInfo { Type=ScmColumnType.CheckBox, Label = "", Value = "IsChecked", Width="70" },
                new ScmColumnInfo { Type=ScmColumnType.Text, Label = "系统编码", Value = "codec" },
                new ScmColumnInfo { Type=ScmColumnType.Text, Label = "系统名称", Value = "namec", Width="*", MinWidth="100" }
            };
        }

        protected override async Task<ScmPageResult<SearchResultItemDvo>> LoadPageAsync()
        {
            var client = SqlHelper.GetSqlClient();

            var total = new RefAsync<int>();
            var list = await client.Queryable<ScmDemoNativeDao>()
                .Select<SearchResultItemDvo>()
                .ToPageListAsync(PageIndex, PageItems, total);

            return new ScmPageResult<SearchResultItemDvo> { Items = list, Total = total.Value };
        }

        protected override void OnError(Exception ex)
        {
            _Window?.ShowException(ex);
        }
    }
}
