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
            Func<IReadOnlyDictionary<string, IParameter>, IReadOnlyList<INgIndexer>> NgIndexMaker { get; }
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
            public static INetworkBuilder UpdateParams(this INetworkBuilder networkBuilder, 
                                    IReadOnlyDictionary<string, IParameter> parameters)
            {
                return new NetworkBuilderImpl
                    (
                        networkBuilderType: networkBuilder.NetworkBuilderType,
                        parameters: parameters,
                        ngInitializer: networkBuilder.NgInitializer,
                        ngUpdaterBuilder: networkBuilder.NgUpdaterBuilder,
                        ngIndexMaker: networkBuilder.NgIndexMaker
                    );
            }

            public static INetwork ToNetwork(this INetworkBuilder networkBuilder)
            {
                var arrayStride = (int)networkBuilder.Parameters["ArrayStride"].Value;
                return new NetworkImpl
                (
                    nodeGroup: networkBuilder.NgInitializer(networkBuilder.Parameters),
                    parameters: networkBuilder.Parameters,
                    ngUpdater: networkBuilder.NgUpdaterBuilder(networkBuilder.Parameters),
                    nodeGroupIndexers: networkBuilder.NgIndexMaker(networkBuilder.Parameters),
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
                            parameters: RingParams,
                            ngInitializer: NgInitializer.KStrideSquared(1),
                            ngUpdaterBuilder: NgUpdaterBuilder.ForRing(),
                            ngIndexMaker: NgIndexer.D1IndexMaker
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
                            parameters: DonutParams,
                            ngInitializer: NgInitializer.KStrideSquared(2),
                            ngUpdaterBuilder: NgUpdaterBuilder.ForDonut(),
                            ngIndexMaker: NgIndexer.D2IndexMaker
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
                            parameters: TwisterParams,
                            ngInitializer: NgInitializer.KStrideSquared(2),
                            ngUpdaterBuilder: NgUpdaterBuilder.ForTwister(),
                            ngIndexMaker: NgIndexer.D2IndexMaker
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
                            parameters: SpotParams,
                            ngInitializer: NgInitializer.KStrideSquared(1),
                            ngUpdaterBuilder: null,
                            ngIndexMaker: null
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
            //                parameters: RingParams
            //            );
            //    }
            //}


            public static IReadOnlyDictionary<string, IParameter> RingParams
            {

                get
                {
                    return new IParameter[]
                        {
                            new ParamInt(4, 1024, 128, "ArrayStride", false),
                            new ParamEnum(typeof (NeighborhoodType), NeighborhoodType.Perimeter.ToString(), "NeighborhoodType"),
                            new ParamFloat(0.0f, 0.4f, 0.1f, "StepSize"),
                            new ParamFloat(0.0f, 0.4f, 0.1f, "Noise"),
                            new ParamFloat(0f, 1f, 0.5f, "Tent"),
                            new ParamFloat(0f, 1f, 0.0f, "Saw")
                        }.ToDictionary(v => v.Name);
                }
            }

            public static IReadOnlyDictionary<string, IParameter> DonutParams
            {
                get
                {
                    return new IParameter[]
                        {
                            new ParamInt(4, 1024, 128, "ArrayStride", false),
                            new ParamEnum(typeof(NeighborhoodType), NeighborhoodType.Perimeter.ToString(), "NeighborhoodType"),
                            new ParamFloat(0f, 1f, 0.5f, "StepSize_X"),
                            new ParamFloat(0f, 1f, 0.5f, "StepSize_Y"),
                            new ParamFloat(0f, 1f, 0.0f, "Noise"),
                            new ParamFloat(0f, 1f, 0.5f, "Tent"),
                            new ParamFloat(0f, 1f, 0.0f, "Saw")
                        }.ToDictionary(v => v.Name);
                }
            }

            public static IReadOnlyDictionary<string, IParameter> TwisterParams
            {

                get
                {
                    return new IParameter[]
                        {
                            new ParamInt(4, 1024, 128, "ArrayStride", false),
                            new ParamEnum(typeof(NeighborhoodType), NeighborhoodType.Perimeter.ToString(), "NeighborhoodType"),
                            new ParamFloat(0f, 1f, 0.5f, "StepSize_X"),
                            new ParamFloat(0f, 1f, 0.5f, "StepSize_Y"),
                            new ParamFloat(0f, 1f, 0.5f, "Bias"),
                            new ParamFloat(0f, 1f, 0.0f, "Noise"),
                            new ParamFloat(0f, 1f, 0.5f, "Tent"),
                            new ParamFloat(0f, 1f, 0.0f, "Saw")
                        }.ToDictionary(v => v.Name);
                }
            }

            public static IReadOnlyDictionary<string, IParameter> SpotParams
            {

                get
                {
                    return new IParameter[]
                        {
                            new ParamInt(4, 1024, 128, "ArrayStride", false),
                            new ParamEnum(typeof (NeighborhoodType), NeighborhoodType.Perimeter.ToString(), "NeighborhoodType"),
                            new ParamFloat(0.0f, 0.4f, 0.1f, "StepSize"),
                            new ParamFloat(0.0f, 0.4f, 0.1f, "Noise"),
                            new ParamFloat(0f, 1f, 0.5f, "Tent"),
                            new ParamFloat(0f, 1f, 0.0f, "Saw")
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
                    Func<IReadOnlyDictionary<string, IParameter>, IReadOnlyList<INgIndexer>> ngIndexMaker
                )
            {
                _networkBuilderType = networkBuilderType;
                _parameters = parameters;
                _ngInitializer = ngInitializer;
                _ngUpdaterBuilder = ngUpdaterBuilder;
                _ngIndexMaker = ngIndexMaker;
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

            private readonly Func<IReadOnlyDictionary<string, IParameter>, IReadOnlyList<INgIndexer>> _ngIndexMaker;
            public Func<IReadOnlyDictionary<string, IParameter>, IReadOnlyList<INgIndexer>> NgIndexMaker
            {
                get { return _ngIndexMaker; }
            }
        }
    }
}
