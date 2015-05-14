using System;
using System.Linq;
using LibNode;
using MathLib;
using MathLib.NumericTypes;
using NodeLib.Common;

namespace NodeLib.NgUpdaters
{
    public static class NgUpdaterClique
    {
        public static INgUpdater Standard(
            string name,
            int arrayStride,
            int memCount,
            float stepSize,
            float noise
            )
        {
            var layerSize = arrayStride * arrayStride;

            return new NgUpdaterImpl
                (
                name: name,
                updateFunctions: Enumerable.Range(0, layerSize)
                    .Select(n2 =>
                        CliqueFunc
                            (
                                groupIndex: n2,
                                layerSize: layerSize,
                                stepSize: stepSize,
                                noise: noise
                            )
                        )
                        .Concat(new[] { NodePassThroughFunc(
                                           offset:  layerSize, 
                                           count: layerSize.ToLowerTriangularArraySize() + layerSize * memCount
                                         ) 
                        })
                        .ToList()
            );
        }

        static Func<NodeGroup, Node[]> CliqueFunc(
            int groupIndex,
            int layerSize,
            float stepSize,
            float noise
        )
        {
            return (ng) =>
            {
                var cnxns = ng.Values
                              .SymmetricArrayValuesWithZeroDiagonal(groupIndex, layerSize, layerSize)
                              .ToList();

                var diff = 0.0f;

                for (var i = 0; i < layerSize; i++)
                {
                    diff += ng.Values[i]*cnxns[i];
                }

                diff *= stepSize;
                var resOne = ng.Values[groupIndex];
                resOne += diff;
                resOne += (float)((SafeRandom.NextDouble() - 0.5f) * noise);


                return new[]
                    {
                        new Node
                            (
                                value: resOne.ToUnitZ(),
                                groupIndex: groupIndex
                            )
                    };
            };

        }

        static Func<NodeGroup, Node[]> NodePassThroughFunc( int offset, int count)
        {
            return (ng) => Enumerable.Range(offset, count)
                .Select(i => new Node(ng.Values[i], i)).ToArray();
        }

    }
}
