namespace Com.Scm.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : HandyControl.Controls.Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public void Init(ScmClient client)
    {
        //this.DataContext = new MenuDvo();

        UcMenu.Init(client);
        UcGuid.Init(client);
        UcInfo.Init();

        Show();
    }
}