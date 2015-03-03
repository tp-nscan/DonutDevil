using System;

namespace MathLib
{
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

    public static class Circular
    {
        static Circular()
        {
            for (var i = 0; i < Steps; i++)
            {
                var step = ((float) i)/Steps;
                sines[i, 0] = Perimeter.UC.ToUnitOffset(step)._UnitToSin();
                sines[i, 1] = Perimeter.UR.ToUnitOffset(step)._UnitToSin();
                sines[i, 2] = Perimeter.CR.ToUnitOffset(step)._UnitToSin();
                sines[i, 3] = Perimeter.LR.ToUnitOffset(step)._UnitToSin();
                sines[i, 4] = Perimeter.LC.ToUnitOffset(step)._UnitToSin();
                sines[i, 5] = Perimeter.LF.ToUnitOffset(step)._UnitToSin();
                sines[i, 6] = Perimeter.CF.ToUnitOffset(step)._UnitToSin();
                sines[i, 7] = Perimeter.UF.ToUnitOffset(step)._UnitToSin();

                cosines[i, 0] = Perimeter.UC.ToUnitOffset(step)._UnitToCos();
                cosines[i, 1] = Perimeter.UR.ToUnitOffset(step)._UnitToCos();
                cosines[i, 2] = Perimeter.CR.ToUnitOffset(step)._UnitToCos();
                cosines[i, 3] = Perimeter.LR.ToUnitOffset(step)._UnitToCos();
                cosines[i, 4] = Perimeter.LC.ToUnitOffset(step)._UnitToCos();
                cosines[i, 5] = Perimeter.LF.ToUnitOffset(step)._UnitToCos();
                cosines[i, 6] = Perimeter.CF.ToUnitOffset(step)._UnitToCos();
                cosines[i, 7] = Perimeter.UF.ToUnitOffset(step)._UnitToCos();
            }
        }

        private const int Steps = 10000;
        static readonly float[,] sines = new float[Steps, 8];
        static readonly float[,] cosines = new float[Steps, 8];


        static float _UnitToSin(this float value)
        {
            return (float)Math.Sin(Math.PI * 2.0 * value);
        }

        static float _UnitToCos(this float value)
        {
            return (float)Math.Cos(Math.PI * 2.0 * value);
        }

        public static float UnitToSin(this float value)
        {
            return sines[(int)(value * Steps), 0];
        }

        public static float UnitToCos(this float value)
        {
            return cosines[(int)(value * Steps), 0];
        }

        public static float UnitToSin(this float value, Perimeter perimeter)
        {
            return sines[(int)(value * Steps), (int) perimeter];
        }

        public static float UnitToCos(this float value, Perimeter perimeter)
        {
            return cosines[(int)(value * Steps), (int)perimeter];
        }

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

    }
}