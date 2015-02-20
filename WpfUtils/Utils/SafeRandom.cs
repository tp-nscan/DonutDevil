using System;
using MathLib.Intervals;

namespace WpfUtils.Utils
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
}
