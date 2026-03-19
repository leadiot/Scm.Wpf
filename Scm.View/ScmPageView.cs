using System.Windows.Controls;

namespace Com.Scm
{
    public interface ScmPageView : ScmView
    {
        UserControl GetCustomView();

        UserControl GetSearchView();

        UserControl GetInfoView();

        UserControl GetEditView();
    }
}
