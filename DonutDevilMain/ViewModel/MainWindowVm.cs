using WpfUtils;

namespace DonutDevilMain.ViewModel
{
    public class MainWindowVm : NotifyPropertyChanged
    {
        public MainWindowVm()
        {
           // NodeGroupVm = new TorusValuedNodeGroupVm();
            NodeGroupVm = new RingValuedNodeGroupVm();
        }

        public NotifyPropertyChanged NodeGroupVm { get; set; }
    }
}
