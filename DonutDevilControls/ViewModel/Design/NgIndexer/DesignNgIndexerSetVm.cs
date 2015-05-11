using System.Collections.Generic;
using DonutDevilControls.ViewModel.NgIndexer;

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
                yield return NodeLib.Indexers.D2Indexer.MakeRingArray2D("Design Disabled", 5).ToNgIndexerVm(NgIndexerVmState.Disabled);
                yield return NodeLib.Indexers.D2Indexer.MakeRingArray2D("Design OneDSelected", 5).ToNgIndexerVm(NgIndexerVmState.OneDSelected);
                yield return NodeLib.Indexers.D2Indexer.MakeRingArray2D("Design OneDUnselected", 5).ToNgIndexerVm(NgIndexerVmState.OneDUnselected);
                yield return NodeLib.Indexers.D2Indexer.MakeRingArray2D("Design TwoDUnselected", 5).ToNgIndexerVm(NgIndexerVmState.TwoDUnselected);
                yield return NodeLib.Indexers.D2Indexer.MakeRingArray2D("Design TwoDx", 5).ToNgIndexerVm(NgIndexerVmState.TwoDx);
                yield return NodeLib.Indexers.D2Indexer.MakeRingArray2D("Design TwoDy", 5).ToNgIndexerVm(NgIndexerVmState.TwoDy);
            }
        }
    }

}
