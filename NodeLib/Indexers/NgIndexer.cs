using System;
using System.Collections.Generic;
using System.Linq;
using MathLib;
using MathLib.NumericTypes;
using NodeLib.Params;

namespace NodeLib.Indexers
{
    public interface INgIndexer
    {
        string Name { get; }
        NgIndexerType NgIndexerType { get; }
        Func<INodeGroup, IEnumerable<D2Val<float>>> IndexingFunc { get; }
        Func<float, float> ValuesToUnitRange { get; }
        Func<float, float> UnitRangeToValues { get; } 
        int Height { get; }
        int Width { get; }
    }

    public enum NgIndexerType
    {
        LinearArray2D,
        RingArray2D,
        FullClique
    }

    public static class NgIndexer
    {
        public static Func<INgIndexer, INgIndexer, double> AbsCorrelationZFunc(INodeGroup nodeGroup)
        {

            return (m, s) =>
            {
                var masterList = m.IndexingFunc(nodeGroup).Select(n=>n.Value).ToList();
                var slaveList = s.IndexingFunc(nodeGroup).Select(n => n.Value).ToList();

                var correlation = masterList.Zip(slaveList, (nm, ns) => Math.Abs(nm - ns)).Sum();

                return (correlation < masterList.Count) ? correlation : (masterList.Count*2) - correlation;
            };
        }

        public static INgIndexer MakeLinearArray2D(string name, int squareSize, int offset = 0)
        {
            return new NgIndexerImpl(
                name: name,
                ngIndexerType: NgIndexerType.LinearArray2D, 
                indexingFunc:
                n => Enumerable.Range(0, squareSize * squareSize)
                                .Select(i => new D2Val<float>
                                    (
                                        i / squareSize,
                                        i % squareSize,
                                        n.Values[i + offset])
                                    ),
                height: squareSize,
                width: squareSize,
                valuesToUnitRange: f => f/2 + 0.5f,
                unitRangeToValues: f => f*2 - 1.0f
            );
        }

        public static INgIndexer MakeRingArray2D(string name, int squareSize, int offset=0)
        {
            return new NgIndexerImpl(
                name: name,
                ngIndexerType: NgIndexerType.RingArray2D, 
                indexingFunc:
                n => Enumerable.Range(0, squareSize*squareSize)
                                .Select(i => new D2Val<float>
                                    (
                                        i / squareSize, 
                                        i % squareSize,
                                        n.Values[i + offset])
                                    ),
                height: squareSize,
                width: squareSize,
                valuesToUnitRange: f=>f,
                unitRangeToValues: f=>f
            );
        }

        public static Func<IReadOnlyDictionary<string, IParameter>, IReadOnlyList<INgIndexer>> LinearArray2DIndexMaker
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

        public static Func<IReadOnlyDictionary<string, IParameter>, IReadOnlyList<INgIndexer>> Clique2DIndexMaker
        {
            get
            {
                return d =>
                {
                    var arrayStride = (int)d["ArrayStride"].Value;
                    var memCount = (int)d["MemCount"].Value;
                    var arraySize = arrayStride*arrayStride;
                    var baseOffset = arraySize.ToLowerTriangularArraySize() + arraySize;
                    var listRet = new List<INgIndexer>();
                    listRet.Add(MakeLinearArray2D("Values", arrayStride));

                    for (var i = 0; i < memCount; i++)
                    {
                        listRet.Add(MakeLinearArray2D("Mem_" + i, arrayStride, baseOffset + arraySize*i));
                    }


                    return listRet;
                };
            }
        }

        public static Func<IReadOnlyDictionary<string, IParameter>, IReadOnlyList<INgIndexer>> RingArray2DIndexMaker
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

        public static Func<IReadOnlyDictionary<string, IParameter>, IReadOnlyList<INgIndexer>> TorusArray2DIndexMaker
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

        public static Func<IReadOnlyDictionary<string, IParameter>, IReadOnlyList<INgIndexer>> SphereArray2DIndexMaker
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

    public class NgIndexerImpl : INgIndexer
    {
        private readonly string _name;
        private readonly Func<INodeGroup, IEnumerable<D2Val<float>>> _indexingFunc;
        private readonly int _height;
        private readonly int _width;
        private readonly Func<float, float> _valuesToUnitRange;
        private readonly Func<float, float> _unitRangeToValues;
        private readonly NgIndexerType _ngIndexerType;
        public NgIndexerImpl(
            string name,
            NgIndexerType ngIndexerType, 
            Func<INodeGroup, IEnumerable<D2Val<float>>> indexingFunc, 
            int height,
            int width, 
            Func<float, float> valuesToUnitRange, 
            Func<float, float> unitRangeToValues)
        {
            _name = name;
            _indexingFunc = indexingFunc;
            _height = height;
            _width = width;
            _valuesToUnitRange = valuesToUnitRange;
            _unitRangeToValues = unitRangeToValues;
            _ngIndexerType = ngIndexerType;
        }

        public string Name
        {
            get { return _name; }
        }

        public NgIndexerType NgIndexerType
        {
            get { return _ngIndexerType; }
        }

        public Func<INodeGroup, IEnumerable<D2Val<float>>> IndexingFunc
        {
            get { return _indexingFunc; }
        }

        public Func<float, float> ValuesToUnitRange
        {
            get { return _valuesToUnitRange; }
        }

        public Func<float, float> UnitRangeToValues
        {
            get { return _unitRangeToValues; }
        }

        public int Height
        {
            get { return _height; }
        }

        public int Width
        {
            get { return _width; }
        }
    }
}
