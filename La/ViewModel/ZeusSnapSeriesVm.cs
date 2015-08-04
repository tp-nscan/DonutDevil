using System.Collections.ObjectModel;
using WpfUtils;

namespace La.ViewModel
{
    public class ZeusSnapSeriesVm : NotifyPropertyChanged
    {

        public ObservableCollection<ZeusSnapVm> ZeusSnapVms { get; }
            = new ObservableCollection<ZeusSnapVm>();
    }
}