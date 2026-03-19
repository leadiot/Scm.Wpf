using Com.Scm.Enums;
using Com.Scm.Wpf.Models;

namespace Com.Scm.Wpf.Views.Samples.Remote
{
    public class SearchParamsDvo : ScmPageGridDvo
    {
        private ScmRowStatusEnum status;
        public ScmRowStatusEnum Status { get { return status; } set { SetProperty(ref status, value); } }

        private bool drawer;
        public bool Drawer { get { return drawer; } set { SetProperty(ref drawer, value); } }

        public void Init()
        {
            Columns = new List<ScmColumnInfo>
            {
                new ScmColumnInfo { Type=ScmColumnType.Text, Label = "ID", Value = "Id",Hidden=true },
                new ScmColumnInfo { Type=ScmColumnType.CheckBox, Label = "", Value = "IsChecked", Width="70" },
                new ScmColumnInfo { Type=ScmColumnType.Text, Label = "系统编码", Value = "Codec" },
                new ScmColumnInfo { Type=ScmColumnType.Text, Label = "系统名称", Value = "Namec", Width="*", MinWidth="100" }
            };
        }

        public override void SearchAsync(int pageIndex = 0)
        {
        }

        public override void FirstPageAsync()
        {
            PageIndex += 1;

            ReloadAsync();
        }

        public override void PrevPageAsync()
        {
            PageIndex -= 1;
            if (PageIndex < 1)
            {
                PageIndex = 1;
            }

            ReloadAsync();
        }

        public override void NextPageAsync()
        {
            PageIndex += 1;
            if (PageIndex > PageCount)
            {
                PageIndex = PageCount;
            }

            ReloadAsync();
        }

        public void EndPageAsync()
        {
            PageIndex = PageCount;

            ReloadAsync();
        }

        public async void ReloadAsync()
        {
            var body = ToDictionary();
            //_Response = await _Owner.GetObjectAsync<ScmSearchPageResponse<SearchResultDataDvo>>("/urposition/pages", body);
            //if (!_Response.Success)
            //{
            //    return;
            //}

            //PgData.ShowData(_Response);
        }
    }
}
