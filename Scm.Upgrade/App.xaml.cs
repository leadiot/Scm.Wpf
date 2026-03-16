using Com.Scm.Config;
using Com.Scm.Views;
using System.Windows;

namespace Com.Scm
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new MainWindow();
            mainWindow.Show();

            try
            {
                UpgradeConfig settings = UpgradeConfig.Load();
                if (settings == null)
                {
                    settings = new UpgradeConfig();
                    settings.LoadDefault();
                }

                mainWindow.Init(settings);
            }
            catch (Exception ex)
            {
                Shutdown();
            }
        }
    }
}
