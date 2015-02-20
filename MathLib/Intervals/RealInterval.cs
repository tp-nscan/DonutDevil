using System;
using System.Collections.Generic;

namespace MathLib.Intervals
{
    public interface IRealInterval
    {
        double Max { get; }
        double Min { get; }
    }

    public static class RealInterval
    {
        public static IRealInterval Make(double position, double span)
        {
            return (span > 0)
                ? new RealIntervalImpl(min: position, max: position + span)
                : new RealIntervalImpl(position + span, position);
        }

        public static double Span(this IRealInterval realInterval)
        {
            return realInterval.Max - realInterval.Min;
        }

        public static double Center(this IRealInterval ri)
        {
            return (ri.Min + ri.Max) / 2;
        }

        public static bool OpenContains(this IRealInterval realInterval, double value)
        {
            if(value <= realInterval.Min)
            {
                return false;
            }

            if (value >= realInterval.Max)
            {
                return false;
            }

            return true;
        }

        public static bool ClosedContains(this IRealInterval realInterval, double value)
        {
            if (value < realInterval.Min)
            {
                return false;
            }

            if (value > realInterval.Max)
            {
                return false;
            }

            return true;
        }

        public static bool Intersects(this IRealInterval lhs, IRealInterval rhs)
        {
            var newMin = Math.Max(lhs.Min, rhs.Min);
            var newMax = Math.Min(lhs.Max, rhs.Max);
            return (newMin <= newMax);
        }

        private static readonly IRealInterval _all = Make(double.NegativeInfinity, double.PositiveInfinity);
        public static IRealInterval All
        {
            get { return _all; }
        }

        static readonly IRealInterval empty = Make(double.PositiveInfinity, 0.0);
        public static IRealInterval Empty
        {
            get { return empty; }
        }

        static readonly IRealInterval zeroRange = Make(0, 0);
        public static IRealInterval ZeroRange
        {
            get { return zeroRange; }
        }

        static readonly IRealInterval unitRange = Make(0, 1);
        public static IRealInterval UnitRange
        {
            get { return unitRange; }
        }

        static readonly IRealInterval unitZRange = Make(-1.0, 2.0);
        public static IRealInterval UnitZRange
        {
            get { return unitZRange; }
        }


        static readonly IRealInterval closedZRange = Make(-0.5, 1.0);
        public static IRealInterval ClosedZRange
        {
            get { return closedZRange; }
        }

        static readonly IRealInterval threeLogsRange = Make(1.0, 1000.0);
        public static IRealInterval ThreeLogsRange
        {
            get { return threeLogsRange; }
        }

        static readonly IRealInterval fiveLogsRange = Make(1.0, 100000.0);
        public static IRealInterval FiveLogsRange
        {
            get { return fiveLogsRange; }
        }

        static readonly IRealInterval positiveNumbers = Make(0, Double.PositiveInfinity);
        public static IRealInterval PositiveNumbers
        {
            get { return positiveNumbers; }
        }

        public static double BoundBy(this double dVal, IRealInterval interval)
        {
            if (dVal < interval.Min)
            {
                return interval.Min;
            }

            if (dVal > interval.Max)
            {
                return interval.Max;
            }
            return dVal;
        }

        public static IEnumerable<double> RandomSample(this IRealInterval interval, int seed)
        {
            var randy = new Random(seed);

            while (true)
            {
                yield return randy.NextDouble() * interval.Span() + interval.Min;
            }
        }
    }


    class RealIntervalImpl : IRealInterval
    {
        public RealIntervalImpl(double min, double max)
        {
            _min = min;
            _max = max;
        }

        private readonly double _max;
        public double Max
        {
            get { return _max; }
        }

        private readonly double _min;
        public double Min
        {
            get { return _min; }
        }
    }

}
