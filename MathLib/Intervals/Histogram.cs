using System;
using System.Collections.Generic;
using System.Linq;
using MathLib.NumericTypes;

namespace MathLib.Intervals
{
    public static class Histogram
    {
        public static IReadOnlyList<Tuple<IRealInterval, List<T>>> SortIntoBins<T>(
                this IEnumerable<T> items, 
                IReadOnlyList<IRealInterval> bins,
                Func<T, double> valuatorFunc
            )
        {
            var lstRet = bins.Select(b => new Tuple<IRealInterval, List<T>>(b, new List<T>())).ToList();
            var unMatched = new Tuple<IRealInterval, List<T>>(RealInterval.Empty, new List<T>());

            foreach (var item in items)
            {
                var wasAdded = false;
                var curVal = valuatorFunc(item);
                foreach (var bin in lstRet)
                {
                    if(bin.Item1.ClosedContains(curVal))
                    {
                        bin.Item2.Add(item);
                        wasAdded = true;
                        break;
                    }
                }
                if (!wasAdded)
                {
                    unMatched.Item2.Add(item);
                }
            }
            lstRet.Add(unMatched);
            return lstRet;
        }

        public static int[] ToHistogram(this IEnumerable<float> floats, int resolution, float max)
        {
            var scale = (resolution - 0.0005f) / max;
            var retInts = new int[resolution];

            foreach (var flt in floats)
            {
                retInts[(int)(flt * scale)]++;
            }

            return retInts;
        }

        public static float[] ToScaledHistogram(this IEnumerable<float> floats, int resolution, float max)
        {
            var histo = floats.ToHistogram(resolution, max);
            var maxInv = 1.0f / histo.Max();
            return histo.Select(v=> maxInv*v).ToArray();
        }

        public static int[,] ToHistogram(this IEnumerable<PointFlt> pointFlts, int resolution, float maxX, float maxY)
        {
            var retInts = new int[resolution, resolution];
            var scaleX = (resolution - 0.0005f) / maxX;
            var scaleY = (resolution - 0.0005f) / maxY;

            foreach (var pointFlt in pointFlts)
            {
                retInts[(int)(pointFlt.X * scaleX), (int)(pointFlt.Y * scaleY)]++;
            }

            return retInts;
        }

        public static float[,] ToScaledHistogram(this IEnumerable<PointFlt> pointFlts, int resolution, float maxX, float maxY)
        {
            var histo = pointFlts.ToHistogram(resolution, maxX, maxY);
            var maxInv = 1.0f / histo.Max();

            var floats = new float[resolution, resolution];

            for (var i = 0; i < floats.GetUpperBound(0); i++)
            {
                for (var j = 0; j < floats.GetUpperBound(1); j++)
                {
                    floats[i, j] = maxInv * histo[i, j];
                }
            }

            return floats;
        }
    }
}
