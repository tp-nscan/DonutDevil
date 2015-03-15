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
            return Network.StandardRing(128, 1234);
        }
    }
}
