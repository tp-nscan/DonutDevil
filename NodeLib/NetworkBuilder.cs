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
            Func<IReadOnlyDictionary<string, IParameter>, IReadOnlyList<ID2Indexer<float>>> NgIndexMaker { get; }
        }

        public enum NetworkBuilderType
        {
            BasinCheck,
            Donut,
            LinearLocal,
            LinearClique,
            Ring,
            Sphere,
            Spots,
            Twister,
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
                    yield return LinearLocal;
                    yield return LinearClique;
                    yield return BasinCheck;
                    yield return Ring;
                    yield return Donut;
                    yield return Sphere;
                    yield return Twister;
                    yield return Spots;
                }
            }

            public static INetworkBuilder LinearLocal
            {
                get
                {
                    return new NetworkBuilderImpl
                        (
                            networkBuilderType: NetworkBuilderType.LinearLocal,
                            parameters: LinearLocalParams,
                            ngInitializer: NgInitializer.KStrideSquaredUnitZ(1),
                            ngUpdaterBuilder: NgUpdaterBuilder.ForLinear(),
                            ngIndexMaker: D2Indexer.LinearArray2DIndexMaker
                        );
                }
            }

            public static INetworkBuilder LinearClique
            {
                get
                {
                    return new NetworkBuilderImpl
                        (
                            networkBuilderType: NetworkBuilderType.LinearClique,
                            parameters: LinearCliqueParams,
                            ngInitializer: NgInitializer.KStrideSquareCliqueUnitZ(),
                            ngUpdaterBuilder: NgUpdaterBuilder.ForLinearClique(),
                            ngIndexMaker: D2Indexer.Clique2DIndexMaker
                        );
                }
            }


            public static INetworkBuilder BasinCheck
            {
                get
                {
                    return new NetworkBuilderImpl
                        (
                            networkBuilderType: NetworkBuilderType.BasinCheck,
                            parameters: BasinCheckParams,
                            ngInitializer: NgInitializer.BasinCheck(),
                            ngUpdaterBuilder: NgUpdaterBuilder.ForLinearClique(),
                            ngIndexMaker: D2Indexer.Clique2DIndexMaker
                        );
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
                            ngInitializer: NgInitializer.KStrideSquaredUnitR(1),
                            ngUpdaterBuilder: NgUpdaterBuilder.ForRing(),
                            ngIndexMaker: D2Indexer.RingArray2DIndexMaker
                        );
                }
            }

            public static INetworkBuilder Sphere
            {
                get
                {
                    return new NetworkBuilderImpl
                        (
                            networkBuilderType: NetworkBuilderType.Sphere,
                            parameters: SphereParams,
                            ngInitializer: NgInitializer.KStrideSquaredSphereZ(1),
                            ngUpdaterBuilder: NgUpdaterBuilder.ForSphere(),
                            ngIndexMaker: D2Indexer.SphereArray2DIndexMaker
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
                            ngInitializer: NgInitializer.KStrideSquaredUnitR(2),
                            ngUpdaterBuilder: NgUpdaterBuilder.ForDonut(),
                            ngIndexMaker: D2Indexer.TorusArray2DIndexMaker
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
                            ngInitializer: NgInitializer.KStrideSquaredUnitR(2),
                            ngUpdaterBuilder: NgUpdaterBuilder.ForTwister(),
                            ngIndexMaker: D2Indexer.TorusArray2DIndexMaker
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
                            ngInitializer: NgInitializer.KStrideSquaredUnitR(1),
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

            public static IReadOnlyDictionary<string, IParameter> LinearLocalParams
            {

                get
                {
                    return new IParameter[]
                        {
                            new ParamInt(4, 1024, 128, "ArrayStride", false),
                            new ParamEnum(typeof (NeighborhoodType), NeighborhoodType.Perimeter.ToString(), "NeighborhoodType", true),
                            new ParamFloat(0.0f, 0.8f, 0.1f, "StepSize", true),
                            new ParamFloat(0.0f, 0.8f, 0.1f, "Noise", true)
                        }.ToDictionary(v => v.Name);
                }
            }

            public static IReadOnlyDictionary<string, IParameter> LinearCliqueParams
            {

                get
                {
                    return new IParameter[]
                        {
                            new ParamInt(4, 64, 16, "ArrayStride", false),
                            new ParamInt(1, 640, 32, "StartSeed", false),
                            new ParamInt(1, 640, 32, "MemSeed", false),
                            new ParamInt(1, 100, 10, "MemCount", false),
                            new ParamFloat(0.0f, 1.0f, 0.4f, "StartMag", false),
                            new ParamFloat(0.0f, 1.0f, 0.1f, "CnxnMag", false),
                            new ParamFloat(0.0f, 0.02f, 0.01f, "StepSize", true),
                            new ParamFloat(0.0f, 0.5f, 0.0f, "Noise", true)
                        }.ToDictionary(v => v.Name);
                }
            }

            public static IReadOnlyDictionary<string, IParameter> BasinCheckParams
            {

                get
                {
                    return new IParameter[]
                        {
                            new ParamInt(4, 64, 32, "ArrayStride", false),
                            new ParamInt(1, 640, 32, "StartSeed", false),
                            new ParamInt(0, 640, 0, "StartIndex", false),
                            new ParamInt(1, 640, 100, "StartDistance", false),
                            new ParamInt(1, 640, 32, "MemSeed", false),
                            new ParamInt(1, 200, 100, "MemCount", false),
                            new ParamFloat(0.0f, 1.0f, 0.01f, "CnxnMag", false),
                            new ParamFloat(0.0f, 0.02f, 0.01f, "StepSize", true),
                            new ParamFloat(0.0f, 0.5f, 0.0f, "Noise", true)
                        }.ToDictionary(v => v.Name);
                }
            }

            public static IReadOnlyDictionary<string, IParameter> RingParams
            {

                get
                {
                    return new IParameter[]
                        {
                            new ParamInt(4, 1024, 128, "ArrayStride", false),
                            new ParamEnum(typeof (NeighborhoodType), NeighborhoodType.Perimeter.ToString(), "NeighborhoodType", true),
                            new ParamFloat(0.0f, 0.4f, 0.1f, "StepSize", true),
                            new ParamFloat(0.0f, 0.4f, 0.1f, "Noise", true),
                            new ParamFloat(0f, 1f, 0.5f, "Tent", true),
                            new ParamFloat(0f, 1f, 0.0f, "Saw", true)
                        }.ToDictionary(v => v.Name);
                }
            }

            public static IReadOnlyDictionary<string, IParameter> SphereParams
            {

                get
                {
                    return new IParameter[]
                        {
                            new ParamInt(4, 1024, 128, "ArrayStride", false),
                            new ParamEnum(typeof (NeighborhoodType), NeighborhoodType.Perimeter.ToString(), "NeighborhoodType", true),
                            new ParamFloat(0.0f, 0.4f, 0.1f, "StepSize", true),
                            new ParamFloat(0.0f, 0.4f, 0.1f, "Noise", true),
                            new ParamFloat(0f, 1f, 0.5f, "Tent", true),
                            new ParamFloat(0f, 1f, 0.0f, "Saw", true)
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
                            new ParamEnum(typeof(NeighborhoodType), NeighborhoodType.Perimeter.ToString(), "NeighborhoodType", true),
                            new ParamFloat(0f, 1f, 0.5f, "StepSize_X", true),
                            new ParamFloat(0f, 1f, 0.5f, "StepSize_Y", true),
                            new ParamFloat(0f, 1f, 0.0f, "Noise", true),
                            new ParamFloat(0f, 1f, 0.5f, "Tent", true),
                            new ParamFloat(0f, 1f, 0.0f, "Saw", true)
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
                            new ParamEnum(typeof(NeighborhoodType), NeighborhoodType.Perimeter.ToString(), "NeighborhoodType", true),
                            new ParamFloat(0f, 1f, 0.5f, "StepSize_X", true),
                            new ParamFloat(0f, 1f, 0.5f, "StepSize_Y", true),
                            new ParamFloat(0f, 1f, 0.5f, "Bias", true),
                            new ParamFloat(0f, 1f, 0.0f, "Noise", true),
                            new ParamFloat(0f, 1f, 0.5f, "Tent", true),
                            new ParamFloat(0f, 1f, 0.0f, "Saw", true)
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
                            new ParamEnum(typeof (NeighborhoodType), NeighborhoodType.Perimeter.ToString(), "NeighborhoodType", true),
                            new ParamFloat(0.0f, 0.4f, 0.1f, "StepSize", true),
                            new ParamFloat(0.0f, 0.4f, 0.1f, "Noise", true),
                            new ParamFloat(0f, 1f, 0.5f, "Tent", true),
                            new ParamFloat(0f, 1f, 0.0f, "Saw", true)
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
                    Func<IReadOnlyDictionary<string, IParameter>, IReadOnlyList<ID2Indexer<float>>> ngIndexMaker
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

            private readonly Func<IReadOnlyDictionary<string, IParameter>, IReadOnlyList<ID2Indexer<float>>> _ngIndexMaker;
            public Func<IReadOnlyDictionary<string, IParameter>, IReadOnlyList<ID2Indexer<float>>> NgIndexMaker
            {
                get { return _ngIndexMaker; }
            }
        }
    }
}
