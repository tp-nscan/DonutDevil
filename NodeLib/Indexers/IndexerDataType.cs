using System;

namespace NodeLib.Indexers
{
    public enum IndexerDataType
    {
        IntervalR,
        IntervalZ,
        Ring,
        Torus,
        Sphere
    }


    public static class IndexerDataTypeExt
    {
        public static Func<float, float> ValuesToUnitRange(this IndexerDataType indexerDataType)
        {
            switch (indexerDataType)
            {
                case IndexerDataType.IntervalR:
                    return f => f;
                case IndexerDataType.IntervalZ:
                    return f =>  f / 2 + 0.5f;
                case IndexerDataType.Ring:
                    return f => f;
                case IndexerDataType.Torus: // uses this map for each coord.
                    return f => f;;
                case IndexerDataType.Sphere:
                    throw new Exception("Sphere not supported");
                default:
                    throw new Exception("Type not handled");;
            }
        }

        public static Func<float, float> UnitRangeToValues(this IndexerDataType indexerDataType)
        {
            switch (indexerDataType)
            {
                case IndexerDataType.IntervalR:
                    return f => f;
                case IndexerDataType.IntervalZ:
                    return f => f * 2 - 1.0f;
                case IndexerDataType.Ring:
                    return f => f;
                case IndexerDataType.Torus:
                    throw new Exception("Sphere not supported");
                case IndexerDataType.Sphere:
                    throw new Exception("Sphere not supported");
                default:
                    throw new Exception("Type not handled"); ;
            }
        }

    }
}