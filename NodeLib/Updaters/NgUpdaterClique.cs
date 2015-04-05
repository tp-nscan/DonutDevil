using System;
using System.Linq;
using MathLib;
using MathLib.NumericTypes;

namespace NodeLib.Updaters
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

        static Func<INodeGroup, INode[]> CliqueFunc(
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
                        Node.Make
                            (
                                value: resOne.ToUnitZ(),
                                groupIndex: groupIndex
                            )
                    };
            };

        }

        static Func<INodeGroup, INode[]> NodePassThroughFunc( int offset, int count)
        {
            return (ng) => Enumerable.Range(offset, count)
                .Select(i => Node.Make(ng.Values[i], i)).ToArray();
        }


        //static Func<INodeGroup, INode[]> CliqueFunc(
        //    int groupIndex,
        //    float stepSize,
        //    float noise
        //)
        //{
        //    return (ng) =>
        //    {
        //        var resOne = ng.Values[groupIndex] * (1 - stepSize);
        //        resOne += (float)((SafeRandom.NextDouble() - 0.5f) * noise);
                

        //        return new[]
        //            {
        //                Node.Make
        //                    (
        //                        value: resOne.ToUnitZ(),
        //                        groupIndex: groupIndex
        //                    )
        //            };
        //    };

        //}






    }
}
