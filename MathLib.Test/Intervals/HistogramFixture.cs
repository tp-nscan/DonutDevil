using System;
using System.Linq;
using MathLib.Intervals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathLib.Test.Intervals
{
    [TestClass]
    public class HistogramFixture
    {
        [TestMethod]
        public void Hisotgram()
        {
            const int binCount = 7;
            const int maxInt = 100;
            const int itemCount = 300;
            var randy = new Random(121);
            var items = Enumerable.Range(0, itemCount)
                                    .Select(i => randy.Next(maxInt))
                                    .ToList();

            var evaluator = new Func<int, double>(i => (double) i/100.0);

            var bins = RealInterval.Make(0, 0.9).SplitToEvenIntervals(binCount).ToList();

            var histo = items.SortIntoBins(bins, evaluator);

            Assert.AreEqual(histo.Count, binCount + 1);
            Assert.AreEqual(histo.Sum(b => b.Item2.Count), itemCount);
        }
    }

}
