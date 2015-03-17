using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Subjects;
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

        private NgDisplayMode _ngisDisplayMode;
        public NgDisplayMode NgisDisplayMode
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
            get { return NgisDisplayMode == NgDisplayMode.Ring; }
            set
            {
                NgisDisplayMode = (value) ? NgDisplayMode.Ring : NgDisplayMode.Torus;
                UpdateLayerSelectorVms();
                UpdateNgisDisplayState();
            }
        }

        public bool IsTorus
        {
            get { return NgisDisplayMode == NgDisplayMode.Torus; }
            set
            {
                NgisDisplayMode = (value) ? NgDisplayMode.Torus : NgDisplayMode.Ring;
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
            if (NgisDisplayMode == NgDisplayMode.Ring)
            {
                NgDisplayState =
                    NgIndexer.NgDisplayState.RingState(
                      ngIndexer:  NgIndexerFor(NgIndexerState.RingSelected));

                return;
            }

            NgDisplayState =
                NgIndexer.NgDisplayState.TorusState
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

        private readonly Subject<INgDisplayState> _ngisDisplayStateChanged
            = new Subject<INgDisplayState>();
        public IObservable<INgDisplayState> OnNgisDisplayStateChanged
        {
            get { return _ngisDisplayStateChanged; }
        }


        private INgDisplayState _ngDisplayState;
        public INgDisplayState NgDisplayState
        {
            get { return _ngDisplayState; }
            set
            {
                _ngDisplayState = value;
                _ngisDisplayStateChanged.OnNext(value);
            }
        }
    }
}
