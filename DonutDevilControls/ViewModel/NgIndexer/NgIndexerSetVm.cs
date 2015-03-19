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
            _ngDisplayMode = NgDisplayMode.Torus;
        }

        private ObservableCollection<NgIndexerVm> _ngIndexerVms;
        public ObservableCollection<NgIndexerVm> NgIndexerVms
        {
            get { return _ngIndexerVms; }
            set { _ngIndexerVms = value; }
        }

        private NgDisplayMode _ngDisplayMode;
        public NgDisplayMode NgDisplayMode
        {
            get { return _ngDisplayMode; }
            set
            {
                _ngDisplayMode = value;
                OnPropertyChanged("State");
                OnPropertyChanged("IsRing");
                OnPropertyChanged("IsTorus");
            }
        }

        public bool IsRing
        {
            get { return NgDisplayMode == NgDisplayMode.Ring; }
            set
            {
                NgDisplayMode = (value) ? NgDisplayMode.Ring : NgDisplayMode.Torus;
                UpdateLayerSelectorVms();
                UpdateNgisDisplayState();
            }
        }

        public bool IsTorus
        {
            get { return NgDisplayMode == NgDisplayMode.Torus; }
            set
            {
                NgDisplayMode = (value) ? NgDisplayMode.Torus : NgDisplayMode.Ring;
                UpdateLayerSelectorVms();
                UpdateNgisDisplayState();
            }
        }

        void UpdateLayerSelectorVms()
        {
            foreach (var ngLayerSelectorVm in NgIndexerVms)
            {
                ngLayerSelectorVm.NgIndexerState = ngLayerSelectorVm.NgIndexerState.Convert(NgDisplayMode);
            }
        }

        private void UpdateNgisDisplayState()
        {
            if (NgDisplayMode == NgDisplayMode.Ring)
            {
                NgDisplayIndexing =
                    NgIndexer.NgDisplayState.RingIndexing(
                      ngIndexer:  NgIndexerFor(NgIndexerState.RingSelected));

                return;
            }

            NgDisplayIndexing =
                NgIndexer.NgDisplayState.TorusIndexing
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

        private readonly Subject<INgDisplayIndexing> _ngisDisplayStateChanged
            = new Subject<INgDisplayIndexing>();
        public IObservable<INgDisplayIndexing> OnNgisDisplayStateChanged
        {
            get { return _ngisDisplayStateChanged; }
        }


        private INgDisplayIndexing _ngDisplayIndexing;
        public INgDisplayIndexing NgDisplayIndexing
        {
            get { return _ngDisplayIndexing; }
            set
            {
                _ngDisplayIndexing = value;
                _ngisDisplayStateChanged.OnNext(value);
            }
        }
    }
}
