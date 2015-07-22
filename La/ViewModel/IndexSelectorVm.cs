using System.Collections.Generic;
using WpfUtils;

namespace La.ViewModel
{
    public class IndexSelectorVm : NotifyPropertyChanged
    {
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

        public IndexVm IndexVm { get; set; }

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
