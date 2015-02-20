using System;
using System.Collections.Generic;
using System.Linq;

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

        public static IReadOnlyList<Tuple<IRealInterval, Box<int>>> ToHistogram<T>(
                    this IEnumerable<T> items,
                    IReadOnlyList<IRealInterval> bins,
                    Func<T, double> valuatorFunc)
        {
            var lstRet = bins.Select(b => new Tuple<IRealInterval, Box<int>>(b, new Box<int>(0))).ToList();
            var unMatched = new Tuple<IRealInterval, Box<int>>(RealInterval.Empty, new Box<int>(0));

            foreach (var item in items)
            {
                var wasAdded = false;
                var curVal = valuatorFunc(item);
                foreach (var bin in lstRet)
                {
                    if (bin.Item1.ClosedContains(curVal))
                    {
                        bin.Item2.Item++;
                        wasAdded = true;
                        break;
                    }
                }
                if (!wasAdded)
                {
                    unMatched.Item2.Item++;
                }
            }
            lstRet.Add(unMatched);
            return lstRet;
        }


    }
}
