using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using WpfUtils;

namespace La.ViewModel
{
    public class IndexSelectorVm : NotifyPropertyChanged
    {
        private readonly Subject<IndexVm> _onSelectionChanged
            = new Subject<IndexVm>();
        public IObservable<IndexVm> OnSelectionChanged => _onSelectionChanged;

        public IndexSelectorVm(IEnumerable<int> indexes)
        {
            IndexVms = new List<IndexVm>();
            foreach (var index in indexes)
            {
                IndexVms.Add(new IndexVm(index));
            }
            IndexVm = IndexVms[0];
        }
        
        public IList<IndexVm> IndexVms { get; }

        private IndexVm _indexVm;
        public IndexVm IndexVm
        {
            get { return _indexVm; }
            set
            {
                _indexVm = value;
                _onSelectionChanged.OnNext(value);
            }
        }

    }

    public class IndexVm
    {
        public IndexVm(int index)
        {
            Index = index;
        }
        
        public int Index { get; }
    }
}
