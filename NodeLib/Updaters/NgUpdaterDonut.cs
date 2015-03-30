using System;
using System.Linq;
using MathLib;
using MathLib.NumericTypes;

namespace NodeLib.Updaters
{
    public static class NgUpdaterDonut
    {
        public static INgUpdater Standard(
                string name,
                int squareSize,
                NeighborhoodType neighborhoodType,
                float stepSizeX,
                float stepSizeY,
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
                               EuclidPerimeter
                                   (
                                       torusNbrhdOne: n2.ToTorus3Nbrs(squareSize, squareSize, 0),
                                       torusNbrhdTwo: n2.ToTorus3Nbrs(squareSize, squareSize, squareSize * squareSize),
                                       stepX: stepSizeX * 10.0f,
                                       stepY: stepSizeY * 10.0f,
                                       tent: tent,
                                       saw: saw,
                                       noise: noise
                                   )
                               )
                           .ToList()
           );
        }

        static Func<INodeGroup, INode[]> EuclidPerimeter(
                Torus3NbrhdIndexer torusNbrhdOne,
                Torus3NbrhdIndexer torusNbrhdTwo,
                float stepX,
                float stepY,
                float tent,
                float saw,
                double noise
            )
        {
            return (ng) =>
            {


                var cOne = ng.Values[torusNbrhdOne.CC];
                var cTwo = ng.Values[torusNbrhdTwo.CC];

                var dUF = Mf2.VDiffSaw(cOne, cTwo, ng.Values[torusNbrhdOne.UF], ng.Values[torusNbrhdTwo.UF], tent, saw);
                var dUC = Mf2.VDiffSaw(cOne, cTwo, ng.Values[torusNbrhdOne.UC], ng.Values[torusNbrhdTwo.UC], tent, saw);
                var dUR = Mf2.VDiffSaw(cOne, cTwo, ng.Values[torusNbrhdOne.UR], ng.Values[torusNbrhdTwo.UR], tent, saw);
                var dCF = Mf2.VDiffSaw(cOne, cTwo, ng.Values[torusNbrhdOne.CF], ng.Values[torusNbrhdTwo.CF], tent, saw);
                var dCR = Mf2.VDiffSaw(cOne, cTwo, ng.Values[torusNbrhdOne.CR], ng.Values[torusNbrhdTwo.CR], tent, saw);
                var dLF = Mf2.VDiffSaw(cOne, cTwo, ng.Values[torusNbrhdOne.LF], ng.Values[torusNbrhdTwo.LF], tent, saw);
                var dLC = Mf2.VDiffSaw(cOne, cTwo, ng.Values[torusNbrhdOne.LC], ng.Values[torusNbrhdTwo.LC], tent, saw);
                var dLR = Mf2.VDiffSaw(cOne, cTwo, ng.Values[torusNbrhdOne.LR], ng.Values[torusNbrhdTwo.LR], tent, saw);


                var resOne = dUF[0] * stepX * dUF[2];
                resOne += dUC[0] * stepX * dUC[2];
                resOne += dUR[0] * stepX * dUR[2];
                resOne += dCF[0] * stepX * dCF[2];
                resOne += dCR[0] * stepX * dCR[2];
                resOne += dLF[0] * stepX * dLF[2];
                resOne += dLC[0] * stepX * dLC[2];
                resOne += dLR[0] * stepX * dLR[2];

                resOne += (float)((SafeRandom.NextDouble() - 0.5f) * noise);

                var resTwo = dUF[1] * stepY * dUF[2];
                resTwo += dUC[1] * stepY * dUC[2];
                resTwo += dUR[1] * stepY * dUR[2];
                resTwo += dCF[1] * stepY * dCF[2];
                resTwo += dCR[1] * stepY * dCR[2];
                resTwo += dLF[1] * stepY * dLF[2];
                resTwo += dLC[1] * stepY * dLC[2];
                resTwo += dLR[1] * stepY * dLR[2];

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
