using Com.Scm.Dao;
using Com.Scm.Dao.Samples;
using Com.Scm.Dvo;
using Com.Scm.Models;
using SqlSugar;
using System.Threading.Tasks;

namespace Com.Scm.Views.Samples.Native
{
    public class MainViewDvo : ScmPageGridDvo
    {
        private ScmWindow _Window;
        private List<SearchResultItemDvo> Items;

        public void Init(ScmWindow window)
        {
            _Window = window;

            Columns = new List<ScmColumnInfo>
            {
                new ScmColumnInfo { Type=ScmColumnType.Text, Label = "ID", Value = "Id",Hidden=true },
                new ScmColumnInfo { Type=ScmColumnType.CheckBox, Label = "", Value = "IsChecked", Width="70" },
                new ScmColumnInfo { Type=ScmColumnType.Text, Label = "系统编码", Value = "Codec" },
                new ScmColumnInfo { Type=ScmColumnType.Text, Label = "系统名称", Value = "Namec", Width="*", MinWidth="100" }
            };
        }

        public override Task SearchAsync(int pageIndex = 0)
        {
            return Task.CompletedTask;
        }

        public override Task FirstPageAsync()
        {
            PageIndex = 1;

            return ReloadAsync();
        }

        public override Task PrevPageAsync()
        {
            PageIndex -= 1;
            if (PageIndex < 1)
            {
                PageIndex = 1;
            }

            return ReloadAsync();
        }

        public override Task NextPageAsync()
        {
            PageIndex += 1;
            if (PageIndex > PageCount)
            {
                PageIndex = PageCount;
            }

            return ReloadAsync();
        }

        public override Task EndPageAsync()
        {
            PageIndex = PageCount;

            return ReloadAsync();
        }

        public override async Task ReloadAsync()
        {
            try
            {
                var client = SqlHelper.GetSqlClient();

                var body = ToDictionary();
                var result = await client.Queryable<ScmDemoNativeDao>()
                    //.Where(a => a != null)
                    .Select<SearchResultItemDvo>()
                    .ToPageListAsync(0, 10, new RefAsync<int>(itemCount));

                Items.Clear();
                Items.AddRange(result);
            }
            catch (Exception exp)
            {
                _Window.ShowException(exp);
            }
        }
    }
}
