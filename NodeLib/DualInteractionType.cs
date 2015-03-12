using System;
using System.Collections.Generic;
using NodeLib.Params;

namespace NodeLib
{
    public enum DualInteractionType
    {
        None,
        Euclidean,
        RotationalBias
    }

    public static class DualInteractionTypeExt
    {
        public static IEnumerable<IParameter> ToParamList(this DualInteractionType dualInteractionType, bool isSymmetric, string suffix = "")
        {
            if (isSymmetric)
            {
                switch (dualInteractionType)
                {
                    case DualInteractionType.None:
                        yield return new ParamFloat(0f, 1f, 0.5f, "StepSize" + suffix);
                        yield return new ParamEnum(typeof(NeighboorhoodType), NeighboorhoodType.Perimeter.ToString(), "NeighboorhoodType" + suffix);
                        yield return new ParamFloat(0f, 1f, 0.0f, "Noise" + suffix);
                        break;
                    case DualInteractionType.Euclidean:
                        yield return new ParamFloat(0f, 1f, 0.5f, "StepSize" + suffix);
                        yield return new ParamEnum(typeof(NeighboorhoodType), NeighboorhoodType.Perimeter.ToString(), "NeighboorhoodType" + suffix);
                        yield return new ParamFloat(0f, 1f, 0.0f, "Noise" + suffix);
                        break;
                    case DualInteractionType.RotationalBias:
                        yield return new ParamFloat(0f, 1f, 0.5f, "StepSize" + suffix);
                        yield return new ParamEnum(typeof(NeighboorhoodType), NeighboorhoodType.Perimeter.ToString(), "NeighboorhoodType" + suffix);
                        yield return new ParamFloat(0f, 1f, 0.5f, "RadialBias" + suffix);
                        yield return new ParamFloat(0f, 1f, 0.0f, "Noise" + suffix);
                        break;
                    default:
                        throw new Exception("Unhandled DualInteractionType");
                }

                yield break;
            }
            switch (dualInteractionType)
            {
                case DualInteractionType.None:
                    yield return new ParamFloat(0f, 1f, 0.5f, "StepSize_A" + suffix);
                    yield return new ParamEnum(typeof(NeighboorhoodType), NeighboorhoodType.Perimeter.ToString(), "NeighboorhoodType_A" + suffix);
                    yield return new ParamFloat(0f, 1f, 0.0f, "Noise_A" + suffix);
                    yield return new ParamFloat(0f, 1f, 0.5f, "StepSize_B" + suffix);
                    yield return new ParamEnum(typeof(NeighboorhoodType), NeighboorhoodType.Perimeter.ToString(), "NeighboorhoodType_B" + suffix);
                    yield return new ParamFloat(0f, 1f, 0.0f, "Noise_B" + suffix);
                    break;
                case DualInteractionType.Euclidean:
                    yield return new ParamFloat(0f, 1f, 0.5f, "StepSize_A" + suffix);
                    yield return new ParamEnum(typeof(NeighboorhoodType), NeighboorhoodType.Perimeter.ToString(), "NeighboorhoodType_A" + suffix);
                    yield return new ParamFloat(0f, 1f, 0.0f, "Noise_A" + suffix);
                    yield return new ParamFloat(0f, 1f, 0.5f, "StepSize_B" + suffix);
                    yield return new ParamEnum(typeof(NeighboorhoodType), NeighboorhoodType.Perimeter.ToString(), "NeighboorhoodType_B" + suffix);
                    yield return new ParamFloat(0f, 1f, 0.0f, "Noise_B" + suffix);
                    break;
                case DualInteractionType.RotationalBias:
                    yield return new ParamFloat(0f, 1f, 0.5f, "StepSize_A" + suffix);
                    yield return new ParamEnum(typeof(NeighboorhoodType), NeighboorhoodType.Perimeter.ToString(), "NeighboorhoodType_A" + suffix);
                    yield return new ParamFloat(0f, 1f, 0.5f, "RadialBias_A" + suffix);
                    yield return new ParamFloat(0f, 1f, 0.0f, "Noise_A" + suffix);
                    yield return new ParamFloat(0f, 1f, 0.5f, "StepSize_B" + suffix);
                    yield return new ParamEnum(typeof(NeighboorhoodType), NeighboorhoodType.Perimeter.ToString(), "NeighboorhoodType_B" + suffix);
                    yield return new ParamFloat(0f, 1f, 0.5f, "RadialBias_B" + suffix);
                    yield return new ParamFloat(0f, 1f, 0.0f, "Noise_B" + suffix);
                    break;
                default:
                    throw new Exception("Unhandled DualInteractionType");
            }
        }

    }
}
