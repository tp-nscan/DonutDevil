using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using NodeLib.Indexers;
using WpfUtils;

namespace DonutDevilControls.ViewModel.D2Indexer
{
    public class NgIndexerVm : NotifyPropertyChanged
    {
        public NgIndexerVm(ID2Indexer id2Indexer, NgIndexerVmState ngIndexerState)
        {
            _id2Indexer = id2Indexer;
            _ngIndexerState = ngIndexerState;
        }

        public string State { get { return NgIndexerState.ToString(); }}

        private NgIndexerVmState _ngIndexerState;
        public NgIndexerVmState NgIndexerState
        {
            get { return _ngIndexerState; }
            set
            {
                _ngIndexerState = value;
                OnPropertyChanged("State");
                OnPropertyChanged("Is1D");
                OnPropertyChanged("IsTorusX");
                OnPropertyChanged("IsTorusY");
            }
        }

        private readonly ID2Indexer _id2Indexer;
        public ID2Indexer Id2Indexer
        {
            get { return _id2Indexer; }
        }


        private bool _optionsAreVisible;
        public bool OptionsAreVisible
        {
            get { return _optionsAreVisible; }
            set
            {
                _optionsAreVisible = value;
                OnPropertyChanged("OptionsAreVisible");
            }
        }

        public bool Is1D
        {
            get { return NgIndexerState == NgIndexerVmState.OneDSelected; }
            set
            {
                var oldState = NgIndexerState;
                NgIndexerState = (value) ? NgIndexerVmState.OneDSelected : NgIndexerVmState.OneDUnselected;
                _ngIndexerStateChanged.OnNext
                    (new Tuple<NgIndexerVm, NgIndexerVmState>(this, oldState));
            }
        }

        public bool IsTorusX
        {
            get { return NgIndexerState == NgIndexerVmState.TwoDx; }
            set
            {
                var oldState = NgIndexerState;
                NgIndexerState = (value) ? NgIndexerVmState.TwoDx : NgIndexerVmState.TwoDUnselected;
                _ngIndexerStateChanged.OnNext
                    (new Tuple<NgIndexerVm, NgIndexerVmState>(this, oldState));
            }
        }

        public bool IsTorusY
        {
            get { return NgIndexerState == NgIndexerVmState.TwoDy; }
            set
            {
                var oldState = NgIndexerState;
                NgIndexerState = (value) ? NgIndexerVmState.TwoDy : NgIndexerVmState.TwoDUnselected;
                _ngIndexerStateChanged.OnNext
                    (new Tuple<NgIndexerVm, NgIndexerVmState>(this, oldState));
            }
        }

        private readonly Subject<Tuple<NgIndexerVm, NgIndexerVmState>> _ngIndexerStateChanged
            = new Subject<Tuple<NgIndexerVm, NgIndexerVmState>>();
        public IObservable<Tuple<NgIndexerVm, NgIndexerVmState>> OnNgIndexerStateChanged
        {
            get { return _ngIndexerStateChanged; }
        }
    }

    public static class NgIndexerVmEx
    {
        public static NgIndexerVm ToNgIndexerVm(this ID2Indexer id2Indexer, NgIndexerVmState ngIndexerState)
        {
            return new NgIndexerVm(id2Indexer, ngIndexerState);
        }

        public static IEnumerable<NgIndexerVm> ToNgIndexerVms(this IReadOnlyList<ID2Indexer> ngIndexer)
        {
            if (ngIndexer.Any())
            {
                yield return ngIndexer[0].ToNgIndexerVm(NgIndexerVmState.OneDSelected);
            }
            for (var i = 1; i < ngIndexer.Count ; i++)
            {
                yield return ngIndexer[i].ToNgIndexerVm(NgIndexerVmState.OneDUnselected);
            }
        }
    }
}
