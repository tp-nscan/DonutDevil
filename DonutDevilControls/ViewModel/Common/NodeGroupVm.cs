using System.Collections.Generic;
using System.Linq;
using WpfUtils;

namespace DonutDevilControls.ViewModel.Common
{
    public interface INodeGroupVm
    {
        IList<INodeVm> NodeVms { get; }
        double Width { get; }
        double Height { get; }
    }

    public static class NodeGroupVm
    {
        public static INodeGroupVm ToNodeGroupVm(this IEnumerable<INodeVm> nodeVms)
        {
            return new NodeGroupVmImpl(nodeVms);
        }
    }

    public class NodeGroupVmImpl : NotifyPropertyChanged, INodeGroupVm
    {
        public NodeGroupVmImpl(IEnumerable<INodeVm> nodeVms)
        {
            _nodeVms = nodeVms.ToList();
            _height = NodeVms.Max(vm => vm.LowRightY());
            _width = NodeVms.Max(vm => vm.LowRightX());
        }

        private readonly IList<INodeVm> _nodeVms;
        public IList<INodeVm> NodeVms
        {
            get { return _nodeVms; }
        }

        private readonly double _width;
        public double Width
        {
            get { return _width; }
        }

        private readonly double _height;
        public double Height
        {
            get { return _height; }
        }
    }
}
