using LibNode;

namespace La.ViewModel.Design
{
    public class DesignWaffleHistoriesVm : WaffleHistoriesVm
    {
        public DesignWaffleHistoriesVm() : base(DesignWaffleHistories(), "C")
        {
        }

        static WaffleHistories DesignWaffleHistories()
        {
            return WngBuilder.InitHistories(10, 10, 10);
        }
    }
}
