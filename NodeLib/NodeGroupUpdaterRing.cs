using System;
using System.Linq;
using MathLib.NumericTypes;

namespace NodeLib
{
    public static class NodeGroupUpdaterRing
    {
        public static INodeGroupUpdater ForSquareTorus
            (
                float gain, 
                float step, 
                float range, 
                int squareSize, 
                bool use8Way
            )
        {
            return new NodeGroupUpdaterImpl(
                Enumerable.Range(0, squareSize * squareSize)
                          .Select(n2 =>
                              AsymmetricRingFunc3
                                  (
                                      position: n2,
                                      step: step,
                                      range: range,
                                      squareSize: squareSize,
                                      use8Way: use8Way
                                  )
                              )
                          .ToList()
                );
        }

        static Func<INodeGroup, INode[]> StandardRingFunc(
                int position, float step, float range, int squareSize, bool use8Way)
        {
            if (use8Way)
            {
                return (ng) =>
                {
                    var orig = ng.Values[position];

                    var res =
                        position.PerimeterOnDt(squareSize, squareSize)
                            .Select(i => ng.Values[i])
                            .Sum(n => orig.MfDeltaAsFloat(n) * step);

                    return new[]
                    {
                        Node.Make
                            (
                                value: (orig + res).AsMf(),
                                groupIndex: position
                            )
                    };
                };
            }
            return (ng) =>
            {
                var orig = ng.Values[position];

                var res =
                    position.SidesOnDt(squareSize, squareSize)
                        .Select(i => ng.Values[i])
                        .Sum(n => orig.MfDeltaAsFloat(n) * step);

                return new[]
                    {
                        Node.Make
                            (
                                value: (orig + res).AsMf(),
                                groupIndex: position
                            )
                    };
            };
        }


        static Func<INodeGroup, INode[]> AsymmetricRingFunc2(
        int position, float step, float range, int squareSize, bool use8Way)
        {
            var s2 =
                position.Sides2OnDt(squareSize, squareSize);

            var baseInc = step * 0.5f;
            var hiInc = step * (0.75f + range);
            var lowInc = step * (0.25f - range);

            if (use8Way)
            {
                return (ng) =>
                {
                    var orig = ng.Values[position];
                    var nbrs =
                        s2.Select(i => ng.Values[i]).ToArray();


                    var res2 =
                            orig.MfDeltaAsFloat(nbrs[0]) * hiInc +
                            orig.MfDeltaAsFloat(nbrs[1]) * lowInc +
                            orig.MfDeltaAsFloat(nbrs[2]) * baseInc +
                            orig.MfDeltaAsFloat(nbrs[3]) * baseInc +
                            orig.MfDeltaAsFloat(nbrs[4]) * hiInc +
                            orig.MfDeltaAsFloat(nbrs[5]) * lowInc +
                            orig.MfDeltaAsFloat(nbrs[6]) * baseInc +
                            orig.MfDeltaAsFloat(nbrs[7]) * baseInc;

                    return new[]
                    {
                        Node.Make
                            (
                                value: (orig + res2).AsMf(),
                                groupIndex: position
                            )
                    };
                };
            }
            return (ng) =>
            {
                var orig = ng.Values[position];

                var nbrs =
                    s2.Select(i => ng.Values[i]).ToArray();


                var res2 =
                        orig.MfDeltaAsFloat(nbrs[0]) * baseInc +
                        orig.MfDeltaAsFloat(nbrs[1]) * baseInc +
                        orig.MfDeltaAsFloat(nbrs[2]) * hiInc +
                        orig.MfDeltaAsFloat(nbrs[3]) * lowInc +
                        orig.MfDeltaAsFloat(nbrs[4]) * baseInc +
                        orig.MfDeltaAsFloat(nbrs[5]) * baseInc +
                        orig.MfDeltaAsFloat(nbrs[6]) * hiInc +
                        orig.MfDeltaAsFloat(nbrs[7]) * lowInc 


                           ;

                return new[]
                    {
                        Node.Make
                            (
                                value: (orig + res2).AsMf(),
                                groupIndex: position
                            )
                    };
            };
        }


