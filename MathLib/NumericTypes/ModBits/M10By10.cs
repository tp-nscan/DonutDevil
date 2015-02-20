using System;

namespace MathLib.NumericTypes.ModBits
{
    public static class M10By10
    {
        public const int Mod = 0x100000;
        public const int Mask = 0xFFFFF;
        public const int Bits = 10;

        struct _M10By10
        {
            private readonly int _valueX;
            private readonly int _valueY;

            public _M10By10(int valueX, int valueY)
                : this()
            {
                _valueX = valueX.AsM10();
                _valueY = valueY.AsM10();
            }

            public _M10By10(int index)
                : this()
            {
                _valueY = (index & M10.Mask).AsM10();
                _valueX = (index >> Bits).AsM10();
            }

            public int ValueX
            {
                get { return _valueX; }
            }

            public int ValueY
            {
                get { return _valueY; }
            }

            public int Index
            {
                get { return (ValueX << Bits) + ValueY; }
            }
        }


        static M10By10()
        {
            forceField = new float[Mod][];
            MakeForceField();
        }

        static void MakeForceField()
        {
            forceField[0] = new[] { 0.0f, 0.0f, 0.0f };

            for (var i = 1; i < Mod; i++)
            {
                var typed = new _M10By10(i);
                var distance = Math.Sqrt(typed.ValueX.M10ToDouble() * typed.ValueX.M10ToDouble() + typed.ValueY.M10ToDouble() * typed.ValueY.M10ToDouble());
                forceField[i] = new[] { (float)distance, (float)(typed.ValueX.M10ToDouble() / distance), (float)(typed.ValueY.M10ToDouble() / distance) };
            }
        }

        private static readonly float[][] forceField;


        public static int[] ToCornersOnM10By10(this int index)
        {
            var typed = new _M10By10(index);
            var upLeft = new _M10By10((typed.ValueX - 1).AsM10(), (typed.ValueY - 1).AsM10());
            var upRight = new _M10By10((typed.ValueX + 1).AsM10(), (typed.ValueY - 1).AsM10());
            var lowLeft = new _M10By10((typed.ValueX - 1).AsM10(), (typed.ValueY + 1).AsM10());
            var lowRight = new _M10By10((typed.ValueX + 1).AsM10(), (typed.ValueY + 1).AsM10());
            return new[] { upLeft.Index, upRight.Index, lowLeft.Index, lowRight.Index };
        }

        public static int[] ToSidesOnM10By10(this int index)
        {
            var typed = new _M10By10(index);
            var top = new _M10By10(typed.ValueX, typed.ValueY - 1);
            var right = new _M10By10(typed.ValueX + 1, typed.ValueY);
            var bottom = new _M10By10(typed.ValueX, typed.ValueY + 1);
            var left = new _M10By10(typed.ValueX - 1, typed.ValueY);
            return new[] { top.Index, right.Index, bottom.Index, left.Index };
        }


        public static int[] ToPerimeterOnM10By10(this int index)
        {
            var typed = new _M10By10(index);
            var top = new _M10By10(typed.ValueX, typed.ValueY - 1);
            var right = new _M10By10(typed.ValueX + 1, typed.ValueY);
            var bottom = new _M10By10(typed.ValueX, typed.ValueY + 1);
            var left = new _M10By10(typed.ValueX - 1, typed.ValueY);
            var upLeft = new _M10By10((typed.ValueX - 1).AsM10(), (typed.ValueY - 1).AsM10());
            var upRight = new _M10By10((typed.ValueX + 1).AsM10(), (typed.ValueY - 1).AsM10());
            var lowLeft = new _M10By10((typed.ValueX - 1).AsM10(), (typed.ValueY + 1).AsM10());
            var lowRight = new _M10By10((typed.ValueX + 1).AsM10(), (typed.ValueY + 1).AsM10());

            return new[] { top.Index, right.Index, bottom.Index, left.Index, upLeft.Index, upRight.Index, lowLeft.Index, lowRight.Index };
        }

        public static float[] ToForceFieldsOnM10By10(this int index)
        {
            return forceField[index];
        }

        public static int M10By10FloatOffset(this int lhs, double xOff, double yOff)
        {
            var typed = new _M10By10(lhs);

            var valueX = (typed.ValueX.M10ToDouble() + xOff).AsM10();
            var valueY = (typed.ValueY.M10ToDouble() + yOff).AsM10();

            return (valueX << Bits) + valueY;
        }

        public static int DeltaOnM10ByM10ToIndex(this int lhs, int rhs)
        {
            var lhsX = (lhs >> 10) & M10.Mask;
            var lhsY = lhs & M10.Mask;

            var rhsX = (rhs >> 10) & M10.Mask;
            var rhsY = rhs & M10.Mask;

            var newX = (lhsX - rhsX + M10.Mod) % M10.Mod;
            var newY = (lhsY - rhsY + M10.Mod) % M10.Mod;


            var res = (newX << 10) + newY;

            return res;
        }

    }
}
