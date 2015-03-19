using DonutDevilMain.ViewModel.Design;
using WpfUtils;

namespace DonutDevilMain.ViewModel
{
    public class MainWindowVm : NotifyPropertyChanged
    {
        public MainWindowVm()
        {
            NetworkVm = new DesignNetworkVm();
            RingValuedNodeGroupVm = new RingValuedNodeGroupVm();
        }

        public NotifyPropertyChanged NetworkVm { get; set; }

        public RingValuedNodeGroupVm RingValuedNodeGroupVm { get; set; }
    }
}
