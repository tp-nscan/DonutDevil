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
                float alpha,
                float beta,
                int squareSize, 
                bool use8Way
            )
        {
            return new NodeGroupUpdaterImpl(
                Enumerable.Range(0, squareSize * squareSize)
                          .Select(n2 =>
                                  RingFSquareBiasFunc
                                      (
                                          torusNbrhd: n2.ToTorusNbrs(squareSize, squareSize),
                                          step: step,
                                          hBias: alpha,
                                          vBias: beta
                                      )
                              )
                          .ToList()
                );
        }

        static Func<INodeGroup, INode[]> PerimeterFunc(
              TorusNbrhd torusNbrhd,
              float step
            )
        {
             return (ng) =>
             {
                 var cOne = ng.Values[torusNbrhd.CC];

                 var resOne = cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.UF]) * step;
                 resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.UC]) * step;
                 resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.UR]) * step;
                 resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.CF]) * step;
                 resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.CR]) * step;
                 resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.LF]) * step;
                 resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.LC]) * step;
                 resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.LR]) * step;

            return new[]
                    {
                        Node.Make
                            (
                                value: (cOne + resOne).AsMf(),
                                groupIndex: torusNbrhd.CC
                            )
                    };
             };

        }






        static Func<INodeGroup, INode[]> SidesFunc(
              TorusNbrhd torusNbrhd,
              float step
            )
        {
            return (ng) =>
            {
                var cOne = ng.Values[torusNbrhd.CC];

                var resOne = cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.UC]) * step;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.CF]) * step;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.CR]) * step;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.LC]) * step;

                return new[]
                    {
                        Node.Make
                            (
                                value: (cOne + resOne).AsMf(),
                                groupIndex: torusNbrhd.CC
                            )
                    };
            };

        }


        static Func<INodeGroup, INode[]> StarFunc(
          TorusNbrhd torusNbrhd,
          float step,
          float vBias
        )
        {
            var hStep = step*(1.0f - vBias*2);
            var vStep = step * (vBias * 2 - 1.0f);


            return (ng) =>
            {
                var cOne = ng.Values[torusNbrhd.CC];

                var resOne = cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.UF]) * step;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.UC]) * step;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.UR]) * step;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.CF]) * step;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.CR]) * step;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.LF]) * step;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.LC]) * step;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.LR]) * step;

                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.CFf]) * hStep;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.CRr]) * hStep;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.UuC]) * vStep;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.LlC]) * vStep;

                return new[]
                    {
                        Node.Make
                            (
                                value: (cOne + resOne).AsMf(),
                                groupIndex: torusNbrhd.CC
                            )
                    };
            };

        }

        static Func<INodeGroup, INode[]> LongStarFunc(
              TorusNbrhd torusNbrhd,
              float step,
              float hBias,
              float vBias

            )
        {

            var hInnerStep = step * (hBias - 0.5f);
            var hOuterStep = step * (hBias - 0.5f);

            var vInnerStep = step * (vBias - 0.5f);
            var vOuterStep = step * (vBias - 0.5f);



            return (ng) =>
            {
                var cOne = ng.Values[torusNbrhd.CC];

                var 
                resOne  = cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.UF]) * step;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.UC]) * step;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.UR]) * step;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.CF]) * step;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.CR]) * step;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.LF]) * step;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.LC]) * step;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.LR]) * step;

                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.CFf])  * hInnerStep;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.CFff]) * hOuterStep;

                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.CRr])  * hInnerStep;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.CRrr]) * hOuterStep;

                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.UuC]) * vInnerStep;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.UuuC]) * vOuterStep;

                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.LlC]) * vInnerStep;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.LllC]) * vOuterStep;


                return new[]
                    {
                        Node.Make
                            (
                                value: (cOne + resOne).AsMf(),
                                groupIndex: torusNbrhd.CC
                            )
                    };
            };

        }


        static Func<INodeGroup, INode[]> RingFSquareBiasFunc(
          TorusNbrhd torusNbrhd,
          float step,
          float hBias,
          float vBias

        )
        {
            var adjH = (hBias - 0.5f);
            var adjV = (vBias - 0.5f);

            var hv = step * (1.0f - adjH - adjV);
            var h = step *  (1.0f - adjH       );
            var hV = step * (1.0f - adjH + adjV);
            var v = step *  (1.0f        - adjV);
            var V = step *  (1.0f        + adjV);
            var Hv = step * (1.0f + adjH - adjV);
            var H = step *  (1.0f + adjH       );
            var HV = step * (1.0f + adjH + adjV);

            return (ng) =>
            {
                var cOne = ng.Values[torusNbrhd.CC];

                var
                resOne = cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.UF]) * hv;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.UC]) * v;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.UR]) * Hv;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.CF]) * h;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.CR]) * H;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.LF]) * hV;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.LC]) * V;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.LR]) * HV;


                return new[]
                    {
                        Node.Make
                            (
                                value: (cOne + resOne).AsMf(),
                                groupIndex: torusNbrhd.CC
                            )
                    };
            };

        }


        static Func<INodeGroup, INode[]> BigRingFunc(
              TorusNbrhd torusNbrhd,
              float step,
              float hBias,
              float vBias

        )
        {

            var hInnerStep = step * (hBias - 0.5f);
            var hOuterStep = step * (hBias - 0.5f);

            var vInnerStep = step * (vBias - 0.5f);
            var vOuterStep = step * (vBias - 0.5f);



            return (ng) =>
            {
                var cOne = ng.Values[torusNbrhd.CC];

                var
                resOne = cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.UF]) * step;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.UC]) * step;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.UR]) * step;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.CF]) * step;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.CR]) * step;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.LF]) * step;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.LC]) * step;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.LR]) * step;

                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.CFf]) * hInnerStep;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.CFff]) * hOuterStep;

                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.CRr]) * hInnerStep;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.CRrr]) * hOuterStep;

                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.UuC]) * vInnerStep;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.UuuC]) * vOuterStep;

                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.LlC]) * vInnerStep;
                resOne += cOne.MfDeltaAsFloat(ng.Values[torusNbrhd.LllC]) * vOuterStep;


                return new[]
                    {
                        Node.Make
                            (
                                value: (cOne + resOne).AsMf(),
                                groupIndex: torusNbrhd.CC
                            )
                    };
            };

        }

    }
}
