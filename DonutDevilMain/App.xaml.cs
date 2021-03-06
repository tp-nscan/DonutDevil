﻿using System.Windows;
using DonutDevilMain.View;
using DonutDevilMain.ViewModel;

namespace DonutDevilMain
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void App_Startup(object sender, StartupEventArgs e)
        {

            var windowVm = new MainWindowVm();
            var mainWindow = new MainWindow();

            mainWindow.DataContext = windowVm;
            mainWindow.Show();
        }
    }
}
