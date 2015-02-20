using System.ComponentModel;

namespace DonutDevilMain.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow(INotifyPropertyChanged vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
