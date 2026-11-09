using Com.Scm.Dvo;

namespace Com.Scm.Views.Demo
{
    public class DemoViewDvo : ScmPageGridDvo<DemoDataModel>
    {
        /// <summary>
        ///     所有数据
        /// </summary>
        private readonly List<DemoDataModel> _totalDataList = new List<DemoDataModel>();

        private string text;
        public string Text { get { return text; } set { SetProperty(ref text, value); } }

        public void Init()
        {
            for (var i = 0; i < 125; i++)
            {
                var item = new DemoDataModel { Id = i, Name = "Name " + i, Remark = "Remark " + i };
                _totalDataList.Add(item);
            }

            PageItems = 10;
        }

        protected override Task<ScmPageResult<DemoDataModel>> LoadPageAsync()
        {
            var list = _totalDataList.Skip((PageIndex - 1) * PageItems).Take(PageItems).ToList();
            return Task.FromResult(new ScmPageResult<DemoDataModel> { Items = list, Total = _totalDataList.Count });
        }
    }

    public class DemoDataModel : ScmSearchResultItemDvo
    {
        public string name;
        public string Name { get { return name; } set { SetProperty(ref name, value); } }

        public string remark;
        public string Remark { get { return remark; } set { SetProperty(ref remark, value); } }
    }
}
