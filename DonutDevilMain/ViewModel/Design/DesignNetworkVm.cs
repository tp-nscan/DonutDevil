using NodeLib;

namespace DonutDevilMain.ViewModel.Design
{
    public class DesignNetworkVm : NetworkVm
    {
        public DesignNetworkVm()
            : base(DesignNetwork())
        {
        }

        public static INetwork DesignNetwork()
        {
            return Network.Donut(128, 1234);
        }
    }
}
