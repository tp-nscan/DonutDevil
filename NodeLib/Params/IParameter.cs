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
        bool CanChangeAtRunTime { get; }
    }

    public static class ParamSets
    {
        public static IReadOnlyDictionary<string, IParameter> StandardRingParams()
        {
            return
                 new IParameter[]
                {
                    new ParamInt(4, 1024, 256, "ArrayStride", false),
                    new ParamEnum(typeof (NeighborhoodType), NeighborhoodType.Perimeter.ToString(), "NeighborhoodType"),
                    new ParamFloat(0.0f, 0.4f, 0.1f, "StepSize"),
                    new ParamFloat(0.0f, 0.4f, 0.1f, "Noise")
                }.ToDictionary(v => v.Name);
        }

        public static IReadOnlyDictionary<string, IParameter> DoubleRingParams()
        {
            return
                 new IParameter[]
                {
                    new ParamInt(4, 1024, 256, "ArrayStride", false),
                    new ParamEnum(typeof (DualInteractionType), DualInteractionType.None.ToString(), "DualInteractionType"),
                    new ParamEnum(typeof (NeighborhoodType), NeighborhoodType.Perimeter.ToString(), "NeighborhoodType"),
                    new ParamFloat(0.0f, 0.4f, 0.1f, "StepSize"),
                    new ParamFloat(0.0f, 0.4f, 0.1f, "Noise")
                }.ToDictionary(v => v.Name);
        }

    }
}
