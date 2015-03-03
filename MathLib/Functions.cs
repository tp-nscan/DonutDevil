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
}
