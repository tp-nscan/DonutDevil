using System.Collections.Generic;
using System.Linq;
using NodeLib.Indexers;
using WpfUtils;

namespace DonutDevilControls.ViewModel.NgIndexer
{
    public class NgIndexerVm : NotifyPropertyChanged
    {
        public NgIndexerVm(INgIndexer ngIndexer, NgIndexerState ngIndexerState)
        {
            _ngIndexer = ngIndexer;
            _ngIndexerState = ngIndexerState;
        }

        public string State { get { return NgIndexerState.ToString(); }}

        private NgIndexerState _ngIndexerState;
        public NgIndexerState NgIndexerState
        {
            get { return _ngIndexerState; }
            set
            {
                _ngIndexerState = value;
                OnPropertyChanged("State");
                OnPropertyChanged("IsRing");
                OnPropertyChanged("IsTorusX");
                OnPropertyChanged("IsTorusY");
            }
        }

        private readonly INgIndexer _ngIndexer;
        public INgIndexer NgIndexer
        {
            get { return _ngIndexer; }
        }

        public bool IsRing
        {
            get { return NgIndexerState == NgIndexerState.RingSelected; }
            set { NgIndexerState = (value) ? NgIndexerState.RingSelected : NgIndexerState.RingUnselected; }
        }

        public bool IsTorusX
        {
            get { return NgIndexerState == NgIndexerState.TorusX; }
            set { NgIndexerState = (value) ? NgIndexerState.TorusX : NgIndexerState.TorusUnselected; }
        }

        public bool IsTorusY
        {
            get { return NgIndexerState == NgIndexerState.TorusY; }
            set { NgIndexerState = (value) ? NgIndexerState.TorusY : NgIndexerState.TorusUnselected; }
        }
    }

    public static class NgIndexerVmEx
    {
        public static NgIndexerVm ToNgIndexerVm(this INgIndexer ngIndexer, NgIndexerState ngIndexerState)
        {
            return new NgIndexerVm(ngIndexer, ngIndexerState);
        }

        public static IEnumerable<NgIndexerVm> ToNgIndexerVms(this IReadOnlyList<INgIndexer> ngIndexer)
        {
            if (ngIndexer.Any())
            {
                yield return ngIndexer[0].ToNgIndexerVm(NgIndexerState.RingSelected);
            }
            for (var i = 1; i < ngIndexer.Count ; i++)
            {
                yield return ngIndexer[i].ToNgIndexerVm(NgIndexerState.RingUnselected);
            }
        }
    }
}
