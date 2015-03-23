using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Subjects;
using NodeLib.Indexers;
using WpfUtils;

namespace DonutDevilControls.ViewModel.NgIndexer
{
    public static class NgIndexerSetVmEx
    {
        public static INgIndexer RingIndexer(this NgIndexerSetVm ngIndexerSetVm)
        {
            var ivm = ngIndexerSetVm.NgIndexerVms.FirstOrDefault(vm => vm.NgIndexerState == NgIndexerState.RingSelected);

            return (ivm == null )? null : ivm.NgIndexer;
        }

        public static INgIndexer TorusXIndexer(this NgIndexerSetVm ngIndexerSetVm)
        {
            var ivm = ngIndexerSetVm.NgIndexerVms.FirstOrDefault(vm => vm.NgIndexerState == NgIndexerState.TorusX);

            return (ivm == null) ? null : ivm.NgIndexer;
        }

        public static INgIndexer TorusYIndexer(this NgIndexerSetVm ngIndexerSetVm)
        {
            var ivm = ngIndexerSetVm.NgIndexerVms.FirstOrDefault(vm => vm.NgIndexerState == NgIndexerState.TorusY);

            return (ivm == null) ? null : ivm.NgIndexer;
        }
    }


    public class NgIndexerSetVm : NotifyPropertyChanged
    {
        public NgIndexerSetVm(IEnumerable<NgIndexerVm> ngIndexerVms)
        {
            _ngIndexerVms = new ObservableCollection<NgIndexerVm>(ngIndexerVms);
            foreach (var ngIndexerVm in NgIndexerVms)
            {
                ngIndexerVm.OnNgIndexerStateChanged.Subscribe(ListenToIndexerStateChanged);
                ngIndexerVm.OptionsAreVisible = NgIndexerVms.Count > 1;
            }

            TorusIsEnabled = NgIndexerVms.Count > 1;
            _ngDisplayMode = NgDisplayMode.Ring;
        }

        private bool _reEntrant;

        private bool ReEntrant
        {
            get { return _reEntrant; }
            set
            {
                _reEntrant = value;
            }
        }

        void ListenToIndexerStateChanged(NgIndexerVm vm)
        {
            if (ReEntrant)
            {
                return;
            }
            switch (vm.NgIndexerState)
            {
                case NgIndexerState.RingSelected:
                    AdjustStates(vm, NgIndexerState.RingSelected, NgIndexerState.RingUnselected);
                    break;
                case NgIndexerState.RingUnselected:
                    break;
                case NgIndexerState.TorusX:
                    AdjustStates(vm, NgIndexerState.TorusX, NgIndexerState.TorusY);
                    break;
                case NgIndexerState.TorusY:
                    AdjustStates(vm, NgIndexerState.TorusY, NgIndexerState.TorusX);
                    break;
                case NgIndexerState.TorusUnselected:
                    break;
                case NgIndexerState.Disabled:
                    break;
                default:
                    throw new Exception("NgIndexerState not handled");
            }

            NotifyNgDisplayStateChanged();
        }

        void AdjustStates(NgIndexerVm vm, NgIndexerState stateFrom, NgIndexerState stateTo)
        {
            foreach (var ngIndexerVm in NgIndexerVms)
            {
                if (ngIndexerVm == vm)
                {
                    continue;
                }
                if (vm.NgIndexerState == stateFrom)
                {
                    ngIndexerVm.NgIndexerState = stateTo;
                }
            }

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
                NotifyNgDisplayStateChanged();
            }
        }

        private bool _torusIsEnabled;
        public bool TorusIsEnabled
        {
            get { return _torusIsEnabled; }
            set
            {
                _torusIsEnabled = value;
                OnPropertyChanged("TorusIsEnabled");
            }
        }

        public bool IsTorus
        {
            get { return NgDisplayMode == NgDisplayMode.Torus; }
            set
            {
                NgDisplayMode = (value) ? NgDisplayMode.Torus : NgDisplayMode.Ring;
                UpdateLayerSelectorVms();
                NotifyNgDisplayStateChanged();
            }
        }

        void UpdateLayerSelectorVms()
        {
            ReEntrant = true;

            if (IsTorus)
            {
                foreach (var ngIndexerVm in NgIndexerVms)
                {
                    ngIndexerVm.NgIndexerState = NgIndexerState.TorusUnselected;
                }

                NgIndexerVms[0].NgIndexerState = NgIndexerState.TorusX;
                NgIndexerVms[1].NgIndexerState = NgIndexerState.TorusY;
            }

            if (IsRing)
            {
                foreach (var ngIndexerVm in NgIndexerVms)
                {
                    ngIndexerVm.NgIndexerState = NgIndexerState.RingUnselected;
                }

                NgIndexerVms[0].NgIndexerState = NgIndexerState.RingSelected;
            }

            ReEntrant = false;
        }

        private void NotifyNgDisplayStateChanged()
        {
            _ngDisplayStateChanged.OnNext
                (
                    NgDisplayIndexing
                );
        }

        public INgDisplayIndexing NgDisplayIndexing
        {
            get
            {
                if (NgDisplayMode == NgDisplayMode.Ring)
                {
                    return NgDisplayState.RingIndexing(
                        ngIndexer: NgIndexerFor(NgIndexerState.RingSelected));
                }

                return NgDisplayState.TorusIndexing
                    (
                        ngIndexerX: NgIndexerFor(NgIndexerState.TorusX),
                        ngIndexerY: NgIndexerFor(NgIndexerState.TorusY)
                    );
            }
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

        private readonly Subject<INgDisplayIndexing> _ngDisplayStateChanged
            = new Subject<INgDisplayIndexing>();
        public IObservable<INgDisplayIndexing> OnNgDisplayStateChanged
        {
            get { return _ngDisplayStateChanged; }
        }
    }
}
