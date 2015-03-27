using System.Linq;
using DonutDevilControls.ViewModel.Params;
using NodeLib;
using NodeLib.NodeLib;
using WpfUtils;

namespace DonutDevilControls.ViewModel.Common
{
    public class NetworkBuilderVm
    {
        public NetworkBuilderVm(INetworkBuilder networkBuilder)
        {
            _networkBuilder = networkBuilder;
            _paramSetEditorVm.ParamVms.AddMany(networkBuilder.Parameters.Values.Select(v => v.ToParamEditorVm()));
        }

        private readonly INetworkBuilder _networkBuilder;
        public INetworkBuilder NetworkBuilder
        {
            get { return _networkBuilder; }
        }

        private ParamSetEditorVm _paramSetEditorVm = new ParamSetEditorVm();
        public ParamSetEditorVm ParamSetEditorVm
        {
            get { return _paramSetEditorVm; }
            set { _paramSetEditorVm = value; }
        }

        public INetwork BuildNetwork()
        {
            var editedBuilder = NetworkBuilder.UpdateParams(
                ParamSetEditorVm.EditedParameters.ToDictionary(p => p.Name));

            return editedBuilder.ToNetwork();
        }
    }
}
