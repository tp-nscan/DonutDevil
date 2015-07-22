using LibNode;

namespace La.ViewModel.Design
{
    public class DesignWaffleHistoriesVm : WaffleHistoriesVm
    {
        public DesignWaffleHistoriesVm() : base(DesignWaffleHistories())
        {
        }

        static WaffleHistories DesignWaffleHistories()
        {
            return WngBuilder.InitHistories(10, 10);
        }
    }
}
