using System;
using System.Collections.Generic;
using System.Linq;

namespace NodeLib.Params
{
    public enum ParamType
    {
        Bool,
        Int,
        Float,
        Enum
    }

    public interface IParameter
    {
        ParamType ParamType { get; }
        Type Type { get; }
        string Name { get; }
        object Value { get; }
    }

    public static class ParamSets
    {
        public static IReadOnlyDictionary<string, IParameter> StandardRingParams()
        {
            return
                 new IParameter[]
                {
                    new ParamEnum(typeof (NeighborhoodType), NeighborhoodType.Perimeter.ToString(), "NeighborhoodType"),
                    new ParamFloat(0.0f, 1.0f, 0.5f, "StepSize"),
                    new ParamFloat(0.0f, 1.0f, 0.0f, "Noise")
                }.ToDictionary(v => v.Name);
        }

        public static IReadOnlyDictionary<string, IParameter> DoubleRingParams()
        {
            return
                 new IParameter[]
                {
                    new ParamEnum(typeof (NeighborhoodType), NeighborhoodType.Perimeter.ToString(), "NeighborhoodType"),
                    new ParamFloat(0.0f, 1.0f, 0.5f, "StepSize"),
                    new ParamFloat(0.0f, 1.0f, 0.0f, "Noise")
                }.ToDictionary(v => v.Name);
        }

    }
}
