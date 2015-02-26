using System;

namespace MathLib
{
    public static class Constants
    {
        static Constants()
        {
            InvSqurtOf2 = (float) (1.0/Math.Sqrt(2.0));
        }


        public const double RoundingError = 0.000000000001;

        public static float InvSqurtOf2;
    }
}
