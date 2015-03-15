﻿using System;
using System.Collections.Generic;
using System.Linq;
using MathLib;
using MathLib.NumericTypes;
using NodeLib.Params;

namespace NodeLib.Updaters
{
    public static class NgUpdaterRing
    {
        public static INgUpdater Standard(
                string name,
                int squareSize,
                int offset,
                float stepSize,
                NeighborhoodType neighborhoodType,
                float noise
            )
        {
            switch (neighborhoodType)
            {
                case NeighborhoodType.Sides:
                    return new NgUpdaterImpl
                    (
                        name: name,
                        updateFunctions: Enumerable.Range(offset, squareSize * squareSize + offset)
                                .Select(n2 =>
                                        SidesFunc
                                            (
                                                torusNbrhd: n2.ToTorusNbrs(squareSize, squareSize),
                                                step: stepSize
                                            )
                                    )
                                .ToList()
                    );

                case NeighborhoodType.Perimeter:
                    return new NgUpdaterImpl
                    (
                        name: name,
                        updateFunctions: Enumerable.Range(offset, squareSize * squareSize + offset)
                                .Select(n2 =>
                                        PerimeterFunc
                                            (
                                                torusNbrhd: n2.ToTorusNbrs(squareSize, squareSize),
                                                step: stepSize
                                            )
                                    )
                                .ToList()
                    );

                case NeighborhoodType.Star:
                    return new NgUpdaterImpl
                    (
                        name: name,
                            updateFunctions: Enumerable.Range(offset, squareSize * squareSize + offset)
                                  .Select(n2 =>
                                          StarFunc
                                              (
                                                  torusNbrhd: n2.ToTorusNbrs(squareSize, squareSize),
                                                  step: stepSize
                                              )
                                      )
                                  .ToList()
                    );

                case NeighborhoodType.DoublePerimeter:
                    return new NgUpdaterImpl
                    (
                        name: name,
                            updateFunctions: Enumerable.Range(offset, squareSize * squareSize + offset)
                                  .Select(n2 =>
                                          DoubleRingFunc
                                              (
                                                  torusNbrhd: n2.ToTorusNbrs(squareSize, squareSize),
                                                  step: stepSize
                                              )
                                      )
                                  .ToList()
                    );

                default:
                    throw new Exception("NeighboorhoodType not handled");
            }
        }


        public static INgUpdater ForSquareTorus
        (
            float gain,
            float step,
            float alpha,
            float beta,
            int squareSize
        )
        {
                var spatial = alpha*alpha*0.1f;
                return new NgUpdaterImpl(
                    name: "ForSquareTorus",
                    updateFunctions: Enumerable.Range(0, squareSize * squareSize)
                              .Select(n2 =>
                                      PeriodicFunc
                                          (
                                              torusNbrhd: n2.ToTorusNbrs(squareSize, squareSize),
                                              temporal: step * 0.20f,
                                              spatial: spatial
                                          )
                                  )
                              .ToList()
                    );
            }

        public static INgUpdater ForSquareTorus0
            (
                float gain, 
                float step,
                float alpha,
                float beta,
                int squareSize, 
                bool use8Way
            )
        {
            var biases = RingRadialCosBiases(step: step, rBias: alpha, aBias: beta);
            return new NgUpdaterImpl(
                name: "ForSquareTorus",
                updateFunctions:  Enumerable.Range(0, squareSize * squareSize)
                          .Select(n2 =>
                                  RingBiasFunc
                                      (
                                          torusNbrhd: n2.ToTorusNbrs(squareSize, squareSize),
                                          biasedSteps: biases
                                      )
                              )
                          .ToList()
                );
        }


        private static Func<INodeGroup, INode[]> PeriodicFunc(
                TorusNbrhd torusNbrhd,
                float temporal,
                float spatial
            )
        {
            var spv = (torusNbrhd.CC % 256)*spatial;
            return (ng) =>
            {
                return new[]
                    {
                        Node.Make
                            (
                                value: (spv + ng.Generation * temporal).AsMf(),
                                groupIndex: torusNbrhd.CC
                            )
                    };
            };

        }


