using System;
using System.Collections;
using System.Collections.Generic;

namespace MathLib
{
    public static class Functions
    {

        public static double SawTooth(double period)
        {
            return 1.0;
        }

        public static float Tent(this float lhs, float max)
        {
            if (lhs < max)
            {
                return lhs;
            }
            if (lhs < 2*max)
            {
                return 2 * max - lhs;
            }
            return 0.0f;
        }


    }

    public static class Circular
    {
        static Circular()
        {
            
        }

        private const int Steps = 1000;
        static float[,] _sines = new float[Steps, 8];

    }

    public enum Perimeter
    {
        UC=0,
        UR=1,
        CR=2,
        LR=3,
        LC=4,
        LF=5,
        CF=6,
        UF=7
    }

    public static class PerimeterExt
    {
        public static float ToUnitOffset(this Perimeter perimeter, float value)
        {
            switch (perimeter)
            {
                case Perimeter.UC:
                    return value;
                case Perimeter.UR:
                    return (value < 0.125f) ? .0875f + value : value - 0.125f;
                case Perimeter.CR:
                    return (value < 0.25f) ? 0.75f + value : value - 0.25f;
                case Perimeter.LR:
                    return (value < 0.375f) ? 0.625f + value : value - 0.375f;
                case Perimeter.LC:
                    return (value < 0.5f) ? 0.5f + value : value - 0.5f;
                case Perimeter.LF:
                    return (value < 0.625f) ? .375f + value : value - 0.625f;
                case Perimeter.CF:
                    return (value < 0.75f) ? 0.25f + value : value - 0.75f;
                case Perimeter.UF:
                    return (value < 0.875f) ? 0.125f + value : value - 0.875f;
                default:
                    throw new Exception("Perimeter value not handled in ToUnitOffset");
            }
        }


        public static float UnitToSin(this float value)
        {
            return (float) Math.Sin(Math.PI*2.0*value);
        }

        public static float UnitToCos(this float value)
        {
            return (float)Math.Sin(Math.PI * 2.0 * value);
        }


    }
}
