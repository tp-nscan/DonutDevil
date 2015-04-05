using System;
using System.Collections.Generic;
using MathLib.Intervals;

namespace MathLib
{
    public static class SafeRandom
    {
        private static readonly Random random = new Random();
        public static int Next()
        {
            lock (random)
            {
                return random.Next();
            }
        }

        public static double NextDouble()
        {
            lock (random)
            {
                return random.NextDouble();
            }
        }

        public static double NextDoubleInRealInterval(IRealInterval realInterval)
        {
            lock (random)
            {
                return random.NextDouble() * realInterval.Span() + realInterval.Min;
            }
        }
    }

    public static class RandomExt
    {
        public static IEnumerable<double> ToRandomDoubles(this int seed, int count)
        {
            var randy = new Random(seed);
            for (var i = 0; i < count; i++)
            {
                yield return randy.NextDouble();
            }
        }

        public static IEnumerable<double> ToRandomPositiveDoubles(this int seed, int count, double positiveLimit)
        {
            var randy = new Random(seed);
            for (var i = 0; i < count; i++)
            {
                yield return randy.NextDouble() * positiveLimit;
            }
        }

        public static IEnumerable<double> ToRandomAbsoluteDoubles(this int seed, int count, double absoluteLimit)
        {
            var randy = new Random(seed);
            for (var i = 0; i < count; i++)
            {
                yield return 2 * randy.NextDouble() * absoluteLimit - absoluteLimit;
            }
        }

        public static IEnumerable<int> ToRandomPlusMinus(this int seed, int count)
        {
            var randy = new Random(seed);
            for (var i = 0; i < count; i++)
            {
                yield return randy.Next(2)*2 - 1;
            }
        }

    }
}
