using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NodeLib.Params;
using NodeLib.Updaters;

namespace NodeLib
{
    public interface INetwork
    {
        Func<IReadOnlyDictionary<string, IParameter>, INgUpdater> NgUpdaterBuilder { get; }
        INgUpdater NgUpdater { get; }
        IReadOnlyDictionary<string, IParameter> Parameters { get; }
        IReadOnlyList<INgLayerIndexer> NodegroupLayers { get; }
        INodeGroup NodeGroup { get; }
    }

    public static class Network
    {
        public static INetwork StandardRing(int squareSize, int seed)
        {
            return new NetworkImpl(
                nodeGroup: NodeGroup.RandomNodeGroup(squareSize*squareSize, seed),
                parameters: NgUpdaterBuilder.StandardRingParams(),
                ngUpdater: NgUpdaterBuilder.StandardRing(squareSize)(NgUpdaterBuilder.StandardRingParams()),
                nodegroupLayers: new[] { NgLayerIndexer.SquareLayer("Node values", squareSize) },
                ngUpdaterBuilder: NgUpdaterBuilder.StandardRing(squareSize)
            );
        }

        public static INetwork Update(
                this INetwork network,
                INodeGroup nodeGroup,
                IReadOnlyDictionary<string, IParameter> paramDictionary = null
            )
        {
            return new NetworkImpl(
                nodeGroup: nodeGroup,
                parameters: (paramDictionary) ?? network.Parameters,
                ngUpdater: (paramDictionary == null) ? network.NgUpdater : network.NgUpdaterBuilder(network.Parameters),
                nodegroupLayers: network.NodegroupLayers,
                ngUpdaterBuilder: network.NgUpdaterBuilder
            );
        }
    }

    public class NetworkImpl : INetwork
    {
        private readonly INgUpdater _ngUpdater;
        private readonly IReadOnlyDictionary<string, IParameter> _parameters;
        private readonly IReadOnlyList<INgLayerIndexer> _nodegroupLayers;
        private readonly INodeGroup _nodeGroup;
        private readonly Func<IReadOnlyDictionary<string, IParameter>, INgUpdater> _ngUpdaterBuilder;

        public NetworkImpl(
            INodeGroup nodeGroup,
            IReadOnlyDictionary<string, IParameter> parameters, 
            INgUpdater ngUpdater, 
            IReadOnlyList<INgLayerIndexer> nodegroupLayers, 
            Func<IReadOnlyDictionary<string, IParameter>, INgUpdater> ngUpdaterBuilder)
        {
            _nodeGroup = nodeGroup;
            _parameters = parameters;
            _ngUpdater = ngUpdater;
            _nodegroupLayers = nodegroupLayers;
            _ngUpdaterBuilder = ngUpdaterBuilder;
        }

        public Func<IReadOnlyDictionary<string, IParameter>, INgUpdater> NgUpdaterBuilder
        {
            get { return _ngUpdaterBuilder; }
        }

        public INgUpdater NgUpdater
        {
            get { return _ngUpdater; }
        }

        public IReadOnlyDictionary<string, IParameter> Parameters
        {
            get { return _parameters; }
        }

        public IReadOnlyList<INgLayerIndexer> NodegroupLayers
        {
            get { return _nodegroupLayers; }
        }

        public INodeGroup NodeGroup
        {
            get { return _nodeGroup; }
        }
    }
}
