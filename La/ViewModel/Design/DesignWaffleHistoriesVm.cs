using LibNode;

namespace La.ViewModel.Design
{
    public class DesignWaffleHistoriesVm : WaffleHistoriesVm
    {
        public DesignWaffleHistoriesVm() : base(DesignZeusHistories(), "C")
        {
        }

        static WaffleHistories DesignZeusHistories()
        {
            return WaffleHistBuilder.InitHistories(10, 10);
        }
    }
}
