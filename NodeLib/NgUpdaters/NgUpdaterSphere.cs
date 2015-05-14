using System;
using System.Linq;
using LibNode;
using MathLib;
using MathLib.NumericTypes;
using NodeLib.Common;

namespace NodeLib.NgUpdaters
{
    public static class NgUpdaterSphere
    {

        public static INgUpdater Standard(
        string name,
        int squareSize,
        NeighborhoodType neighborhoodType,
        float stepSize,
        float noise,
        float tent,
        float saw
    )
        {
            switch (neighborhoodType)
            {
                case NeighborhoodType.Sides:
                    return new NgUpdaterImpl
                    (
                        name: name,
                        updateFunctions: Enumerable.Range(0, squareSize * squareSize)
                                .Select(n2 =>
                                        SidesFuncSaw
                                            (
                                                torusNbrhdOne: n2.ToTorus3Nbrs(squareSize, squareSize, 0),
                                                torusNbrhdTwo: n2.ToTorus3Nbrs(squareSize, squareSize, squareSize * squareSize),
                                                torusNbrhdThree: n2.ToTorus3Nbrs(squareSize, squareSize, 2 * squareSize * squareSize),
                                                step: stepSize,
                                                noise: noise,
                                                tent: tent
                                            )
                                    )
                                .ToList()
                    );


                case NeighborhoodType.Perimeter:
                    return new NgUpdaterImpl
                    (
                        name: name,
                        updateFunctions: Enumerable.Range(0, squareSize * squareSize)
                                .Select(n2 =>
                                        PerimeterFuncSaw
                                            (
                                                torusNbrhdOne: n2.ToTorus3Nbrs(squareSize, squareSize, 0),
                                                torusNbrhdTwo: n2.ToTorus3Nbrs(squareSize, squareSize, squareSize * squareSize),
                                                torusNbrhdThree: n2.ToTorus3Nbrs(squareSize, squareSize, 2*squareSize * squareSize),
                                                step: stepSize,
                                                noise: noise,
                                                tent: tent
                                            )
                                    )
                                .ToList()
                    );

                default:
                    throw new Exception("NeighborhoodType not handled");
            }
        }


        static Func<NodeGroup, Node[]> SidesFuncSaw(
            Torus3NbrhdIndexer torusNbrhdOne,
            Torus3NbrhdIndexer torusNbrhdTwo,
            Torus3NbrhdIndexer torusNbrhdThree,
            float tent,
            float step,
            float noise
        )
        {
            return (ng) =>
            {
                var cOne = ng.Values[torusNbrhdOne.CC];
                var cTwo = ng.Values[torusNbrhdTwo.CC];
                var cThree = ng.Values[torusNbrhdThree.CC];

                var vUC = SpherePoint.Thresh(
                    ng.Values[torusNbrhdOne.UC] - cOne,
                    ng.Values[torusNbrhdTwo.UC] - cTwo,
                    ng.Values[torusNbrhdThree.UC] - cThree,
                    tent
                );

                var vCF = SpherePoint.Thresh(
                    ng.Values[torusNbrhdOne.CF] - cOne,
                    ng.Values[torusNbrhdTwo.CF] - cTwo,
                    ng.Values[torusNbrhdThree.CF] - cThree,
                    tent
                );

                var vCR = SpherePoint.Thresh(
                    ng.Values[torusNbrhdOne.CR] - cOne,
                    ng.Values[torusNbrhdTwo.CR] - cTwo,
                    ng.Values[torusNbrhdThree.CR] - cThree,
                    tent
                );

                var vLC = SpherePoint.Thresh(
                    ng.Values[torusNbrhdOne.LC] - cOne,
                    ng.Values[torusNbrhdTwo.LC] - cTwo,
                    ng.Values[torusNbrhdThree.LC] - cThree,
                    tent
                );


                var sumDiff = new[]
                {
                    vUC[0] + vCF[0] + vCR[0] + vLC[0],
                    vUC[1] + vCF[1] + vCR[1] + vLC[1],
                    vUC[2] + vCF[2] + vCR[2] + vLC[2]
                };


                var preProj = new[]
                {
                    cOne +   sumDiff[0] * step + (float)((SafeRandom.NextDouble() - 0.5f) * noise),
                    cTwo +   sumDiff[1] * step + (float)((SafeRandom.NextDouble() - 0.5f) * noise),
                    cThree + sumDiff[2] * step + (float)((SafeRandom.NextDouble() - 0.5f) * noise)
                };


                var onSphere = SpherePoint.Make(preProj[0], preProj[1], preProj[2]);

                return new[]
                    {
                        new Node
                            (
                                value: onSphere[0],
                                groupIndex: torusNbrhdOne.CC
                            ),

                        new Node
                            (
                                value: onSphere[1],
                                groupIndex: torusNbrhdTwo.CC
                            ),

                        new Node
                            (
                                value: onSphere[2],
                                groupIndex: torusNbrhdThree.CC
                            )


                    };
            };

        }


