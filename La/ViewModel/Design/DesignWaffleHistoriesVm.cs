using LibNode;

namespace La.ViewModel.Design
{
    public class DesignZeusHistoriesVm : ZeusHistoriesVm
    {
        public DesignZeusHistoriesVm() : base(DesignZeusHistories(), "C")
        {
        }

        static WaffleHistories DesignZeusHistories()
        {
            return WngBuilder.InitHistories(10, 10, 10);
        }
    }
}
