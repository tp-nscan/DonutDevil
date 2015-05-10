using System;
using System.Collections.Generic;
using System.Linq;
using MathLib.NumericTypes;
using NodeLib.Params;
using MathLib;

namespace NodeLib
{
    public class NgInitializer
    {
        public static Func<IReadOnlyDictionary<string, IParameter>, INodeGroup> KStrideSquaredUnitR(int k)
        {
            return d =>
            {
                var arrayStride = (int)d["ArrayStride"].Value;
                return NodeGroup.RandomNodeGroupUnitR(k * arrayStride * arrayStride, (int)DateTime.Now.Ticks);

            };
        }

        public static Func<IReadOnlyDictionary<string, IParameter>, INodeGroup> KStrideSquaredUnitZ(int k)
        {
            return d =>
            {
                var arrayStride = (int)d["ArrayStride"].Value;
                return NodeGroup.RandomNodeGroupUnitZ(k * arrayStride * arrayStride, (int)DateTime.Now.Ticks);
            };
        }

        public static Func<IReadOnlyDictionary<string, IParameter>, INodeGroup> KStrideSquaredSphereZ(int k)
        {
            return d =>
            {
                var arrayStride = (int)d["ArrayStride"].Value;
                var arraySize = arrayStride*arrayStride;
                var randy = new Random((int)DateTime.Now.Ticks);
                var sps = Enumerable.Range(0, arrayStride * arrayStride).Select(i => SpherePoint.Random(randy)).ToList();


                return new NodeGroupImpl
                    (
                        nodes: Enumerable.Range(0, arraySize)
                                  .Select(i => Node.Make(sps[i][0], groupIndex: i))
                                  .Concat(
                                      Enumerable.Range(0, arraySize)
                                        .Select(i => Node.Make(sps[i][1], groupIndex: i + arraySize))
                                  )
                                  .Concat(
                                      Enumerable.Range(0, arraySize)
                                        .Select(i => Node.Make(sps[i][2], groupIndex: i + arraySize*2))
                                  ),
                        nodeCount: arraySize*3,
                        generation: 0
                    );
            };
        }

        public static Func<IReadOnlyDictionary<string, IParameter>, INodeGroup> KStrideSquareCliqueUnitZ()
        {
            return d =>
            {
                var arrayStride = (int)d["ArrayStride"].Value;
                var startSeed = (int)d["StartSeed"].Value;
                var memSeed = (int)d["MemSeed"].Value;
                var memCount = (int)d["MemCount"].Value;
                var startMag = (float)d["StartMag"].Value;
                var cnxnMag = (float)d["CnxnMag"].Value;

                var arraySize = arrayStride*arrayStride;

                var startingVals = startSeed.ToRandomAbsoluteDoubles(arraySize, startMag).ToNodes(0).ToList();

                var mems = memSeed.Mems(arraySize, memCount);
                var cnxnVals = mems.ToCnxnMatrix(cnxnMag)
                                    .ToNodes(arraySize);

                var smash = mems.SelectMany(m => m.AsEnumerable()
                                .Select(b=>(float)b))
                                .ToNodes(arraySize + arraySize.ToLowerTriangularArraySize());

                return startingVals
                        .Concat(cnxnVals)
                        .Concat(smash)
                        .ToNodeGroup(arraySize + arraySize.ToLowerTriangularArraySize() + arraySize*memCount, 0);


            };
        }



        public static Func<IReadOnlyDictionary<string, IParameter>, INodeGroup> BasinCheck()
        {
            return d =>
            {
                var arrayStride = (int)d["ArrayStride"].Value;
                var startSeed = (int)d["StartSeed"].Value;
                var startIndex = (int)d["StartIndex"].Value;
                var startDistance = (int)d["StartDistance"].Value;
                var memSeed = (int)d["MemSeed"].Value;
                var memCount = (int)d["MemCount"].Value;
                var cnxnMag = (float)d["CnxnMag"].Value;

                var arraySize = arrayStride * arrayStride;


                var mems = memSeed.Mems(arraySize, memCount);

                var startingVals = startSeed.ToRandomPlusMinus(startDistance).Select(i=> (float)i)
                                                   .Concat(mems[startIndex].Skip(startDistance).Select(i=>(float)i))
                                                   .ToNodes(0)
                                                   .ToList();

                var cnxnVals = mems.ToCnxnMatrix(cnxnMag)
                                    .ToNodes(arraySize);

                var smash = mems.SelectMany(m => m.AsEnumerable()
                                .Select(b => (float)b))
                                .ToNodes(arraySize + arraySize.ToLowerTriangularArraySize());

                return startingVals
                        .Concat(cnxnVals)
                        .Concat(smash)
                        .ToNodeGroup(arraySize + arraySize.ToLowerTriangularArraySize() + arraySize * memCount, 0);


            };
        }

    }

    public static class MemMaker
    {

