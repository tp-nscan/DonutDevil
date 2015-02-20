using System;
using System.Diagnostics;
using MathLib.NumericTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathLib.Test.NumericTypes
{
    [TestClass]
    public class PointFltFixture
    {
        [TestMethod]
        public void TestMoveTowardsWithBalancedForces()
        {
            var randy = new Random();

            var sw = new Stopwatch();
            sw.Start();

            for (var i = 0; i < 10000000; i++)
            {
                var pt = new PointFlt(
                    (float)randy.NextDouble() * (1 - Mf.Epsilon),
                    (float)randy.NextDouble() * (1 - Mf.Epsilon)
                    );

                var dx = (float)(randy.NextDouble() * (1 - Mf.Epsilon));
                var dy = (float)(randy.NextDouble() * (1 - Mf.Epsilon));

                var ptA = new PointFlt(pt.X.MfAdd(dx), pt.Y.MfAdd(dy));
                var ptB = new PointFlt(pt.X.MfAdd(-dx), pt.Y.MfAdd(-dy));
                var ptC = new PointFlt(pt.X.MfAdd(dx), pt.Y.MfAdd(0.0f));
                var ptD = new PointFlt(pt.X.MfAdd(0.0f), pt.Y.MfAdd(dy));
                var ptE = new PointFlt(pt.X.MfAdd(-dx), pt.Y.MfAdd(0.0f));
                var ptF = new PointFlt(pt.X.MfAdd(0.0f), pt.Y.MfAdd(-dy));

                var ptOut = pt.FieldUpdate(new[] { ptA, ptB, ptC, ptD, ptE, ptF }, 0.20f);


                Assert.IsTrue(pt.X.IsNearlyEqualTo(ptOut.X));
                Assert.IsTrue(pt.Y.IsNearlyEqualTo(ptOut.Y));
            }

            sw.Stop();
            var time = sw.ElapsedMilliseconds;

        }

        [TestMethod]
        public void Ts()
        {
            var pt = new PointFlt(
                    (float)0.0,
                    (float)0.0
                );

            var dx = (float)0.4;
            var dy = (float)0.0;

            var ptA = new PointFlt(pt.X.MfAdd(dx), pt.Y.MfAdd(dy));
            var ptB = new PointFlt(pt.X.MfAdd(-dx), pt.Y.MfAdd(-dy));
            //var ptC = new PointFlt(pt.X.MfAdd(dx), pt.Y.MfAdd(0.0f));
            //var ptD = new PointFlt(pt.X.MfAdd(0.0f), pt.Y.MfAdd(dy));
            //var ptE = new PointFlt(pt.X.MfAdd(-dx), pt.Y.MfAdd(0.0f));
            //var ptF = new PointFlt(pt.X.MfAdd(0.0f), pt.Y.MfAdd(-dy));

            var ptOut = pt.FieldUpdate(new[] { ptA, ptB }, 0.20f);


            Assert.IsTrue(pt.X.IsNearlyEqualTo(ptOut.X));
            Assert.IsTrue(pt.Y.IsNearlyEqualTo(ptOut.Y));

        }
    }

}
