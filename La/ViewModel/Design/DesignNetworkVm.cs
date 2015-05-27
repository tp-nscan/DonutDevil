using La.Model;

namespace La.ViewModel.Design
{
    public class DesignNetworkVm : NetworkVm
    {
        public DesignNetworkVm() : base(DesignNetwork)
        {
        }

        public static INetwork DesignNetwork
        {
            get { return null; }
        }
    }


}
