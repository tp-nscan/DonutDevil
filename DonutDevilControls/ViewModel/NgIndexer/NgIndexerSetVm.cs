using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NodeLib.Indexers;
using WpfUtils;

namespace DonutDevilControls.ViewModel.NgIndexer
{
    public class NgIndexerSetVm : NotifyPropertyChanged
    {
        public NgIndexerSetVm(IEnumerable<NgIndexerVm> ngIndexerVms)
        {
            _ngIndexerVms = new ObservableCollection<NgIndexerVm>(ngIndexerVms);
        }

        private ObservableCollection<NgIndexerVm> _ngIndexerVms;
        public ObservableCollection<NgIndexerVm> NgIndexerVms
        {
            get { return _ngIndexerVms; }
            set { _ngIndexerVms = value; }
        }

        private NgisDisplayMode _ngisDisplayMode;
        public NgisDisplayMode NgisDisplayMode
        {
            get { return _ngisDisplayMode; }
            set
            {
                _ngisDisplayMode = value;
                OnPropertyChanged("State");
                OnPropertyChanged("IsRing");
                OnPropertyChanged("IsTorus");
            }
        }

        public bool IsRing
        {
            get { return NgisDisplayMode == NgisDisplayMode.Ring; }
            set
            {
                NgisDisplayMode = (value) ? NgisDisplayMode.Ring : NgisDisplayMode.Torus;
                UpdateLayerSelectorVms();
                UpdateNgisDisplayState();
            }
        }

        public bool IsTorus
        {
            get { return NgisDisplayMode == NgisDisplayMode.Torus; }
            set
            {
                NgisDisplayMode = (value) ? NgisDisplayMode.Torus : NgisDisplayMode.Ring;
                UpdateLayerSelectorVms();
                UpdateNgisDisplayState();
            }
        }

        void UpdateLayerSelectorVms()
        {
            foreach (var ngLayerSelectorVm in NgIndexerVms)
            {
                ngLayerSelectorVm.NgIndexerState = ngLayerSelectorVm.NgIndexerState.Convert(NgisDisplayMode);
            }
        }

        private void UpdateNgisDisplayState()
        {
            if (NgisDisplayMode == NgisDisplayMode.Ring)
            {
                NgisDisplayState =
                    NgIndexer.NgisDisplayState.RingState(
                      ngIndexer:  NgIndexerFor(NgIndexerState.RingSelected));

                return;
            }

            NgisDisplayState =
                NgIndexer.NgisDisplayState.TorusState
                (
                      ngIndexerX:  NgIndexerFor(NgIndexerState.TorusX),
                      ngIndexerY:  NgIndexerFor(NgIndexerState.TorusY)
                );
        }


        INgIndexer NgIndexerFor(NgIndexerState ngIndexerState)
        {
            var indexerVm = NgIndexerVms.FirstOrDefault(vm => vm.NgIndexerState == ngIndexerState);
            if (indexerVm != null)
            {
                return indexerVm.NgIndexer;
            }
            return null;
        }

        private INgisDisplayState _ngisDisplayState;
        public INgisDisplayState NgisDisplayState
        {
            get { return _ngisDisplayState; }
            set
            {
                _ngisDisplayState = value;
            }
        }
    }
}
