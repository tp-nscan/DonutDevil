using System;
using System.Collections.Generic;

namespace MathLib.NumericTypes.ModBits
{
    public static class M
    {
        public static int MoveTowardsM(this int target, int draw, int range, int modulus)
        {
            var diff = (draw - target + modulus) % modulus;


            if (diff < range)
            {
                return diff;
            }

            if (diff > modulus - range)
            {
                return diff - modulus;
            }

            if (diff < range * 2)
            {
                return range * 2 - diff;
            }

            if (diff > modulus - 2 * range)
            {
                return -diff + modulus - 2 * range;
            }
            return 0;
        }

        public static int MoveTowardsOnM8(this int target, int draw)
        {
            var diff = ((draw - target) | 0x100) & 0xFF;

            if (diff < 64)
            {
                return diff;
            }

            if (diff > 192)
            {
                return diff - 256;
            }

            return 128 - diff;
        }

        public static int MoveTowardsOnM10(this int target, int draw)
        {
            var diff = ((draw - target) | 1024) & 1023;

            if (diff < 256)
            {
                return diff;
            }

            if (diff > 768)
            {
                return diff - 1024;
            }

            return 512 - diff;
        }

        public static int MoveTowardsOnM12(this int target, int draw)
        {
            var diff = ((draw - target) | 0x1000) & 0xFFF;

            if (diff < 1024)
            {
                return diff;
            }

            if (diff > 3072)
            {
                return diff - 4096;
            }

            return 2048 - diff;
        }
    }

    public static class M4
    {
        public const int Bits = 4;
        public const int Mod = 0x10;
        public const int Mask = 0xF;
        public const int HalfMod = Mod/2;
        public static double[] DoubleValues = new double[Mod];

        static M4()
        {
            for (double i = 0; i < HalfMod; i++)
            {
                DoubleValues[(int)i] = i / Mod;
            }

            for (double i = HalfMod; i < Mod; i++)
            {
                DoubleValues[(int)i] = -1.0 + (i) / Mod;
            }
        }

        public static int AsM4(this int value)
        {
            return (value + Mod) & Mask;
        }

        public static int AsM4(this double value)
        {
            var puff = value + 100.0;
            var adj = puff - Math.Floor(puff);
            var quant = (int)(Math.Round(adj * Mod)) % Mod;
            return quant;
        }

        public static double M4ToDouble(this int m4)
        {
            return DoubleValues[m4];
        }

        public static IReadOnlyList<double> DblValues
        {
            get { return DoubleValues; }
        }

        public static int BitsPrecision
        {
            get { return Bits; }
        }

        public static int Modulus
        {
            get { return Mod; }
        }
    }

    public static class M6
    {
        public const int Bits = 6;
        public const int Mod = 0x40;
        public const int Mask = 0x3F;
        public const int HalfMod = Mod / 2;
        public static double[] DoubleValues = new double[Mod];

        static M6()
        {
            for (double i = 0; i < HalfMod; i++)
            {
                DoubleValues[(int)i] = i / Mod;
            }

            for (double i = HalfMod; i < Mod; i++)
            {
                DoubleValues[(int)i] = -1.0 + (i) / Mod;
            }
        }

        public static int AsM6(this int value)
        {
            return (value + Mod) & Mask;
        }

        public static int AsM6(this double value)
        {
            var puff = value + 100.0;
            var adj = puff - Math.Floor(puff);
            var quant = (int)(Math.Round(adj * Mod)) % Mod;
            return quant;
        }

        public static double M6ToDouble(this int m4)
        {
            return DoubleValues[m4];
        }

        public static IReadOnlyList<double> DblValues
        {
            get { return DoubleValues; }
        }

        public static int BitsPrecision
        {
            get { return Bits; }
        }

        public static int Modulus
        {
            get { return Mod; }
        }
    }

    public static class M8
    {
        public const int Bits = 8;
        public const int Mod = 0x100;
        public const int Mask = 0xFF;
        public const int HalfMod = Mod/2;

        public static double[] DoubleValues = new double[Mod];

        static M8()
        {
            for (double i = 0; i < HalfMod; i++)
            {
                DoubleValues[(int)i] = i / Mod;
            }

            for (double i = HalfMod; i < Mod; i++)
            {
                DoubleValues[(int)i] = -1.0 + (i) / Mod;
            }
        }

        public static int AsM8(this double value)
        {
            var puff = value + 100.0;
            var adj = puff - Math.Floor(puff);
            var quant = (int)(Math.Round(adj*Mod)) % Mod;
            return quant;
        }

        public static int AsM8(this int value)
        {
            return (value + Mod) & Mask;
        }

        public static double M8ToDouble(this int m4)
        {
            return DoubleValues[m4];
        }

        public static IReadOnlyList<double> DblValues
        {
            get { return DoubleValues; }
        }

        public static int BitsPrecision
        {
            get { return Bits; }
        }

        public static int Modulus
        {
            get { return Mod; }
        }

    }

    public static class M10
    {
        public const int Bits = 10;
        public const int Mod = 1024;
        public const int Mask = 1023;
        public const int HalfMod = Mod / 2;
        public static double[] DoubleValues = new double[Mod];

        static M10()
        {
            for (double i = 0; i < HalfMod; i++)
            {
                DoubleValues[(int)i] = i / Mod;
            }

            for (double i = HalfMod; i < Mod; i++)
            {
                DoubleValues[(int)i] = -1.0 + (i) / Mod;
            }
        }

        public static int AsM10(this double value)
        {
            var puff = value + 100.0;
            var adj = puff - Math.Floor(puff);
            var quant = (int)(Math.Round(adj * Mod)) % Mod;
            return quant;
        }

        public static int AsM10(this int value)
        {
            return (value + Mod) & Mask;
        }

        public static double M10ToDouble(this int m4)
        {
            return DoubleValues[m4];
        }

        public static IReadOnlyList<double> DblValues
        {
            get { return DoubleValues; }
        }

        public static int BitsPrecision
        {
            get { return Bits; }
        }

        public static int Modulus
        {
            get { return Mod; }
        }
    }

    public static class M12
    {
        public const int Bits = 12;
        public const int Mod = 0x1000;
        public const int Mask = 0xFFF;
        public const int HalfMod = Mod / 2;
        public static double[] DoubleValues = new double[Mod];

        static M12()
        {
            for (double i = 0; i < HalfMod; i++)
            {
                DoubleValues[(int)i] = i / Mod;
            }

            for (double i = HalfMod; i < Mod; i++)
            {
                DoubleValues[(int)i] = -1.0 + (i) / Mod;
            }
        }

        public static int AsM12(this double value)
        {
            var puff = value + 100.0;
            var adj = puff - Math.Floor(puff);
            var quant = (int)(Math.Round(adj * Mod)) % Mod;
            return quant;
        }

        public static int AsM12(this int value)
        {
            return (value + Mod) & Mask;
        }

        public static double M12ToDouble(this int m4)
        {
            return DoubleValues[m4];
        }

        public static IReadOnlyList<double> DblValues
        {
            get { return DoubleValues; }
        }

        public static int BitsPrecision
        {
            get { return Bits; }
        }

        public static int Modulus
        {
            get { return Mod; }
        }

    }

}
