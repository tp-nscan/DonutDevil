using System;
using System.Collections.Generic;
using NodeLib.Indexers;
using NodeLib.Params;
using NodeLib.Updaters;

namespace NodeLib
{
    public interface INetwork
    {
        Func<IReadOnlyDictionary<string, IParameter>, INgUpdater> NgUpdaterBuilder { get; }
        INgUpdater NgUpdater { get; }
        IReadOnlyDictionary<string, IParameter> Parameters { get; }
        IReadOnlyList<INgIndexer> NodeGroupIndexers { get; }
        INodeGroup NodeGroup { get; }
        int SquareSize { get; }
    }

    public static class Network
    {
        public static INetwork StandardRing(int squareSize, int seed)
        {
            return new NetworkImpl(
                nodeGroup: NodeGroup.RandomNodeGroup(squareSize*squareSize, seed),
                parameters: NgUpdaterBuilder.StandardRingParams(),
                ngUpdater: NgUpdaterBuilder.StandardRing(squareSize)(NgUpdaterBuilder.StandardRingParams()),
                nodeGroupIndexers: new[] { NgIndexer.MakeD2Float("Node values", squareSize) },
                ngUpdaterBuilder: NgUpdaterBuilder.StandardRing(squareSize),
                squareSize: squareSize
            );
        }

        public static INetwork UpdateNodeGroup(
            this INetwork network
        )
        {
            return new NetworkImpl(
                nodeGroup: network.NgUpdater.Update(network.NodeGroup),
                parameters: network.Parameters,
                ngUpdater: network.NgUpdater,
                nodeGroupIndexers: network.NodeGroupIndexers,
                ngUpdaterBuilder: network.NgUpdaterBuilder,
                squareSize: network.SquareSize
            );
        }

        public static INetwork UpdateParams(
            this INetwork network,
            IReadOnlyDictionary<string, IParameter> paramDictionary
        )
        {
            return new NetworkImpl(
                nodeGroup: network.NodeGroup,
                parameters: paramDictionary,
                ngUpdater: network.NgUpdaterBuilder(network.Parameters),
                nodeGroupIndexers: network.NodeGroupIndexers,
                ngUpdaterBuilder: network.NgUpdaterBuilder,
                squareSize: network.SquareSize
            );
        }

    }

    public class NetworkImpl : INetwork
    {
        private readonly INgUpdater _ngUpdater;
        private readonly IReadOnlyDictionary<string, IParameter> _parameters;
        private readonly IReadOnlyList<INgIndexer> _nodeGroupIndexers;
        private readonly INodeGroup _nodeGroup;
        private readonly Func<IReadOnlyDictionary<string, IParameter>, INgUpdater> _ngUpdaterBuilder;
        private readonly int _squareSize;

        public NetworkImpl(
            INodeGroup nodeGroup,
            IReadOnlyDictionary<string, IParameter> parameters, 
            INgUpdater ngUpdater, 
            IReadOnlyList<INgIndexer> nodeGroupIndexers, 
            Func<IReadOnlyDictionary<string, IParameter>, INgUpdater> ngUpdaterBuilder, 
            int squareSize)
        {
            _nodeGroup = nodeGroup;
            _parameters = parameters;
            _ngUpdater = ngUpdater;
            _nodeGroupIndexers = nodeGroupIndexers;
            _ngUpdaterBuilder = ngUpdaterBuilder;
            _squareSize = squareSize;
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

        public IReadOnlyList<INgIndexer> NodeGroupIndexers
        {
            get { return _nodeGroupIndexers; }
        }

        public INodeGroup NodeGroup
        {
            get { return _nodeGroup; }
        }

        public int SquareSize
        {
            get { return _squareSize; }
        }
    }
}
