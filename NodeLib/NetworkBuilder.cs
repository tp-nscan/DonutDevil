using System;
using System.Collections.Generic;
using System.Linq;
using NodeLib.Indexers;
using NodeLib.Params;
using NodeLib.Updaters;

namespace NodeLib
{
    namespace NodeLib
    {
        public interface INetworkBuilder
        {
            NetworkBuilderType NetworkBuilderType { get; }
            Func<IReadOnlyDictionary<string, IParameter>, INodeGroup> NgInitializer { get;}
            IReadOnlyDictionary<string, IParameter> Parameters { get; }
            Func<IReadOnlyDictionary<string, IParameter>, INgUpdater> NgUpdaterBuilder { get; }
            IReadOnlyList<INgIndexer> NgIndexers { get; }
        }

        public enum NetworkBuilderType
        {
            Ring,
            Donut,
            Twister,
            Spots
        }

        public static class NetworkBuilder
        {

            public static INetworkBuilder UpdateParams(this INetworkBuilder networkBuilder, IReadOnlyDictionary<string, IParameter> parameters)
            {
                return null;
            }

            public static INetwork ToNetwork(this INetworkBuilder networkBuilder)
            {
                var arrayStride = (int)networkBuilder.Parameters["ArrayStride"].Value;
                return new NetworkImpl
                (
                    nodeGroup: networkBuilder.NgInitializer(networkBuilder.Parameters),
                    parameters: networkBuilder.Parameters,
                    ngUpdater: networkBuilder.NgUpdaterBuilder(networkBuilder.Parameters),
                    nodeGroupIndexers: networkBuilder.NgIndexers,
                    ngUpdaterBuilder: networkBuilder.NgUpdaterBuilder
                );
            }

            public static IEnumerable<INetworkBuilder> CurrentBuilders
            {
                get
                {
                    yield return Ring;
                    yield return Donut;
                    yield return Twister;
                    yield return Spots;
                }
            }

            public static INetworkBuilder Ring
            {
                get
                {
                    return new NetworkBuilderImpl
                        (
                            networkBuilderType: NetworkBuilderType.Ring,
                            parameters: SimpleRingParams,
                            ngInitializer: null,
                            ngUpdaterBuilder: null,
                            ngIndexers: new[] { NgIndexer.MakeD2Float("Node values", 0) }
                        );
                }
            }

            public static INetworkBuilder Donut
            {
                get
                {
                    return new NetworkBuilderImpl
                        (
                            networkBuilderType: NetworkBuilderType.Donut,
                            parameters: SimpleRingParams,
                            ngInitializer: null,
                            ngUpdaterBuilder: null,
                            ngIndexers: null
                        );
                }
            }

            public static INetworkBuilder Twister
            {
                get
                {
                    return new NetworkBuilderImpl
                        (
                            networkBuilderType: NetworkBuilderType.Twister,
                            parameters: SimpleRingParams,
                            ngInitializer: null,
                            ngUpdaterBuilder: null,
                            ngIndexers: null
                        );
                }
            }

            public static INetworkBuilder Spots
            {
                get
                {
                    return new NetworkBuilderImpl
                        (
                            networkBuilderType: NetworkBuilderType.Spots,
                            parameters: SimpleRingParams,
                            ngInitializer: null,
                            ngUpdaterBuilder: null,
                            ngIndexers: null
                        );
                }
            }

            //public static INetworkBuilder TwistySpots
            //{
            //    get
            //    {
            //        return new NetworkBuilderImpl
            //            (
            //                networkBuilderType: "TwistySpots",
            //                parameters: SimpleRingParams
            //            );
            //    }
            //}


            public static IReadOnlyDictionary<string, IParameter> SimpleRingParams
            {

                get
                {
                    return new IParameter[]
                            {
                                new ParamInt(4, 1024, 128, "ArrayStride", false),
                                new ParamEnum(typeof (NeighborhoodType), NeighborhoodType.Perimeter.ToString(), "NeighborhoodType"),
                                new ParamFloat(0.0f, 0.4f, 0.1f, "StepSize"),
                                new ParamFloat(0.0f, 0.4f, 0.1f, "Noise")
                            }.ToDictionary(v => v.Name);
                }
            }

        }

        public class NetworkBuilderImpl : INetworkBuilder
        {
            public NetworkBuilderImpl
                (
                    NetworkBuilderType networkBuilderType, 
                    IReadOnlyDictionary<string, IParameter> parameters, 
                    Func<IReadOnlyDictionary<string, IParameter>, INodeGroup> ngInitializer, 
                    Func<IReadOnlyDictionary<string, IParameter>, INgUpdater> ngUpdaterBuilder, 
                    IReadOnlyList<INgIndexer> ngIndexers
                )
            {
                _networkBuilderType = networkBuilderType;
                _parameters = parameters;
                _ngInitializer = ngInitializer;
                _ngUpdaterBuilder = ngUpdaterBuilder;
                _ngIndexers = ngIndexers;
            }

            private readonly NetworkBuilderType _networkBuilderType;
            public NetworkBuilderType NetworkBuilderType
            {
                get { return _networkBuilderType; }
            }

            private readonly Func<IReadOnlyDictionary<string, IParameter>, INodeGroup> _ngInitializer;
            public Func<IReadOnlyDictionary<string, IParameter>, INodeGroup> NgInitializer
            {
                get { return _ngInitializer; }
            }

            private readonly IReadOnlyDictionary<string, IParameter> _parameters;
            public IReadOnlyDictionary<string, IParameter> Parameters
            {
                get { return _parameters; }
            }

            private readonly Func<IReadOnlyDictionary<string, IParameter>, INgUpdater> _ngUpdaterBuilder;
            public Func<IReadOnlyDictionary<string, IParameter>, INgUpdater> NgUpdaterBuilder
            {
                get { return _ngUpdaterBuilder; }
            }

            private readonly IReadOnlyList<INgIndexer> _ngIndexers;
            public IReadOnlyList<INgIndexer> NgIndexers
            {
                get { return _ngIndexers; }
            }
        }
    }
}