        static Func<INodeGroup, INode[]> AsymmetricRingFunc3(
                int position, float step, float range, int squareSize, bool use8Way)
        {
            var baseInc = step * 0.5f;
            var halfInc = baseInc * 0.5f;
            var hiInc = step * (halfInc + range);
            var lowInc = step * (halfInc - range);

            var s2 =
                position.Sides3OnDt(squareSize, squareSize);

            if (use8Way)
            {
                return (ng) =>
                {
                    var orig = ng.Values[position];
                    var nbrs =
                        s2.Select(i => ng.Values[i]).ToArray();


                    var res2 =
                        orig.MfDeltaAsFloat(nbrs[0]) * baseInc +
                        orig.MfDeltaAsFloat(nbrs[1]) * halfInc +
                        orig.MfDeltaAsFloat(nbrs[2]) * halfInc +


                        orig.MfDeltaAsFloat(nbrs[3]) * hiInc +
                        orig.MfDeltaAsFloat(nbrs[4]) * baseInc +
                        orig.MfDeltaAsFloat(nbrs[5]) * lowInc +


                        orig.MfDeltaAsFloat(nbrs[6]) * baseInc +
                        orig.MfDeltaAsFloat(nbrs[7]) * halfInc +
                        orig.MfDeltaAsFloat(nbrs[8]) * halfInc +


                        orig.MfDeltaAsFloat(nbrs[9]) * hiInc +
                        orig.MfDeltaAsFloat(nbrs[10]) * baseInc +
                        orig.MfDeltaAsFloat(nbrs[11]) * lowInc 

                               ;

                    return new[]
                    {
                        Node.Make
                            (
                                value: (orig + res2).AsMf(),
                                groupIndex: position
                            )
                    };
                };
            }
            return (ng) =>
            {
                var orig = ng.Values[position];

                var nbrs =
                    s2.Select(i => ng.Values[i]).ToArray();


                var res2 =
                    orig.MfDeltaAsFloat(nbrs[0]) * hiInc +
                    orig.MfDeltaAsFloat(nbrs[1]) * baseInc +
                    orig.MfDeltaAsFloat(nbrs[2]) * lowInc +


                    orig.MfDeltaAsFloat(nbrs[3]) * baseInc +
                    orig.MfDeltaAsFloat(nbrs[4]) * halfInc +
                    orig.MfDeltaAsFloat(nbrs[5]) * halfInc +


                    orig.MfDeltaAsFloat(nbrs[6]) * hiInc +
                    orig.MfDeltaAsFloat(nbrs[7]) * baseInc +
                    orig.MfDeltaAsFloat(nbrs[8]) * lowInc +

                    orig.MfDeltaAsFloat(nbrs[9]) * baseInc +
                    orig.MfDeltaAsFloat(nbrs[10]) * halfInc +
                    orig.MfDeltaAsFloat(nbrs[11]) * halfInc


                           ;

                return new[]
                    {
                        Node.Make
                            (
                                value: (orig + res2).AsMf(),
                                groupIndex: position
                            )
                    };
            };
        }


        static Func<INodeGroup, INode[]> AsymmetricRingFunc3p(
        int position, float step, float range, int squareSize, bool use8Way)
        {
            var s2 =
                position.Sides3OnDt(squareSize, squareSize);

            if (use8Way)
            {



                return (ng) =>
                {
                    var orig = ng.Values[position];
                    var nbrs =
                        s2.Select(i => ng.Values[i]).ToArray();


                    var res2 =
                        orig.MfDeltaAsFloat(nbrs[0]) * step * 0.5f +
                        orig.MfDeltaAsFloat(nbrs[1]) * step * 0.25f +
                        orig.MfDeltaAsFloat(nbrs[2]) * step * 0.25f +


                        orig.MfDeltaAsFloat(nbrs[3]) * step * 1.0f +
                        orig.MfDeltaAsFloat(nbrs[4]) * step * -0.5f +
                        orig.MfDeltaAsFloat(nbrs[5]) * step * 0.5f +


                        orig.MfDeltaAsFloat(nbrs[6]) * step * 0.5f +
                        orig.MfDeltaAsFloat(nbrs[7]) * step * 0.25f +
                        orig.MfDeltaAsFloat(nbrs[8]) * step * 0.25f +


                        orig.MfDeltaAsFloat(nbrs[9]) * step * 1.0f +
                        orig.MfDeltaAsFloat(nbrs[10]) * step * 0.5f +
                        orig.MfDeltaAsFloat(nbrs[11]) * step * -0.5f

                               ;

                    return new[]
                    {
                        Node.Make
                            (
                                value: (orig + res2).AsMf(),
                                groupIndex: position
                            )
                    };
                };
            }
            return (ng) =>
            {
                var orig = ng.Values[position];

                var nbrs =
                    s2.Select(i => ng.Values[i]).ToArray();


                var res2 =
                    orig.MfDeltaAsFloat(nbrs[0]) * step * 1.0f +
                    orig.MfDeltaAsFloat(nbrs[1]) * step * 0.5f +
                    orig.MfDeltaAsFloat(nbrs[2]) * step * -0.5f +


                    orig.MfDeltaAsFloat(nbrs[3]) * step * 0.5f +
                    orig.MfDeltaAsFloat(nbrs[4]) * step * 0.25f +
                    orig.MfDeltaAsFloat(nbrs[5]) * step * 0.25f +


                    orig.MfDeltaAsFloat(nbrs[6]) * step * 1.0f +
                    orig.MfDeltaAsFloat(nbrs[7]) * step * -0.5f +
                    orig.MfDeltaAsFloat(nbrs[8]) * step * 0.5f +

                    orig.MfDeltaAsFloat(nbrs[9]) * step * 0.5f +
                    orig.MfDeltaAsFloat(nbrs[10]) * step * 0.25f +
                    orig.MfDeltaAsFloat(nbrs[11]) * step * 0.25f


                           ;

                return new[]
                    {
                        Node.Make
                            (
                                value: (orig + res2).AsMf(),
                                groupIndex: position
                            )
                    };
            };
        }


