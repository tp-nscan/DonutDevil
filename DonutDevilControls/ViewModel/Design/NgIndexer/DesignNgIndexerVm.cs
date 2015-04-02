using DonutDevilControls.ViewModel.NgIndexer;

namespace DonutDevilControls.ViewModel.Design.NgIndexer
{
    public class DesignNgIndexerVm : NgIndexerVm
    {
        public DesignNgIndexerVm()
            : base(
                NodeLib.Indexers.NgIndexer.MakeRingArray2D("Design name", 5),
                NgIndexerVmState.OneDSelected)
        {
            OptionsAreVisible = true;
        }

    }
}
