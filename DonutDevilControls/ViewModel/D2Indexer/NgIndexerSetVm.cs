using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Subjects;
using DonutDevilControls.ViewModel.Legend;
using NodeLib.Indexers;
using WpfUtils;

namespace DonutDevilControls.ViewModel.D2Indexer
{
    public static class NgIndexerSetVmEx
    {
        public static D2IndexerBase<float> Indexer1D(this NgIndexerSetVm ngIndexerSetVm)
        {
            var ivm = ngIndexerSetVm.NgIndexerVms.FirstOrDefault(vm => vm.NgIndexerState == NgIndexerVmState.OneDSelected);

            return (D2IndexerBase<float>)((ivm == null) ? null : ivm.Id2Indexer);
        }

        public static D2IndexerBase<float> Indexer2Dx(this NgIndexerSetVm ngIndexerSetVm)
        {
            var ivm = ngIndexerSetVm.NgIndexerVms.FirstOrDefault(vm => vm.NgIndexerState == NgIndexerVmState.TwoDx);

            return (D2IndexerBase<float>)((ivm == null) ? null : ivm.Id2Indexer);
        }

        public static D2IndexerBase<float> Indexer2Dy(this NgIndexerSetVm ngIndexerSetVm)
        {
            var ivm = ngIndexerSetVm.NgIndexerVms.FirstOrDefault(vm => vm.NgIndexerState == NgIndexerVmState.TwoDy);

            return (D2IndexerBase<float>)((ivm == null) ? null : ivm.Id2Indexer);
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

        void ListenToIndexerStateChanged(Tuple<NgIndexerVm, NgIndexerVmState> tuple)
        {
            if (ReEntrant)
            {
                return;
            }
            switch (tuple.Item1.NgIndexerState)
            {
                case NgIndexerVmState.OneDSelected:
                    AdjustStates(tuple.Item1, NgIndexerVmState.OneDSelected, NgIndexerVmState.OneDUnselected);
                    break;
                case NgIndexerVmState.OneDUnselected:
                    break;
                case NgIndexerVmState.TwoDx:
                    AdjustStates(tuple.Item1, NgIndexerVmState.TwoDy, NgIndexerVmState.TwoDx);
                    break;
                case NgIndexerVmState.TwoDy:
                    AdjustStates(tuple.Item1, NgIndexerVmState.TwoDx, NgIndexerVmState.TwoDy);
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
                if (vm.NgIndexerState == stateTo)
                {
                    ngIndexerVm.NgIndexerState = stateFrom;
                    return;
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
                ResetLayerSelectorVms();
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
                ResetLayerSelectorVms();
                NotifyNgDisplayStateChanged();
            }
        }

        void ResetLayerSelectorVms()
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
                        id2Indexer: NgIndexerFor(NgIndexerVmState.OneDSelected));
                }

                return NgDisplayState.TorusIndexing
                    (
                        id2IndexerX: NgIndexerFor(NgIndexerVmState.TwoDx),
                        id2IndexerY: NgIndexerFor(NgIndexerVmState.TwoDy)
                    );
            }
        }

        ID2Indexer NgIndexerFor(NgIndexerVmState ngIndexerState)
        {
            var indexerVm = NgIndexerVms.FirstOrDefault(vm => vm.NgIndexerState == ngIndexerState);
            if (indexerVm != null)
            {
                return indexerVm.Id2Indexer;
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
