using System.Linq;

namespace La.ViewModel.Design
{
    public class DesignIndexSelectorVm : IndexSelectorVm
    {
        public DesignIndexSelectorVm() : base(Enumerable.Range(0,50))
        {
        }
    }
}
