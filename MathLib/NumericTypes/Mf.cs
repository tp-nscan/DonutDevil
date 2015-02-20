using System;

namespace MathLib.NumericTypes
{
    public static class Mf
    {
        public const float Padder = 1000000.0f;
        public const float Epsilon =  0.00001f;

        public static float AsMf(this float lhs)
        {
            return (lhs + Padder) - (float)Math.Floor(lhs + Padder); ;
        }

        public static bool IsNearlyEqualTo(this float lhs, float rhs)
        {
            var diff = Math.Abs(lhs - rhs);

            if ((diff < Mf.Epsilon) || ((diff <= 1.0f ) && (diff > (1.0f - Epsilon))))
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

        //returns the circular distance betweeh lhs and rhs, positive is clockwise
        public static float MfDeltaAsFloat(this float lhs, float rhs)
        {
            var delta = lhs - rhs;

            if (lhs > rhs)
            {
                return (delta <= 0.5) ? - delta : 1.0f - delta;
            }

            return (delta < - 0.5) ? -1.0f - delta : - delta;
        }
    }

    public static class Mf2
    {
        /// <summary>
        /// returns [(+/-) x^2/d^2 , (+/-) y^2/d^2, d^2]. The returned values are pure float
        /// </summary>
        public static float[] VDiff(float lhsX, float lhsY, float rhsX, float rhsY)
        {
            var dX = lhsX.MfDeltaAsFloat(rhsX);
            var dY = lhsY.MfDeltaAsFloat(rhsY);

            return new[] { dX * Math.Abs(dX), dY * Math.Abs(dY), dX * dX + dY * dY };
        }
    }

}
