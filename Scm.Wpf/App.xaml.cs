using Com.Scm.Dao;
using System.Windows;

namespace Com.Scm;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnExit(ExitEventArgs e)
    {
        SqlHelper.Close();
        base.OnExit(e);
    }
}

