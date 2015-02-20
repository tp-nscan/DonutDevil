using System;

namespace MathLib.NumericTypes.ModBits
{
    public static class M8By8
    {
        public const int Mod = 0x10000;
        public const int Mask = 0xFFFF;
        public const int Bits = 8;

        struct _M8By8
        {
            private readonly int _valueX;
            private readonly int _valueY;

            public _M8By8(int valueX, int valueY)
                : this()
            {
                _valueX = valueX.AsM8();
                _valueY = valueY.AsM8();
            }


            public _M8By8(int index)
                : this()
            {
                _valueY = (index & M8.Mask).AsM8();
                _valueX = (index >> Bits).AsM8();
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


        static M8By8()
        {
            forceField = new float[Mod][];
            MakeForceField();
        }

        static void MakeForceField()
        {
            forceField[0] = new[] { 0.0f, 0.0f, 0.0f };

            for (var i = 1; i < Mod; i++)
            {
                var typed = new _M8By8(i);
                var distance = Math.Sqrt(typed.ValueX.M8ToDouble() * typed.ValueX.M8ToDouble() + typed.ValueY.M8ToDouble() * typed.ValueY.M8ToDouble());
                forceField[i] = new[] { (float)distance, (float)(typed.ValueX.M8ToDouble() / distance), (float)(typed.ValueY.M8ToDouble() / distance) };
            }
        }

        private static readonly float[][] forceField;


        public static int[] ToCornersOnM8By8(this int index)
        {
            var typed = new _M8By8(index);
            var upLeft = new _M8By8((typed.ValueX - 1).AsM8(), (typed.ValueY - 1).AsM8());
            var upRight = new _M8By8((typed.ValueX + 1).AsM8(), (typed.ValueY - 1).AsM8());
            var lowLeft = new _M8By8((typed.ValueX - 1).AsM8(), (typed.ValueY + 1).AsM8());
            var lowRight = new _M8By8((typed.ValueX + 1).AsM8(), (typed.ValueY + 1).AsM8());
            return new[] { upLeft.Index, upRight.Index, lowLeft.Index, lowRight.Index };
        }

        public static int[] ToSidesOnM8By8(this int index)
        {
            var typed = new _M8By8(index);
            var top = new _M8By8(typed.ValueX, typed.ValueY - 1);
            var right = new _M8By8(typed.ValueX + 1, typed.ValueY);
            var bottom = new _M8By8(typed.ValueX, typed.ValueY + 1);
            var left = new _M8By8(typed.ValueX - 1, typed.ValueY);
            return new[] { top.Index, right.Index, bottom.Index, left.Index };
        }


        public static int[] ToPerimeterOnM8By8(this int index)
        {
            var typed = new _M8By8(index);
            var top = new _M8By8(typed.ValueX, typed.ValueY - 1);
            var right = new _M8By8(typed.ValueX + 1, typed.ValueY);
            var bottom = new _M8By8(typed.ValueX, typed.ValueY + 1);
            var left = new _M8By8(typed.ValueX - 1, typed.ValueY);
            var upLeft = new _M8By8((typed.ValueX - 1).AsM8(), (typed.ValueY - 1).AsM8());
            var upRight = new _M8By8((typed.ValueX + 1).AsM8(), (typed.ValueY - 1).AsM8());
            var lowLeft = new _M8By8((typed.ValueX - 1).AsM8(), (typed.ValueY + 1).AsM8());
            var lowRight = new _M8By8((typed.ValueX + 1).AsM8(), (typed.ValueY + 1).AsM8());

            return new[] { top.Index, right.Index, bottom.Index, left.Index, upLeft.Index, upRight.Index, lowLeft.Index, lowRight.Index };
        }

        public static float[] ToForceFieldsOnM8By8(this int index)
        {
            return forceField[index];
        }

        public static int M8By8FloatOffset(this int lhs, double xOff, double yOff)
        {
            var typed = new _M8By8(lhs);

            var valueX = (typed.ValueX.M8ToDouble() + xOff).AsM8();
            var valueY = (typed.ValueY.M8ToDouble() + yOff).AsM8();

            return (valueX << Bits) + valueY;
        }

        public static int DeltaOnM8ByM8ToIndex(this int lhs, int rhs)
        {
            var lhsX = (lhs >> 8) & M8.Mask;
            var lhsY = lhs & M8.Mask;

            var rhsX = (rhs >> 8) & M8.Mask;
            var rhsY = rhs & M8.Mask;

            var newX = (lhsX - rhsX + M8.Mod) % M8.Mod;
            var newY = (lhsY - rhsY + M8.Mod) % M8.Mod;


            var res = (newX << 8) + newY;

            return res;
        }

        public static int[] MoveTowardsOnM8ByM8(this int lhs, int rhs)
        {
            var lhsX = (lhs >> 8) & M8.Mask;
            var lhsY = lhs & M8.Mask;

            var rhsX = (rhs >> 8) & M8.Mask;
            var rhsY = rhs & M8.Mask;

            return new[]
            {
                lhsX.MoveTowardsOnM8(rhsX),
                lhsY.MoveTowardsOnM8(rhsY)
            };
        }

    }

}
