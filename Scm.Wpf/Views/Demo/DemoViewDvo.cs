using Com.Scm.Wpf;
using System.Windows.Input;

namespace Com.Scm.Views.Demo
{
    public class DemoViewDvo : ScmPageGridDvo
    {
        /// <summary>
        ///     所有数据
        /// </summary>
        private readonly List<DemoDataModel> _totalDataList = new List<DemoDataModel>();

        private string text;
        public string Text { get { return text; } set { SetProperty(ref text, value); } }

        /// <summary>
        /// 页码改变命令
        /// </summary>
        public ICommand PageUpdatedCmd { get; set; }

        public void Init()
        {
            for (var i = 0; i < 125; i++)
            {
                var item = new DemoDataModel { Id = i, Name = "Name " + i, Remark = "Remark " + i };
                _totalDataList.Add(item);
            }

            //PageUpdatedCmd = new ScmCommand(PageUpdated);
        }

        /// <summary>
        /// 页码改变
        /// </summary>
        public void PageUpdated(int index)
        {
            var list = _totalDataList.Skip((index - 1) * 10).Take(10).ToList();
            Items.Clear();
            foreach (var item in list)
            {
                Items.Add(item);
            }
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
