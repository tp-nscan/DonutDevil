using System;
using System.Collections.Generic;
using LibNode;
using NodeLib.Indexers;
using NodeLib.NgUpdaters;
using NodeLib.Params;

namespace NodeLib.NetworkOld
{
    public interface INetwork
    {
        Func<IReadOnlyDictionary<string, IParameter>, INgUpdater> NgUpdaterBuilder { get; }
        INgUpdater NgUpdater { get; }
        IReadOnlyDictionary<string, IParameter> Parameters { get; }
        IReadOnlyList<ID2Indexer> NodeGroupIndexers { get; }
        NodeGroup NodeGroup { get; }
        int Generation { get; }
    }

    public static class Network
    {
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
                generation: network.Generation + 1
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
                ngUpdater: network.NgUpdaterBuilder(paramDictionary),
                nodeGroupIndexers: network.NodeGroupIndexers,
                ngUpdaterBuilder: network.NgUpdaterBuilder,
                generation: network.Generation
            );
        }
    }

    public class NetworkImpl : INetwork
    {
        private readonly INgUpdater _ngUpdater;
        private readonly IReadOnlyDictionary<string, IParameter> _parameters;
        private readonly IReadOnlyList<ID2Indexer> _nodeGroupIndexers;
        private readonly NodeGroup _nodeGroup;
        private readonly Func<IReadOnlyDictionary<string, IParameter>, INgUpdater> _ngUpdaterBuilder;
        private readonly int _generation;

        public NetworkImpl
            (
                NodeGroup nodeGroup,
                IReadOnlyDictionary<string, IParameter> parameters, 
                INgUpdater ngUpdater,
                IReadOnlyList<ID2Indexer> nodeGroupIndexers, 
                Func<IReadOnlyDictionary<string, IParameter>, INgUpdater> ngUpdaterBuilder, int generation)
        {
            _nodeGroup = nodeGroup;
            _parameters = parameters;
            _ngUpdater = ngUpdater;
            _nodeGroupIndexers = nodeGroupIndexers;
            _ngUpdaterBuilder = ngUpdaterBuilder;
            _generation = generation;
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

        public IReadOnlyList<ID2Indexer> NodeGroupIndexers
        {
            get { return _nodeGroupIndexers; }
        }

        public NodeGroup NodeGroup
        {
            get { return _nodeGroup; }
        }

        public int Generation
        {
            get { return _generation; }
        }
    }
}
