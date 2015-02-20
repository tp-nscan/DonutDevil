using System;
using System.Collections.Generic;
using System.Linq;

namespace MathLib.Intervals
{
    public static class RealIntervalExt
    {
        public static IEnumerable<IRealInterval> SplitToEvenIntervals(this IRealInterval realInterval, int segmentCount)
        {
            for (var i = 0; i < segmentCount; i++)
            {
                yield return RealInterval.Make
                    (
                        realInterval.TicAtIndex(i, segmentCount),
                        realInterval.Span() / segmentCount
                    );
            }
        }

        public static double Mid(this IRealInterval realInterval)
        {
            return realInterval.Min + realInterval.Span()/2;
        }

        public static IRealInterval Pad(this IRealInterval realInterval, double leftSide, double rightSide)
        {
            return RealInterval.Make(realInterval.Min - leftSide, realInterval.Span() + leftSide + rightSide);
        }

        public static IEnumerable<double> TicMarks(this IRealInterval realInterval, int segmentCount)
        {
            for (var i = 0; i < segmentCount + 1; i++)
            {
                yield return realInterval.TicAtIndex(i, segmentCount);
            }
        }

        public static IRealInterval Match(this IEnumerable<IRealInterval> buckets, double value)
        {
            foreach (var bucket in buckets)
            {
                if (bucket.ClosedContains(value))
                {
                    return bucket;
                }
            }
            return RealInterval.Empty;
        }


        public static double TicAtIndex(this IRealInterval realInterval, int tic, int segmentCount)
        {
            return realInterval.Min + (realInterval.Span() * tic) / (segmentCount);
        }

        public static float TicAtIndex(float firstValue, float lastValue, int tic, int segmentCount)
        {
            return firstValue + ((lastValue - firstValue) * tic) / (segmentCount);
        }

        public static IRealInterval Union(this IEnumerable<IRealInterval> dRs)
        {
            if (dRs == null)
            {
                return RealInterval.Empty;
            }

            var list = dRs.ToList();
            var minVal = list.Min(T => T.Min);
            return RealInterval.Make(position: minVal, span: list.Max(T => T.Max) - minVal);
        }

        public static string BracketFormat(this IRealInterval realInterval, string numberFormat)
        {
            return String.Format("[{0}-{1}]", realInterval.Min.ToString(numberFormat),
                                 realInterval.Max.ToString(numberFormat));
        }

        /// <summary>
        /// Zooms out for factor > 1, zooms in for factor < 1
        /// </summary>
        public static IRealInterval ZoomBy(this IRealInterval interval, double factor)
        {
            factor = Math.Abs(factor);
            return RealInterval.Make
                (
                    interval.Center() - interval.Span() * factor / 2,
                    interval.Center() + interval.Span() * factor / 2
               );
        }

        public static IRealInterval AdjustBy(this IRealInterval interval, double minDelta, double maxDelta)
        {
            return RealInterval.Make
                (
                    interval.Min + minDelta,
                    interval.Max + maxDelta
               );
        }

        public static IRealInterval Offset(this IRealInterval realInterval, double delta)
        {
            return RealInterval.Make(realInterval.Min + delta, realInterval.Max + delta);
        }
    }
}
