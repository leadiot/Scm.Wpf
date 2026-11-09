using Com.Scm.Dvo;
using Com.Scm.Enums;
using Com.Scm.Models;

namespace Com.Scm.Views.Samples.Remote
{
    public class SearchParamsDvo : ScmPageGridDvo<SearchResultDataDvo>
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

        protected override Task<ScmPageResult<SearchResultDataDvo>> LoadPageAsync()
        {
            //var body = ToDictionary();
            //var response = await _Owner.GetObjectAsync<ScmSearchPageResponse<SearchResultDataDvo>>("/urposition/pages", body);
            //if (!response.Success)
            //{
            //    return new ScmPageResult<SearchResultDataDvo>();
            //}
            //return new ScmPageResult<SearchResultDataDvo> { Items = response.Data, Total = response.Total };

            return Task.FromResult(new ScmPageResult<SearchResultDataDvo> { Items = new List<SearchResultDataDvo>(), Total = 0 });
        }
    }
}
