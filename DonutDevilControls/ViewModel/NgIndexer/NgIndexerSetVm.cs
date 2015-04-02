using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Subjects;
using DonutDevilControls.ViewModel.Legend;
using NodeLib.Indexers;
using WpfUtils;

namespace DonutDevilControls.ViewModel.NgIndexer
{
    public static class NgIndexerSetVmEx
    {
        public static INgIndexer Indexer1D(this NgIndexerSetVm ngIndexerSetVm)
        {
            var ivm = ngIndexerSetVm.NgIndexerVms.FirstOrDefault(vm => vm.NgIndexerState == NgIndexerVmState.OneDSelected);

            return (ivm == null )? null : ivm.NgIndexer;
        }

        public static INgIndexer Indexer2Dx(this NgIndexerSetVm ngIndexerSetVm)
        {
            var ivm = ngIndexerSetVm.NgIndexerVms.FirstOrDefault(vm => vm.NgIndexerState == NgIndexerVmState.TwoDx);

            return (ivm == null) ? null : ivm.NgIndexer;
        }

        public static INgIndexer Indexer2Dy(this NgIndexerSetVm ngIndexerSetVm)
        {
            var ivm = ngIndexerSetVm.NgIndexerVms.FirstOrDefault(vm => vm.NgIndexerState == NgIndexerVmState.TwoDy);

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
            _legendMode = LegendMode.OneLayer;
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
                case NgIndexerVmState.OneDSelected:
                    AdjustStates(vm, NgIndexerVmState.OneDSelected, NgIndexerVmState.OneDUnselected);
                    break;
                case NgIndexerVmState.OneDUnselected:
                    break;
                case NgIndexerVmState.TwoDx:
                    AdjustStates(vm, NgIndexerVmState.TwoDx, NgIndexerVmState.TwoDy);
                    break;
                case NgIndexerVmState.TwoDy:
                    AdjustStates(vm, NgIndexerVmState.TwoDy, NgIndexerVmState.TwoDx);
                    break;
                case NgIndexerVmState.TwoDUnselected:
                    break;
                case NgIndexerVmState.Disabled:
                    break;
                default:
                    throw new Exception("NgIndexerState not handled");
            }

            NotifyNgDisplayStateChanged();
        }

        void AdjustStates(NgIndexerVm vm, NgIndexerVmState stateFrom, NgIndexerVmState stateTo)
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

        private LegendMode _legendMode;
        public LegendMode LegendMode
        {
            get { return _legendMode; }
            set
            {
                _legendMode = value;
                OnPropertyChanged("Is1D");
                OnPropertyChanged("Is2D");
            }
        }

        public bool Is1D
        {
            get { return LegendMode == LegendMode.OneLayer; }
            set
            {
                LegendMode = (value) ? LegendMode.OneLayer : LegendMode.TwoLayers;
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

        public bool Is2D
        {
            get { return LegendMode == LegendMode.TwoLayers; }
            set
            {
                LegendMode = (value) ? LegendMode.TwoLayers : LegendMode.OneLayer;
                UpdateLayerSelectorVms();
                NotifyNgDisplayStateChanged();
            }
        }

        void UpdateLayerSelectorVms()
        {
            ReEntrant = true;

            if (Is2D)
            {
                foreach (var ngIndexerVm in NgIndexerVms)
                {
                    ngIndexerVm.NgIndexerState = NgIndexerVmState.TwoDUnselected;
                }

                NgIndexerVms[0].NgIndexerState = NgIndexerVmState.TwoDx;
                NgIndexerVms[1].NgIndexerState = NgIndexerVmState.TwoDy;
            }

            if (Is1D)
            {
                foreach (var ngIndexerVm in NgIndexerVms)
                {
                    ngIndexerVm.NgIndexerState = NgIndexerVmState.OneDUnselected;
                }

                NgIndexerVms[0].NgIndexerState = NgIndexerVmState.OneDSelected;
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
                if (LegendMode == LegendMode.OneLayer)
                {
                    return NgDisplayState.RingIndexing(
                        ngIndexer: NgIndexerFor(NgIndexerVmState.OneDSelected));
                }

                return NgDisplayState.TorusIndexing
                    (
                        ngIndexerX: NgIndexerFor(NgIndexerVmState.TwoDx),
                        ngIndexerY: NgIndexerFor(NgIndexerVmState.TwoDy)
                    );
            }
        }

        INgIndexer NgIndexerFor(NgIndexerVmState ngIndexerState)
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
