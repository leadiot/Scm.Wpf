using Com.Scm.Wpf.Dvo;

namespace Com.Scm.Views.Demo
{
    public class DemoViewDvo : ScmDvo
    {
        private string text;
        public string Text { get { return text; } set { SetProperty(ref text, value); } }
    }
}
