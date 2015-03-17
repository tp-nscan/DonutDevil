using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MathLib.NumericTypes
{
    public static class NumericExt
    {
        public static double FractionOf(this int numerator, int denominator)
        {
            return ((double)numerator)/denominator;
        }

        public static IEnumerable<double> ToUnitInterp(this int stepCount)
        {
            return Enumerable.Range(0, stepCount).Select(i => ((double) i)/stepCount);
        }
    }
}
