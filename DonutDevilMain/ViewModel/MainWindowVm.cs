using DonutDevilMain.ViewModel.Design;
using WpfUtils;

namespace DonutDevilMain.ViewModel
{
    public class MainWindowVm : NotifyPropertyChanged
    {
        public MainWindowVm()
        {
            NetworkVm = new DesignNetworkVm();
        }

        public NotifyPropertyChanged NetworkVm { get; set; }
    }
}
