using System;
using System.Linq;
using LibNode;
using MathLib;
using MathLib.NumericTypes;
using NodeLib.Common;

namespace NodeLib.NgUpdaters
{
    public static class NgUpdaterLinear
    {

        public static INgUpdater Standard(
        string name,
        int squareSize,
        NeighborhoodType neighborhoodType,
        float stepSize,
        float noise
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
                                        SidesFunc
                                            (
                                                torusNbrhd: n2.ToTorus3Nbrs(squareSize, squareSize, 0),
                                                step: stepSize,
                                                noise: noise
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
                                        PerimeterFunc
                                            (
                                                torusNbrhd: n2.ToTorus3Nbrs(squareSize, squareSize, 0),
                                                step: stepSize,
                                                noise: noise
                                            )
                                    )
                                .ToList()
                    );

                case NeighborhoodType.Star:
                    return new NgUpdaterImpl
                    (
                        name: name,
                        updateFunctions: Enumerable.Range(0, squareSize * squareSize)
                                .Select(n2 =>
                                        StarFunc
                                            (
                                                torusNbrhd: n2.ToTorus3Nbrs(squareSize, squareSize, 0),
                                                step: stepSize,
                                                noise: noise
                                            )
                                    )
                                .ToList()
                    );

                case NeighborhoodType.DoublePerimeter:
                    return new NgUpdaterImpl
                    (
                        name: name,
                        updateFunctions: Enumerable.Range(0, squareSize * squareSize)
                                .Select(n2 =>
                                        DoubleRingFunc
                                            (
                                                torusNbrhd: n2.ToTorus3Nbrs(squareSize, squareSize, 0),
                                                step: stepSize,
                                                noise: noise
                                            )
                                    )
                                .ToList()
                    );

                default:
                    throw new Exception("NeighborhoodType not handled");
            }
        }



        static Func<NodeGroup, Node[]> SidesFunc(
            Torus3NbrhdIndexer torusNbrhd,
            float step,
            float noise
    )
        {
            return (ng) =>
            {
                var cOne = ng.Values[torusNbrhd.CC];

                var resOne = ng.Values[torusNbrhd.UC] * step;
                resOne += ng.Values[torusNbrhd.CF] * step;
                resOne += ng.Values[torusNbrhd.CR] * step;
                resOne += ng.Values[torusNbrhd.LC] * step;

                resOne += (float)((SafeRandom.NextDouble() - 0.5f) * noise);

                return new[]
                    {
                        new Node
                            (
                                value: (cOne + resOne).ToUnitZ(),
                                groupIndex: torusNbrhd.CC
                            )
                    };
            };

        }



        static Func<NodeGroup, Node[]> PerimeterFunc(
            Torus3NbrhdIndexer torusNbrhd,
            float step,
            float noise
        )
        {
            return (ng) =>
            {
                var cOne = ng.Values[torusNbrhd.CC];

                var resOne = ng.Values[torusNbrhd.UF] * step;
                resOne += ng.Values[torusNbrhd.UC] * step;
                resOne += ng.Values[torusNbrhd.UR] * step;
                resOne += ng.Values[torusNbrhd.CF] * step;
                resOne += ng.Values[torusNbrhd.CR] * step;
                resOne += ng.Values[torusNbrhd.LF] * step;
                resOne += ng.Values[torusNbrhd.LC] * step;
                resOne += ng.Values[torusNbrhd.LR] * step;

                resOne += (float)((SafeRandom.NextDouble() - 0.5f) * noise);

                return new[]
                    {
                        new Node
                            (
                                value: (cOne + resOne).ToUnitZ(),
                                groupIndex: torusNbrhd.CC
                            )
                    };
            };

        }

        static Func<NodeGroup, Node[]> StarFunc(
    Torus3NbrhdIndexer torusNbrhd,
    float step,
    float noise
)
        {
            return (ng) =>
            {
                var cOne = ng.Values[torusNbrhd.CC];

                var resOne = ng.Values[torusNbrhd.UF] * step;
                resOne += ng.Values[torusNbrhd.UC] * step;
                resOne += ng.Values[torusNbrhd.UR] * step;
                resOne += ng.Values[torusNbrhd.CF] * step;
                resOne += ng.Values[torusNbrhd.CR] * step;
                resOne += ng.Values[torusNbrhd.LF] * step;
                resOne += ng.Values[torusNbrhd.LC] * step;
                resOne += ng.Values[torusNbrhd.LR] * step;

                resOne += ng.Values[torusNbrhd.CFf] * step;
                resOne += ng.Values[torusNbrhd.CRr] * step;
                resOne += ng.Values[torusNbrhd.UuC] * step;
                resOne += ng.Values[torusNbrhd.LlC] * step;

                resOne += (float)((SafeRandom.NextDouble() - 0.5f) * noise);

                return new[]
                    {
                        new Node
                            (
                                value: (cOne + resOne).ToUnitZ(),
                                groupIndex: torusNbrhd.CC
                            )
                    };
            };

        }


        static Func<NodeGroup, Node[]> DoubleRingFunc(
            Torus3NbrhdIndexer torusNbrhd,
            float step,
            float noise
        )
        {
            return (ng) =>
            {
                var cOne = ng.Values[torusNbrhd.CC];

                var
                resOne =  ng.Values[torusNbrhd.UF] * step;
                resOne += ng.Values[torusNbrhd.UC] * step;
                resOne += ng.Values[torusNbrhd.UR] * step;
                resOne += ng.Values[torusNbrhd.CF] * step;
                resOne += ng.Values[torusNbrhd.CR] * step;
                resOne += ng.Values[torusNbrhd.LF] * step;
                resOne += ng.Values[torusNbrhd.LC] * step;
                resOne += ng.Values[torusNbrhd.LR] * step;

                resOne += ng.Values[torusNbrhd.CFf] * step;
                resOne += ng.Values[torusNbrhd.CFff] * step;

                resOne += ng.Values[torusNbrhd.CRr] * step;
                resOne += ng.Values[torusNbrhd.CRrr] * step;

                resOne += ng.Values[torusNbrhd.UuC] * step;
                resOne += ng.Values[torusNbrhd.UuuC] * step;

                resOne += ng.Values[torusNbrhd.LlC] * step;
                resOne += ng.Values[torusNbrhd.LllC] * step;

                resOne += (float)((SafeRandom.NextDouble() - 0.5f) * noise);

                return new[]
                    {
                        new Node
                            (
                                value: (cOne + resOne).ToUnitZ(),
                                groupIndex: torusNbrhd.CC
                            )
                    };
            };

        }







    }
}
