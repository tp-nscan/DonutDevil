using System.Collections.Generic;
using DonutDevilControls.ViewModel.NgIndexer;
using NodeLib.Indexers;

namespace DonutDevilControls.ViewModel.Design.NgIndexer
{
    public class DesignNgIndexerSetVm : NgIndexerSetVm
    {
        public DesignNgIndexerSetVm()
            : base(DesignNgIndexerVms)
        {
        }

        static IEnumerable<NgIndexerVm> DesignNgIndexerVms
        {
            get
            {
                yield return new NgIndexerImpl("Design Disabled", null, 5, 5).ToNgIndexerVm(NgIndexerState.Disabled);
                yield return new NgIndexerImpl("Design RingSelected", null, 5, 5).ToNgIndexerVm(NgIndexerState.RingSelected);
                yield return new NgIndexerImpl("Design RingUnselected", null, 5, 5).ToNgIndexerVm(NgIndexerState.RingUnselected);
                yield return new NgIndexerImpl("Design TorusUnselected", null, 5, 5).ToNgIndexerVm(NgIndexerState.TorusUnselected);
                yield return new NgIndexerImpl("Design TorusX", null, 5, 5).ToNgIndexerVm(NgIndexerState.TorusX);
                yield return new NgIndexerImpl("Design TorusY", null, 5, 5).ToNgIndexerVm(NgIndexerState.TorusY);
            }
        }
    }

    
}
