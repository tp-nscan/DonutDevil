using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using NodeLib.Indexers;
using WpfUtils;

namespace DonutDevilControls.ViewModel.NgIndexer
{
    public class NgIndexerVm : NotifyPropertyChanged
    {
        public NgIndexerVm(INgIndexer ngIndexer, NgIndexerVmState ngIndexerState)
        {
            _ngIndexer = ngIndexer;
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

        private readonly INgIndexer _ngIndexer;
        public INgIndexer NgIndexer
        {
            get { return _ngIndexer; }
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
                NgIndexerState = (value) ? NgIndexerVmState.OneDSelected : NgIndexerVmState.OneDUnselected;
                if (value)
                {
                    _ngIndexerStateChanged.OnNext(this);
                }
            }
        }

        public bool IsTorusX
        {
            get { return NgIndexerState == NgIndexerVmState.TwoDx; }
            set
            {
                NgIndexerState = (value) ? NgIndexerVmState.TwoDx : NgIndexerVmState.TwoDUnselected;
                if (value)
                {
                    _ngIndexerStateChanged.OnNext(this);
                }
            }
        }

        public bool IsTorusY
        {
            get { return NgIndexerState == NgIndexerVmState.TwoDy; }
            set
            {
                NgIndexerState = (value) ? NgIndexerVmState.TwoDy : NgIndexerVmState.TwoDUnselected;
                if (value)
                {
                    _ngIndexerStateChanged.OnNext(this);
                }
            }
        }

        private readonly Subject<NgIndexerVm> _ngIndexerStateChanged
            = new Subject<NgIndexerVm>();
        public IObservable<NgIndexerVm> OnNgIndexerStateChanged
        {
            get { return _ngIndexerStateChanged; }
        }
    }

    public static class NgIndexerVmEx
    {
        public static NgIndexerVm ToNgIndexerVm(this INgIndexer ngIndexer, NgIndexerVmState ngIndexerState)
        {
            return new NgIndexerVm(ngIndexer, ngIndexerState);
        }

        public static IEnumerable<NgIndexerVm> ToNgIndexerVms(this IReadOnlyList<INgIndexer> ngIndexer)
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
