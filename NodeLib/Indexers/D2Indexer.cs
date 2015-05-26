using System;
using System.Collections.Generic;
using System.Linq;
using LibNode;
using MathLib;
using MathLib.NumericTypes;
using NodeLib.ParamsOld;

namespace NodeLib.Indexers
{
    public interface ID2Indexer
    {
        string Name { get; }
        IndexerDataType IndexerDataType { get; }
        D2ArrayShape D2ArrayShape { get; }
        int Stride { get; }
    }


    public static class D2Indexer
    {
        public static Func<D2IndexerBase<float>, D2IndexerBase<float>, double> AbsCorrelationZFunc(NodeGroup nodeGroup)
        {
            return (m, s) =>
            {
                var masterList = m.IndexingFunc(nodeGroup).Select(n=>n.Value).ToList();
                var slaveList = s.IndexingFunc(nodeGroup).Select(n => n.Value).ToList();

                var correlation = masterList.Zip(slaveList, (nm, ns) => Math.Abs(nm - ns)).Sum();

                return (correlation < masterList.Count) ? correlation : (masterList.Count*2) - correlation;
            };
        }

        public static ID2Indexer MakeLinearArray2D(string name, int squareSize, int offset = 0)
        {
            return new D2IndexerBase<float>(
                name: name,
                indexingFunc:
                n => Enumerable.Range(0, squareSize * squareSize)
                                .Select(i => new D2Val<float>
                                    (
                                        i / squareSize,
                                        i % squareSize,
                                        n.Values[i + offset])
                                    ),
                stride: squareSize,
                indexerDataType: IndexerDataType.IntervalZ, 
                d2ArrayShape: D2ArrayShape.Donut
            );
        }

        public static D2IndexerBase<float> MakeRingArray2D(string name, int squareSize, int offset = 0)
        {
            return new D2IndexerBase<float>(
                name: name,
                indexingFunc:
                n => Enumerable.Range(0, squareSize*squareSize)
                                .Select(i => new D2Val<float>
                                    (
                                        i / squareSize, 
                                        i % squareSize,
                                        n.Values[i + offset])
                                    ),
                stride: squareSize,
                indexerDataType: IndexerDataType.IntervalR,
                d2ArrayShape: D2ArrayShape.Donut
            );
        }

        public static Func<IReadOnlyDictionary<string, IParameter>, IReadOnlyList<ID2Indexer>> LinearArray2DIndexMaker
        {
            get
            {
                return d =>
                {
                    var arrayStride = (int)d["ArrayStride"].Value;
                    return new[]
                    {
                        MakeLinearArray2D("Values", arrayStride)
                    };
                };
            }
        }

        public static Func<IReadOnlyDictionary<string, IParameter>, IReadOnlyList<ID2Indexer>> Clique2DIndexMaker
        {
            get
            {
                return d =>
                {
                    var arrayStride = (int)d["ArrayStride"].Value;
                    var memCount = (int)d["MemCount"].Value;
                    var arraySize = arrayStride*arrayStride;
                    var baseOffset = arraySize.ToLowerTriangularArraySize() + arraySize;
                    var listRet = new List<ID2Indexer>();
                    listRet.Add(MakeLinearArray2D("Values", arrayStride));

                    for (var i = 0; i < memCount; i++)
                    {
                        listRet.Add(MakeLinearArray2D("Mem_" + i, arrayStride, baseOffset + arraySize*i));
                    }


                    return listRet;
                };
            }
        }

        public static Func<IReadOnlyDictionary<string, IParameter>, IReadOnlyList<ID2Indexer>> RingArray2DIndexMaker
        {
            get
            {
                return d =>
                {
                    var arrayStride = (int)d["ArrayStride"].Value;
                    return new[]
                    {
                        MakeRingArray2D("Values", arrayStride)
                    };
                };
            }
        }

        public static Func<IReadOnlyDictionary<string, IParameter>, IReadOnlyList<ID2Indexer>> TorusArray2DIndexMaker
        {
            get
            {
                return d =>
                {
                    var arrayStride = (int) d["ArrayStride"].Value;
                    return new[]
                    {
                        MakeRingArray2D("X values", arrayStride),
                        MakeRingArray2D("Y values", arrayStride, arrayStride*arrayStride)
                    };
                };
            }
        }

        public static Func<IReadOnlyDictionary<string, IParameter>, IReadOnlyList<ID2Indexer>> SphereArray2DIndexMaker
        {
            get
            {
                return d =>
                {
                    var arrayStride = (int)d["ArrayStride"].Value;
                    return new[]
                    {
                        MakeLinearArray2D("X values", arrayStride),
                        MakeLinearArray2D("Y values", arrayStride, arrayStride*arrayStride),
                        MakeLinearArray2D("Z values", arrayStride, 2 * arrayStride*arrayStride)
                    };
                };
            }
        }
    }


    public class D2IndexerBase<T> : ID2Indexer
    {
        private readonly string _name;
        private readonly Func<NodeGroup, IEnumerable<D2Val<T>>> _indexingFunc;
        private readonly int _stride;

        public D2IndexerBase(
            string name,
            Func<NodeGroup, IEnumerable<D2Val<T>>> indexingFunc, 
            int stride,
            IndexerDataType indexerDataType, 
            D2ArrayShape d2ArrayShape
            )
        {
            _name = name;
            _indexingFunc = indexingFunc;
            _stride = stride;
            _indexerDataType = indexerDataType;
            _d2ArrayShape = d2ArrayShape;
        }

        public string Name
        {
            get { return _name; }
        }

        private readonly IndexerDataType _indexerDataType;
        public IndexerDataType IndexerDataType
        {
            get { return _indexerDataType; }
        }

        private readonly D2ArrayShape _d2ArrayShape;
        public D2ArrayShape D2ArrayShape
        {
            get { return _d2ArrayShape; }
        }

        public Func<NodeGroup, IEnumerable<D2Val<T>>> IndexingFunc
        {
            get { return _indexingFunc; }
        }

        public int Stride
        {
            get { return _stride; }
        }
    }
}