        static Func<NodeGroup, Node[]> PerimeterFuncSaw(
            Torus3NbrhdIndexer torusNbrhdOne,
            Torus3NbrhdIndexer torusNbrhdTwo,
            Torus3NbrhdIndexer torusNbrhdThree,
            float tent,
            float step,
            float noise
)
        {
            return (ng) =>
            {
                var cOne = ng.Values[torusNbrhdOne.CC];
                var cTwo = ng.Values[torusNbrhdTwo.CC];
                var cThree = ng.Values[torusNbrhdThree.CC];

                var vUF = SpherePoint.Thresh(
                    ng.Values[torusNbrhdOne.UF] - cOne,
                    ng.Values[torusNbrhdTwo.UF] - cTwo,
                    ng.Values[torusNbrhdThree.UF] - cThree,
                    tent
                );

                var vUC = SpherePoint.Thresh(
                    ng.Values[torusNbrhdOne.UC] - cOne,
                    ng.Values[torusNbrhdTwo.UC] - cTwo,
                    ng.Values[torusNbrhdThree.UC] - cThree,
                    tent
                );

                var vUR = SpherePoint.Thresh(
                    ng.Values[torusNbrhdOne.UR] - cOne,
                    ng.Values[torusNbrhdTwo.UR] - cTwo,
                    ng.Values[torusNbrhdThree.UR] - cThree,
                    tent
                );

                var vCF = SpherePoint.Thresh(
                    ng.Values[torusNbrhdOne.CF] - cOne,
                    ng.Values[torusNbrhdTwo.CF] - cTwo,
                    ng.Values[torusNbrhdThree.CF] - cThree,
                    tent
                );


                var vCR = SpherePoint.Thresh(
                    ng.Values[torusNbrhdOne.CR] - cOne,
                    ng.Values[torusNbrhdTwo.CR] - cTwo,
                    ng.Values[torusNbrhdThree.CR] - cThree,
                    tent
                );

                var vLF = SpherePoint.Thresh(
                    ng.Values[torusNbrhdOne.LF] - cOne,
                    ng.Values[torusNbrhdTwo.LF] - cTwo,
                    ng.Values[torusNbrhdThree.LF] - cThree,
                    tent
                );

                var vLC = SpherePoint.Thresh(
                    ng.Values[torusNbrhdOne.LC] - cOne,
                    ng.Values[torusNbrhdTwo.LC] - cTwo,
                    ng.Values[torusNbrhdThree.LC] - cThree,
                    tent
                );

                var vLR = SpherePoint.Thresh(
                    ng.Values[torusNbrhdOne.LR] - cOne,
                    ng.Values[torusNbrhdTwo.LR] - cTwo,
                    ng.Values[torusNbrhdThree.LR] - cThree,
                    tent
                );

                var sumDiff = new[]
                {
                    vUF[0] + vUC[0] + vUR[0] + vCF[0] + vCR[0] + vLF[0] + vLC[0] + vLR[0],
                    vUF[1] + vUC[1] + vUR[1] + vCF[1] + vCR[1] + vLF[1] + vLC[1] + vLR[1],
                    vUF[2] + vUC[2] + vUR[2] + vCF[2] + vCR[2] + vLF[2] + vLC[2] + vLR[2]
                };

                var preProj = new[]
                {
                    cOne +   sumDiff[0] * step + (float)((SafeRandom.NextDouble() - 0.5f) * noise),
                    cTwo +   sumDiff[1] * step + (float)((SafeRandom.NextDouble() - 0.5f) * noise),
                    cThree + sumDiff[2] * step + (float)((SafeRandom.NextDouble() - 0.5f) * noise)
                };


                var onSphere = SpherePoint.Make(preProj[0], preProj[1], preProj[2]);

                return new[]
                    {
                        new Node
                            (
                                value: onSphere[0],
                                groupIndex: torusNbrhdOne.CC
                            ),

                        new Node
                            (
                                value: onSphere[1],
                                groupIndex: torusNbrhdTwo.CC
                            ),

                        new Node
                            (
                                value: onSphere[2],
                                groupIndex: torusNbrhdThree.CC
                            )


                    };
            };

        }
    }
}
