using WpfUtils;

namespace DonutDevilMain.ViewModel
{
    public class MainWindowVm : NotifyPropertyChanged
    {
        public MainWindowVm()
        {
            NodeGroupVm = new TorusValuedNodeGroupVm();
        }

        public NotifyPropertyChanged NodeGroupVm { get; set; }
    }
}
