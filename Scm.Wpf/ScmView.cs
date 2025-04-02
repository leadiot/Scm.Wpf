using System.Windows.Controls;

namespace Com.Scm.Wpf
{
    public interface ScmView
    {
        void Init(ScmWindow window);

        UserControl GetView();
    }
}
