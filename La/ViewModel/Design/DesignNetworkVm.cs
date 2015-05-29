using La.Model;
using LibNode;

namespace La.ViewModel.Design
{
    public class DesignNetworkVm : NetworkVm
    {
        public DesignNetworkVm() : base(DesignNetwork)
        {
        }

        public static ISym DesignNetwork
        {
            get { return null; }
        }
    }


}
