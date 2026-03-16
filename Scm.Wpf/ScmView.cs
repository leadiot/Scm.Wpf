using System.Windows.Controls;

namespace Com.Scm.Wpf
{
    /// <summary>
    /// SCM视图接口
    /// </summary>
    public interface ScmView
    {
        void Init(ScmWindow window);

        UserControl GetView();
    }
}
