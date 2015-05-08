using System;
using System.Linq;
using MathLib;
using MathLib.NumericTypes;

namespace NodeLib.Updaters
{
    public static class NgUpdaterTwister
    {
        public static INgUpdater Standard(
                string name,
                int squareSize,
                NeighborhoodType neighborhoodType,
                float stepSizeX,
                float stepSizeY,
                float bias,
                float noise,
                float tent,
                float saw
            )
        {
            return new NgUpdaterImpl
            (
               name: name,
               updateFunctions: Enumerable.Range(0, squareSize * squareSize)
                           .Select(n2 =>
                               Ring2UsingPerimeterWithRotationalBias
                                   (
                                       torusNbrhdOne: n2.ToTorus3Nbrs(squareSize, squareSize, 0),
                                       torusNbrhdTwo: n2.ToTorus3Nbrs(squareSize, squareSize, squareSize * squareSize),
                                       stepX: stepSizeX,
                                       stepY: stepSizeY,
                                       tent: tent,
                                       saw: saw,
                                       noise: noise,
                                       bias: bias
                                   )
                               )
                           .ToList()
           );
        }




        /// <summary>
        ///  2-ring metric with perimeter nbhd and rotational bias
        /// </summary>
        static Func<INodeGroup, INode[]> Ring2UsingPerimeterWithRotationalBias(
                Torus3NbrhdIndexer torusNbrhdOne,
                Torus3NbrhdIndexer torusNbrhdTwo,
                float stepX,
                float stepY,
                float bias,
                float noise,
                float tent,
                float saw
            )
        {
            return (ng) =>
            {


                var cOne = ng.Values[torusNbrhdOne.CC];
                var cTwo = ng.Values[torusNbrhdTwo.CC];


                var biasesOne = UpdateUtils.RingRadialCosBiases(step: stepX, rBias: bias, aBias: cTwo);


                var resOne = cOne.MfDelta(ng.Values[torusNbrhdOne.UF]) * biasesOne[7];
                resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.UC]) * biasesOne[0];
                resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.UR]) * biasesOne[1];
                resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.CF]) * biasesOne[6];
                resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.CR]) * biasesOne[2];
                resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.LF]) * biasesOne[5];
                resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.LC]) * biasesOne[4];
                resOne += cOne.MfDelta(ng.Values[torusNbrhdOne.LR]) * biasesOne[3];

                resOne += (float)((SafeRandom.NextDouble() - 0.5f) * noise);


                var biasesTwo = UpdateUtils.RingRadialSinBiases(step: stepY, rBias: bias, aBias: cOne);

                var resTwo = cTwo.MfDelta(ng.Values[torusNbrhdTwo.UF]) * biasesTwo[7];
                resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.UC]) * biasesTwo[0];
                resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.UR]) * biasesTwo[1];
                resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.CF]) * biasesTwo[6];
                resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.CR]) * biasesTwo[2];
                resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.LF]) * biasesTwo[5];
                resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.LC]) * biasesTwo[4];
                resTwo += cTwo.MfDelta(ng.Values[torusNbrhdTwo.LR]) * biasesTwo[3];


                resTwo += (float)((SafeRandom.NextDouble() - 0.5f) * noise);

                return new[]
                    {
                        Node.Make
                            (
                                value: (cOne + resOne).AsMf(),
                                groupIndex: torusNbrhdOne.CC
                            ),

                        Node.Make
                            (
                                value: (cTwo + resTwo).AsMf(),
                                groupIndex: torusNbrhdTwo.CC
                            )
                    };
            };

        }











    }



}