        static Func<INodeGroup, INode[]> AsymmetricRingFunc4p(
                int position, float step, float range, int squareSize, bool use8Way)
        {
            var s2 =
                position.Star3OnDt(squareSize, squareSize);

            if (use8Way)
            {
                return (ng) =>
                {
                    var orig = ng.Values[position];
                    var nbrs =
                        s2.Select(i => ng.Values[i]).ToArray();


                    var res2 =
                        orig.MfDeltaAsFloat(nbrs[0]) * step * 0.5f +
                        orig.MfDeltaAsFloat(nbrs[1]) * step * 0.25f +
                        orig.MfDeltaAsFloat(nbrs[2]) * step * 0.25f +


                        orig.MfDeltaAsFloat(nbrs[3]) * step * 1.0f +
                        orig.MfDeltaAsFloat(nbrs[4]) * step * -1.0f +
                        orig.MfDeltaAsFloat(nbrs[5]) * step * 1.0f +


                        orig.MfDeltaAsFloat(nbrs[6]) * step * 0.5f +
                        orig.MfDeltaAsFloat(nbrs[7]) * step * 0.25f +
                        orig.MfDeltaAsFloat(nbrs[8]) * step * 0.25f +


                        orig.MfDeltaAsFloat(nbrs[9]) * step * 1.0f +
                        orig.MfDeltaAsFloat(nbrs[10]) * step * 1.0f +
                        orig.MfDeltaAsFloat(nbrs[11]) * step * -1.0f +



                        orig.MfDeltaAsFloat(nbrs[12]) * step * 1.0f +
                        orig.MfDeltaAsFloat(nbrs[13]) * step * 1.0f +
                        orig.MfDeltaAsFloat(nbrs[14]) * step * 1.0f +
                        orig.MfDeltaAsFloat(nbrs[15]) * step * 1.0f

                               ;

                    return new[]
                    {
                        Node.Make
                            (
                                value: (orig + res2).AsMf(),
                                groupIndex: position
                            )
                    };
                };
            }
            return (ng) =>
            {
                var orig = ng.Values[position];

                var nbrs =
                    s2.Select(i => ng.Values[i]).ToArray();


                var res2 =
                    orig.MfDeltaAsFloat(nbrs[0]) * step * 1.0f +
                    orig.MfDeltaAsFloat(nbrs[1]) * step * 0.5f +
                    orig.MfDeltaAsFloat(nbrs[2]) * step * -0.5f +


                    orig.MfDeltaAsFloat(nbrs[3]) * step * 0.5f +
                    orig.MfDeltaAsFloat(nbrs[4]) * step * 0.25f +
                    orig.MfDeltaAsFloat(nbrs[5]) * step * 0.25f +


                    orig.MfDeltaAsFloat(nbrs[6]) * step * 1.0f +
                    orig.MfDeltaAsFloat(nbrs[7]) * step * 0.5f +
                    orig.MfDeltaAsFloat(nbrs[8]) * step * -0.5f +

                    orig.MfDeltaAsFloat(nbrs[9]) * step * 0.5f +
                    orig.MfDeltaAsFloat(nbrs[10]) * step * 0.25f +
                    orig.MfDeltaAsFloat(nbrs[11]) * step * 0.25f +

                    orig.MfDeltaAsFloat(nbrs[12]) * step * 1.0f +
                    orig.MfDeltaAsFloat(nbrs[13]) * step * 1.0f +
                    orig.MfDeltaAsFloat(nbrs[14]) * step * 1.0f +
                    orig.MfDeltaAsFloat(nbrs[15]) * step * 1.0f

                           ;

                return new[]
                    {
                        Node.Make
                            (
                                value: (orig + res2).AsMf(),
                                groupIndex: position
                            )
                    };
            };
        }



    }
}
