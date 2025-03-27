using Com.Scm.Wpf.Views.Samples.Remote;

namespace Com.Scm.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : HandyControl.Controls.Window
{
    private ScmClient _Client;

    public MainWindow()
    {
        InitializeComponent();
    }

    public void Init(ScmClient client)
    {
        _Client = client;

        //this.DataContext = new MenuDvo();

        UcMenu.Init(client);
        UcGuid.Init(client);
        UcInfo.Init();

        Show();

        ShowView();
    }

    private void ShowView()
    {
        var view = new MainView();
        view.Init(_Client);
        GdView.Children.Add(view);
    }
}