        static Func<INodeGroup, INode[]> PerimeterFunc(
              TorusNbrhd torusNbrhd,
              float step
            )
        {
             return (ng) =>
             {
                 var cOne = ng.Values[torusNbrhd.CC];

                 var resOne = cOne.MfDelta(ng.Values[torusNbrhd.UF]) * step;
                 resOne += cOne.MfDelta(ng.Values[torusNbrhd.UC]) * step;
                 resOne += cOne.MfDelta(ng.Values[torusNbrhd.UR]) * step;
                 resOne += cOne.MfDelta(ng.Values[torusNbrhd.CF]) * step;
                 resOne += cOne.MfDelta(ng.Values[torusNbrhd.CR]) * step;
                 resOne += cOne.MfDelta(ng.Values[torusNbrhd.LF]) * step;
                 resOne += cOne.MfDelta(ng.Values[torusNbrhd.LC]) * step;
                 resOne += cOne.MfDelta(ng.Values[torusNbrhd.LR]) * step;

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

                var resOne = cOne.MfDelta(ng.Values[torusNbrhd.UC]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CF]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CR]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LC]) * step;

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
          float step
        )
        {
            return (ng) =>
            {
                var cOne = ng.Values[torusNbrhd.CC];

                var resOne = cOne.MfDelta(ng.Values[torusNbrhd.UF]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.UC]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.UR]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CF]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CR]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LF]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LC]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LR]) * step;

                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CFf]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CRr]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.UuC]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LlC]) * step;

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


        static Func<INodeGroup, INode[]> StarFuncVbias(
          TorusNbrhd torusNbrhd,
          float step,
          float vBias
        )
        {
            var hStep = step * (1.0f - vBias * 2);
            var vStep = step * (vBias * 2 - 1.0f);


            return (ng) =>
            {
                var cOne = ng.Values[torusNbrhd.CC];

                var resOne = cOne.MfDelta(ng.Values[torusNbrhd.UF]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.UC]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.UR]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CF]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CR]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LF]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LC]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LR]) * step;

                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CFf]) * hStep;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CRr]) * hStep;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.UuC]) * vStep;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LlC]) * vStep;

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
                resOne  = cOne.MfDelta(ng.Values[torusNbrhd.UF]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.UC]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.UR]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CF]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CR]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LF]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LC]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LR]) * step;

                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CFf])  * hInnerStep;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CFff]) * hOuterStep;

                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CRr])  * hInnerStep;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CRrr]) * hOuterStep;

                resOne += cOne.MfDelta(ng.Values[torusNbrhd.UuC]) * vInnerStep;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.UuuC]) * vOuterStep;

                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LlC]) * vInnerStep;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LllC]) * vOuterStep;


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


        public static float[] RingRadialCosBiases
            (
                float step,
                float rBias,
                float aBias
            )
        {
            return new[]
            {
                step*  (1.0f + rBias * aBias.UnitToCos(Perimeter.UC)),  //0
                step * (1.0f + rBias * aBias.UnitToCos(Perimeter.UR)),  //1
                step * (1.0f + rBias * aBias.UnitToCos(Perimeter.CR)),  //2
                step * (1.0f + rBias * aBias.UnitToCos(Perimeter.LR)),  //3
                step * (1.0f + rBias * aBias.UnitToCos(Perimeter.LC)),  //4
                step * (1.0f + rBias * aBias.UnitToCos(Perimeter.LF)),  //5
                step * (1.0f + rBias * aBias.UnitToCos(Perimeter.CF)),  //6
                step * (1.0f + rBias * aBias.UnitToCos(Perimeter.UF))   //7
            };

        }

        public static float[] RingRadialSinBiases
    (
        float step,
        float rBias,
        float aBias
    )
        {
            return new[]
            {
                step*  (1.0f + rBias * aBias.UnitToSin(Perimeter.UC)), 
                step * (1.0f + rBias * aBias.UnitToSin(Perimeter.UR)),
                step * (1.0f + rBias * aBias.UnitToSin(Perimeter.CR)),
                step * (1.0f + rBias * aBias.UnitToSin(Perimeter.LR)),
                step * (1.0f + rBias * aBias.UnitToSin(Perimeter.LC)),
                step * (1.0f + rBias * aBias.UnitToSin(Perimeter.LF)),
                step * (1.0f + rBias * aBias.UnitToSin(Perimeter.CF)),
                step * (1.0f + rBias * aBias.UnitToSin(Perimeter.UF))
            };

        }

        static Func<INodeGroup, INode[]> RingBiasFunc(
          TorusNbrhd torusNbrhd,
          float[] biasedSteps

        )
        {
            return (ng) =>
            {
                var cOne = ng.Values[torusNbrhd.CC];

                var
                resOne  = cOne.MfDelta(ng.Values[torusNbrhd.UC]) * biasedSteps[0];
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.UR]) * biasedSteps[1];
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CR]) * biasedSteps[2];
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LR]) * biasedSteps[3];
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LC]) * biasedSteps[4];
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LF]) * biasedSteps[5];
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CF]) * biasedSteps[6];
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.UF]) * biasedSteps[7];


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

        static Func<INodeGroup, INode[]> RingSquareBiasFunc(
          TorusNbrhd torusNbrhd,
          float step,
          float hBias,
          float vBias

        )
        {
            var adjH = (hBias - 0.5f);
            var adjV = (vBias - 0.5f);

            var hv = step * (1.0f - adjH - adjV);
            var h = step * (1.0f - adjH);
            var hV = step * (1.0f - adjH + adjV);
            var v = step * (1.0f - adjV);
            var V = step * (1.0f + adjV);
            var Hv = step * (1.0f + adjH - adjV);
            var H = step * (1.0f + adjH);
            var HV = step * (1.0f + adjH + adjV);

            return (ng) =>
            {
                var cOne = ng.Values[torusNbrhd.CC];

                var
                resOne = cOne.MfDelta(ng.Values[torusNbrhd.UF]) * hv;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.UC]) * v;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.UR]) * Hv;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CF]) * h;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CR]) * H;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LF]) * hV;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LC]) * V;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LR]) * HV;


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

        static Func<INodeGroup, INode[]> DoubleRingFunc(
              TorusNbrhd torusNbrhd,
              float step

        )
        {
            return (ng) =>
            {
                var cOne = ng.Values[torusNbrhd.CC];

                var
                resOne = cOne.MfDelta(ng.Values[torusNbrhd.UF]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.UC]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.UR]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CF]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CR]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LF]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LC]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LR]) * step;

                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CFf]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CFff]) * step;

                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CRr]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CRrr]) * step;

                resOne += cOne.MfDelta(ng.Values[torusNbrhd.UuC]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.UuuC]) * step;

                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LlC]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LllC]) * step;


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



        static Func<INodeGroup, INode[]> BigRingFuncWithBias(
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
                resOne = cOne.MfDelta(ng.Values[torusNbrhd.UF]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.UC]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.UR]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CF]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CR]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LF]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LC]) * step;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LR]) * step;

                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CFf]) * hInnerStep;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CFff]) * hOuterStep;

                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CRr]) * hInnerStep;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.CRrr]) * hOuterStep;

                resOne += cOne.MfDelta(ng.Values[torusNbrhd.UuC]) * vInnerStep;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.UuuC]) * vOuterStep;

                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LlC]) * vInnerStep;
                resOne += cOne.MfDelta(ng.Values[torusNbrhd.LllC]) * vOuterStep;


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