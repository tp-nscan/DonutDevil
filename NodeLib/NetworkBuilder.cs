using System;
using System.Collections.Generic;
using System.Linq;
using NodeLib.Params;
using NodeLib.Updaters;

namespace NodeLib
{
    namespace NodeLib
    {
        public interface INetworkBuilder
        {
            string Name { get;}
            Func<IReadOnlyDictionary<string, IParameter>, INodeGroup> NodeGroupInitializer { get;}
            IReadOnlyDictionary<string, IParameter> Parameters { get; }
            Func<IReadOnlyDictionary<string, IParameter>, INgUpdater> NgUpdaterBuilder { get; }
        }


        public static class NetworkBuilder
        {

            public static INetworkBuilder UpdateParams(this INetworkBuilder networkBuilder, IReadOnlyDictionary<string, IParameter> parameters)
            {
                return null;
            }

            public static INetwork ToNetwork(this INetworkBuilder networkBuilder)
            {
                return null;
            }

            public static IEnumerable<INetworkBuilder> CurrentBuilders
            {
                get
                {
                    yield return OneLayerLocal;
                    yield return Donut;
                    yield return Twister;
                    yield return Spots;
                }
            }

            public static INetworkBuilder OneLayerLocal
            {
                get
                {
                    return new NetworkBuilderImpl
                        (
                            name: "OneLayerLocal",
                            parameters: SimpleRingParams
                        );
                }
            }

            public static INetworkBuilder Donut
            {
                get
                {
                    return new NetworkBuilderImpl
                        (
                            name: "Donut",
                            parameters: SimpleRingParams
                        );
                }
            }

            public static INetworkBuilder Twister
            {
                get
                {
                    return new NetworkBuilderImpl
                        (
                            name: "Twister",
                            parameters: SimpleRingParams
                        );
                }
            }

            public static INetworkBuilder Spots
            {
                get
                {
                    return new NetworkBuilderImpl
                        (
                            name: "Spots",
                            parameters: SimpleRingParams
                        );
                }
            }

            public static INetworkBuilder TwistySpots
            {
                get
                {
                    return new NetworkBuilderImpl
                        (
                            name: "TwistySpots",
                            parameters: SimpleRingParams
                        );
                }
            }


            public static IReadOnlyDictionary<string, IParameter> SimpleRingParams
            {

                get
                {
                    return new IParameter[]
                            {
                                new ParamEnum(typeof (NeighborhoodType), NeighborhoodType.Perimeter.ToString(), "NeighborhoodType"),
                                new ParamFloat(0.0f, 0.4f, 0.1f, "StepSize"),
                                new ParamFloat(0.0f, 0.4f, 0.1f, "Noise")
                            }.ToDictionary(v => v.Name);
                }
            }

        }

        public class NetworkBuilderImpl : INetworkBuilder
        {

            private readonly string _name;
            public string Name
            {
                get { return _name; }
            }

            private Func<IReadOnlyDictionary<string, IParameter>, INodeGroup> _nodeGroupInitializer;
            public Func<IReadOnlyDictionary<string, IParameter>, INodeGroup> NodeGroupInitializer
            {
                get { return _nodeGroupInitializer; }
            }

            private readonly IReadOnlyDictionary<string, IParameter> _parameters;
            public IReadOnlyDictionary<string, IParameter> Parameters
            {
                get { return _parameters; }
            }

            private Func<IReadOnlyDictionary<string, IParameter>, INgUpdater> _ngUpdaterBuilder;

            public NetworkBuilderImpl(string name, IReadOnlyDictionary<string, IParameter> parameters)
            {
                _name = name;
                _parameters = parameters;
            }

            public Func<IReadOnlyDictionary<string, IParameter>, INgUpdater> NgUpdaterBuilder
            {
                get { return _ngUpdaterBuilder; }
            }
        }
    }
}
