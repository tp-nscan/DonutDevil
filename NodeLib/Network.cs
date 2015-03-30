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
                ngUpdaterBuilder: network.NgUpdaterBuilder
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
                ngUpdaterBuilder: network.NgUpdaterBuilder
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

        public NetworkImpl
            (
                INodeGroup nodeGroup,
                IReadOnlyDictionary<string, IParameter> parameters, 
                INgUpdater ngUpdater, 
                IReadOnlyList<INgIndexer> nodeGroupIndexers, 
                Func<IReadOnlyDictionary<string, IParameter>, INgUpdater> ngUpdaterBuilder
            )
        {
            _nodeGroup = nodeGroup;
            _parameters = parameters;
            _ngUpdater = ngUpdater;
            _nodeGroupIndexers = nodeGroupIndexers;
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

        public IReadOnlyList<INgIndexer> NodeGroupIndexers
        {
            get { return _nodeGroupIndexers; }
        }

        public INodeGroup NodeGroup
        {
            get { return _nodeGroup; }
        }
    }
}
