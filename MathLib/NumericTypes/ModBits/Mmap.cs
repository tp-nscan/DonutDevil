using System;
using System.Collections.Generic;
using System.Linq;

namespace MathLib.NumericTypes.ModBits
{
    public static class Mmap
    {
        public static IReadOnlyList<int> M4Approx(this Func<double, double> function)
        {
            return Enumerable.Range(0, M4.Mod).Select(i => function(i.M4ToDouble()).AsM4()).ToArray();
        }

        public static IReadOnlyList<int> M6Approx(this Func<double, double> function)
        {
            return Enumerable.Range(0, M6.Mod).Select(i => function(i.M6ToDouble()).AsM6()).ToArray();
        }

        public static IReadOnlyList<int> M8Approx(this Func<double, double> function)
        {
            return Enumerable.Range(0, M8.Mod).Select(i => function(i.M8ToDouble()).AsM8()).ToArray();
        }

        public static IReadOnlyList<int> M10Approx(this Func<double, double> function)
        {
            return Enumerable.Range(0, M10.Mod).Select(i => function(i.M10ToDouble()).AsM10()).ToArray();
        }

        public static IReadOnlyList<int> M12Approx(this Func<double, double> function)
        {
            return Enumerable.Range(0, M12.Mod).Select(i => function(i.M12ToDouble()).AsM12()).ToArray();
        }
    }
}
