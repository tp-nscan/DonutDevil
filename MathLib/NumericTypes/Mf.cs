using System;

namespace MathLib.NumericTypes
{
    public static class Mf
    {
        public const float Epsilon =  0.00001f;
        public const float LookupTablePrecision = 0.001f;

        public static float AsMf(this float lhs)
        {
            if (lhs >= 0)
            {
                if (lhs < 1.0)
                {
                    return lhs;
                }
                return (float)(lhs - Math.Floor(lhs));
            }
            return 1.0f - (-lhs).AsMf();
        }

        public static float AsMf(this double lhs)
        {
            if (lhs >= 0)
            {
                if (lhs < 1.0)
                {
                    return (float) lhs;
                }
                return (float)(lhs - Math.Floor(lhs));
            }
            return 1.0f - (-lhs).AsMf();
        }

        public static bool IsNearlyEqualTo(this float lhs, float rhs)
        {
            var diff = Math.Abs(lhs - rhs);

            if ((diff < Epsilon) || ((diff <= 1.0f ) && (diff > (1.0f - Epsilon))))
            {
                return true;
            }

            return false;
        }

        public static float MfAdd(this float lhs, float rhs)
        {
            return (lhs + rhs).AsMf();
        }

        public static Func<double, double> ShortTanh(double gain)
        {
            return x => 0.5 + Math.Tanh(gain * (x - 0.5)) / 2;
        }

        //returns the directed circular distance from lhs to rhs, positive is clockwise
        public static float MfDelta(this float lhs, float rhs)
        {
            var delta = lhs - rhs;
            if (lhs > rhs)
            {
                return (delta <= 0.5) ? - delta : 1.0f - delta;
            }
            return (delta < - 0.5) ? -1.0f - delta : - delta;
        }

        //returns the directed circular distance, minus saw, from lhs to rhs, positive is clockwise
        public static float MfDeltaSaw(this float lhs, float rhs, float saw)
        {
            var delta = lhs - rhs;

            if (lhs > rhs)
            {
                if (delta < saw)
                {
                    return -delta + saw;
                }
                if(delta < 0.5)
                {
                    return - delta + saw;
                }

                if (delta > 1.0 - saw)
                {
                    return 1.0f - delta - saw;
                }

                return 1.0f - delta - saw;
            }

            if (delta < saw - 1.0f)
            {
                return -delta - 1.0f + saw;
            }

            if (delta < -0.5)
            {
                return -1.0f - delta + saw;
            }

            if (delta < - saw)
            {
                return - delta - saw;
            }

            return - delta - saw;
        }

        //returns the circular distance betweeh lhs and rhs.
        public static float MfAbsDeltaAsFloat(this float lhs, float rhs)
        {
            var delta = lhs - rhs;

            if (delta > 0.5)
            {
                return 1.0f - delta;
            }

            if (delta > 0.0)
            {
                return delta;
            }

            if (delta > - 0.5)
            {
                return - delta;
            }

            return 1 - delta;
        }

        public static float MfSin(this double value)
        {
            return (Math.Sin(value) / 2.0 + 0.5).AsMf();
        }
    }

    public static class Mf2
    {
        /// <summary>
        /// returns [(+/-) x^2/d^2 , (+/-) y^2/d^2, d^2]. The returned values are pure float
        /// </summary>
        public static float[] VDiff(float lhsX, float lhsY, float rhsX, float rhsY)
        {
            var dX = lhsX.MfDelta(rhsX);
            var dY = lhsY.MfDelta(rhsY);

            return new[] { dX * Math.Abs(dX), dY * Math.Abs(dY), dX * dX + dY * dY };
        }

        /// <summary>
        /// returns [(+/-) x^2/d^2 , (+/-) y^2/d^2, d^2]. The returned values are pure float
        /// </summary>
        public static float[] VDiffSaw(float lhsX, float lhsY, float rhsX, float rhsY, float tentMax, float saw)
        {
            var dX = lhsX.MfDelta(rhsX);
            var dY = lhsY.MfDelta(rhsY);

            var mag = dX * dX + dY * dY;

            return new[] { dX * Math.Abs(dX), dY * Math.Abs(dY), mag.Tent(tentMax) - saw };
        }
        /// <summary>
        /// returns [(+/-) x^2/d^2 , (+/-) y^2/d^2, d^2]. The returned values are pure float
        /// </summary>
        public static float[] VDiffTent(float lhsX, float lhsY, float rhsX, float rhsY, float tentMax)
        {
            var dX = lhsX.MfDelta(rhsX);
            var dY = lhsY.MfDelta(rhsY);

            return new[] { dX * Math.Abs(dX), dY * Math.Abs(dY), (dX * dX + dY * dY).Tent(tentMax)};
        }
    }

}
