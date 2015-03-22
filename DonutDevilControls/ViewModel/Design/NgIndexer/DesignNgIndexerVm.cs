using DonutDevilControls.ViewModel.NgIndexer;
using NodeLib.Indexers;

namespace DonutDevilControls.ViewModel.Design.NgIndexer
{
    public class DesignNgIndexerVm : NgIndexerVm
    {
        public DesignNgIndexerVm() : base(
            new NgIndexerImpl("Design name", null, 5, 5),
            NgIndexerState.RingSelected)
        {
            this.OptionsAreVisible = true;
        }
    }
}
