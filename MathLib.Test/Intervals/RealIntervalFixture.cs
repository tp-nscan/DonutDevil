using System;
using System.Linq;
using MathLib.Intervals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathLib.Test.Intervals
{
    [TestClass]
    public class RealIntervalFixture
    {
        [TestMethod]
        public void SplitToEvenIntervals()
        {
            const int segmentCount = 7;
            var realInterval = RealInterval.Make(-5, 20);

            var pcs = realInterval.SplitToEvenIntervals(segmentCount)
                .ToList();

            Assert.IsTrue(pcs.Count == segmentCount);

            var piecelength = realInterval.Span()/segmentCount;
            Assert.IsTrue(pcs.All(pc => Math.Abs(pc.Span() - piecelength) < Constants.RoundingError));
        }

        [TestMethod]
        public void tryThis()
        {
            var num = 7.123456;
            var formato = "0.00";

            var formy = num.ToString(formato);
        }
    }


}
