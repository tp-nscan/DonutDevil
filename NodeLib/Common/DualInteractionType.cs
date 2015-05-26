using System;
using System.Collections.Generic;
using NodeLib.ParamsOld;

namespace NodeLib.Common
{
    public enum DualInteractionType
    {
        None,
        Direct,
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
                        yield return new ParamFloat(0f, 1f, 0.5f, "StepSize" + suffix, true);
                        yield return new ParamEnum(typeof(NeighborhoodType), NeighborhoodType.Perimeter.ToString(), "NeighboorhoodType" + suffix, true);
                        yield return new ParamFloat(0f, 1f, 0.0f, "Noise" + suffix, true);
                        break;
                    case DualInteractionType.Euclidean:
                        yield return new ParamFloat(0f, 1f, 0.5f, "StepSize" + suffix, true);
                        yield return new ParamEnum(typeof(NeighborhoodType), NeighborhoodType.Perimeter.ToString(), "NeighboorhoodType" + suffix, true);
                        yield return new ParamFloat(0f, 1f, 0.0f, "Noise" + suffix, true);
                        break;
                    case DualInteractionType.RotationalBias:
                        yield return new ParamFloat(0f, 1f, 0.5f, "StepSize" + suffix, true);
                        yield return new ParamEnum(typeof(NeighborhoodType), NeighborhoodType.Perimeter.ToString(), "NeighboorhoodType" + suffix, true);
                        yield return new ParamFloat(0f, 1f, 0.5f, "RadialBias" + suffix, true);
                        yield return new ParamFloat(0f, 1f, 0.0f, "Noise" + suffix, true);
                        break;
                    default:
                        throw new Exception("Unhandled DualInteractionType");
                }

                yield break;
            }
            switch (dualInteractionType)
            {
                case DualInteractionType.None:
                    yield return new ParamFloat(0f, 1f, 0.5f, "StepSize_A" + suffix, true);
                    yield return new ParamEnum(typeof(NeighborhoodType), NeighborhoodType.Perimeter.ToString(), "NeighboorhoodType_A" + suffix, true);
                    yield return new ParamFloat(0f, 1f, 0.0f, "Noise_A" + suffix, true);
                    yield return new ParamFloat(0f, 1f, 0.5f, "StepSize_B" + suffix, true);
                    yield return new ParamEnum(typeof(NeighborhoodType), NeighborhoodType.Perimeter.ToString(), "NeighboorhoodType_B" + suffix, true);
                    yield return new ParamFloat(0f, 1f, 0.0f, "Noise_B" + suffix, true);
                    break;
                case DualInteractionType.Euclidean:
                    yield return new ParamFloat(0f, 1f, 0.5f, "StepSize_A" + suffix, true);
                    yield return new ParamEnum(typeof(NeighborhoodType), NeighborhoodType.Perimeter.ToString(), "NeighboorhoodType_A" + suffix, true);
                    yield return new ParamFloat(0f, 1f, 0.0f, "Noise_A" + suffix, true);
                    yield return new ParamFloat(0f, 1f, 0.5f, "StepSize_B" + suffix, true);
                    yield return new ParamEnum(typeof(NeighborhoodType), NeighborhoodType.Perimeter.ToString(), "NeighboorhoodType_B" + suffix, true);
                    yield return new ParamFloat(0f, 1f, 0.0f, "Noise_B" + suffix, true);
                    break;
                case DualInteractionType.RotationalBias:
                    yield return new ParamFloat(0f, 1f, 0.5f, "StepSize_A" + suffix, true);
                    yield return new ParamEnum(typeof(NeighborhoodType), NeighborhoodType.Perimeter.ToString(), "NeighboorhoodType_A" + suffix, true);
                    yield return new ParamFloat(0f, 1f, 0.5f, "RadialBias_A" + suffix, true);
                    yield return new ParamFloat(0f, 1f, 0.0f, "Noise_A" + suffix, true);
                    yield return new ParamFloat(0f, 1f, 0.5f, "StepSize_B" + suffix, true);
                    yield return new ParamEnum(typeof(NeighborhoodType), NeighborhoodType.Perimeter.ToString(), "NeighboorhoodType_B" + suffix, true);
                    yield return new ParamFloat(0f, 1f, 0.5f, "RadialBias_B" + suffix, true);
                    yield return new ParamFloat(0f, 1f, 0.0f, "Noise_B" + suffix, true);
                    break;
                default:
                    throw new Exception("Unhandled DualInteractionType");
            }
        }

    }
}
