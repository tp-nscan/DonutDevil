using System.Linq;
using DonutDevilControls.ViewModel.Params;
using NodeLib;
using NodeLib.NetworkOld;
using NodeLib.NetworkOld.NodeLib;

namespace DonutDevilControls.ViewModel.Common
{
    public class NetworkBuilderVm
    {
        public NetworkBuilderVm(INetworkBuilder networkBuilder)
        {
            _networkBuilder = networkBuilder;
            _paramSetEditorVm = new ParamSetEditorVm(NetworkBuilder.Parameters.Values.ToList(), true);
        }

        private readonly INetworkBuilder _networkBuilder;
        public INetworkBuilder NetworkBuilder
        {
            get { return _networkBuilder; }
        }

        private readonly ParamSetEditorVm _paramSetEditorVm;
        public ParamSetEditorVm ParamSetEditorVm
        {
            get { return _paramSetEditorVm; }
        }

        public INetwork BuildNetwork()
        {
            var editedBuilder = NetworkBuilder.UpdateParams(
                ParamSetEditorVm.LatestParameters.ToDictionary(p => p.Name));

            return editedBuilder.ToNetwork();
        }
    }
}
