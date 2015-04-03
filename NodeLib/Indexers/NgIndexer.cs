using System;
using System.Collections.Generic;
using System.Linq;
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
        FloatArray2D,
        FullClique
    }

    public static class NgIndexer
    {

        public static INgIndexer MakeLinearArray2D(string name, int squareSize, int offset = 0)
        {
            return new NgIndexerImpl(
                name: name,
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
    }

    public class NgIndexerImpl : INgIndexer
    {
        private readonly string _name;
        private readonly Func<INodeGroup, IEnumerable<D2Val<float>>> _indexingFunc;
        private readonly int _height;
        private readonly int _width;
        private readonly Func<float, float> _valuesToUnitRange;
        private readonly Func<float, float> _unitRangeToValues;

        public NgIndexerImpl(
            string name, 
            Func<INodeGroup, IEnumerable<D2Val<float>>> indexingFunc, 
            int height,
            int width, Func<float, float> valuesToUnitRange, Func<float, float> unitRangeToValues)
        {
            _name = name;
            _indexingFunc = indexingFunc;
            _height = height;
            _width = width;
            _valuesToUnitRange = valuesToUnitRange;
            _unitRangeToValues = unitRangeToValues;
        }

        public string Name
        {
            get { return _name; }
        }

        public NgIndexerType NgIndexerType
        {
            get { return NgIndexerType.FloatArray2D; }
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
