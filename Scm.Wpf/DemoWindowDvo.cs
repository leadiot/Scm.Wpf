using Com.Scm.Dvo;
using System.Windows.Input;

namespace Com.Scm
{
    public class DemoWindowDvo : ScmDvo
    {
        public ICommand OpenFileCommand { get; }

        private ScmWindow _Window;

        public DemoWindowDvo()
        {
        }

        public void Init(ScmWindow window)
        {
            _Window = window;
        }
    }
}
