using DonutDevilMain.ViewModel.Design;
using WpfUtils;

namespace DonutDevilMain.ViewModel
{
    public class MainWindowVm : NotifyPropertyChanged
    {
        public MainWindowVm()
        {
            NetworkVm = new DesignNetworkVm();
            RingValuedNodeGroupVm = new RingValuedNodeGroupVm2();
        }

        public NotifyPropertyChanged NetworkVm { get; set; }

        public RingValuedNodeGroupVm2 RingValuedNodeGroupVm { get; set; }
    }
}