        public static IReadOnlyList<IReadOnlyList<int>> Mems(this int seed, int arraySize, int count)
        {
            var listRet = new List<IReadOnlyList<int>>();
            var randy = new Random(seed);

            for (var i = 0; i < count; i++)
            {
                listRet.Add(randy.Next().ToRandomPlusMinus(arraySize).ToList());
            }
            return listRet;
        }

        public static IList<float> ToCnxnMatrix(this IReadOnlyList<IReadOnlyList<int>> mems, float cnxnMag)
        {
            return mems.Select(m => m.AutoCorrelate()).SumLists()
                       .Select(k => (k * cnxnMag).ToUnitZ())
                       .ToList();
        }


        public static IList<float> ToCnxnValsOne(this int arrayStride, float cnxnMag)
        {
            var memOne = Enumerable.Range(0, arrayStride * arrayStride / 2).Select(i => 1)
                .Concat(Enumerable.Range(0, arrayStride * arrayStride / 2).Select(i => -1))
                .ToList()
                .AutoCorrelate();

            var memTwo = Enumerable.Range(0, arrayStride / 2 * arrayStride / 2).Select(i => 1)
                .Concat(Enumerable.Range(0, arrayStride / 2 * arrayStride / 2).Select(i => -1))
                .Repeat(2)
                .ToList().AutoCorrelate();


            var memThree = Enumerable.Range(0, arrayStride / 4 * arrayStride / 2).Select(i => 1)
                .Concat(Enumerable.Range(0, arrayStride / 4 * arrayStride / 2).Select(i => -1))
                .Repeat(4)
                .ToList().AutoCorrelate();

            var memFour = Enumerable.Range(0, arrayStride / 4 * arrayStride / 4).Select(i => 1)
                .Concat(Enumerable.Range(0, arrayStride / 4 * arrayStride / 4).Select(i => -1))
                .Repeat(8)
                .ToList().AutoCorrelate();


            var memFive = Enumerable.Repeat(1, arrayStride / 2)
                                    .Concat(Enumerable.Repeat(-1, arrayStride / 2))
                                    .Repeat(arrayStride)
                                    .ToList()
                                    .AutoCorrelate();

            var memSix = Enumerable.Repeat(1, arrayStride / 4)
                                    .Concat(Enumerable.Repeat(-1, arrayStride / 4))
                                    .Concat(Enumerable.Repeat(1, arrayStride / 4))
                                    .Concat(Enumerable.Repeat(-1, arrayStride / 4))
                                    .Repeat(arrayStride)
                                    .ToList()
                                    .AutoCorrelate();

            var memSeven = Enumerable.Repeat(1, arrayStride / 8)
                            .Concat(Enumerable.Repeat(-1, arrayStride / 8))
                            .Concat(Enumerable.Repeat(1, arrayStride / 8))
                            .Concat(Enumerable.Repeat(-1, arrayStride / 8))
                            .Concat(Enumerable.Repeat(1, arrayStride / 8))
                            .Concat(Enumerable.Repeat(-1, arrayStride / 8))
                            .Concat(Enumerable.Repeat(1, arrayStride / 8))
                            .Concat(Enumerable.Repeat(-1, arrayStride / 8))
                        .Repeat(arrayStride)
                        .ToList()
                        .AutoCorrelate();

            var memEight = Enumerable.Repeat(1, arrayStride / 16)
                            .Concat(Enumerable.Repeat(-1, arrayStride / 16))
                            .Concat(Enumerable.Repeat(1, arrayStride / 16))
                            .Concat(Enumerable.Repeat(-1, arrayStride / 16))
                            .Concat(Enumerable.Repeat(1, arrayStride / 16))
                            .Concat(Enumerable.Repeat(-1, arrayStride / 16))
                            .Concat(Enumerable.Repeat(1, arrayStride / 16))
                            .Concat(Enumerable.Repeat(-1, arrayStride / 16))
                            .Concat(Enumerable.Repeat(1, arrayStride / 16))
                            .Concat(Enumerable.Repeat(-1, arrayStride / 16))
                            .Concat(Enumerable.Repeat(1, arrayStride / 16))
                            .Concat(Enumerable.Repeat(-1, arrayStride / 16))
                            .Concat(Enumerable.Repeat(1, arrayStride / 16))
                            .Concat(Enumerable.Repeat(-1, arrayStride / 16))
                            .Concat(Enumerable.Repeat(1, arrayStride / 16))
                            .Concat(Enumerable.Repeat(-1, arrayStride / 16))
                        .Repeat(arrayStride)
                        .ToList()
                        .AutoCorrelate();

            var res = (new[] { memOne, memTwo, memThree, memFour, memFive, memSeven, memEight }).SumLists()
                                            .Select(k => (k * cnxnMag).ToUnitZ())
                                            .ToList();
            return res;
        }

    }
}
