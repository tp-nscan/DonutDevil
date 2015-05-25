using La.View;
using La.ViewModel;
using System.Windows;

namespace La
{
    public partial class App
    {
        private void App_Startup(object sender, StartupEventArgs e)
        {
            var windowVm = new MainWindowVm();
            var mainWindow = new MainWindow {DataContext = windowVm};

            mainWindow.Show();
        }
    }
}
