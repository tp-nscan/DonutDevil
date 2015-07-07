using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathLib.Test.NumericTypes
{
    [TestClass]
    public class BenchmarkFixture
    {
        [TestMethod]
        public void ModulusVsShiftAndMask()
        {
            var sw = new Stopwatch();
            sw.Start();

            for (var i = 0; i < 100000000; i++)
            {
                var res = i%587;
                //var res2 = ((i+512) & 511);
                //Assert.AreEqual(res, res2);
            }

            sw.Stop();
            var time = sw.ElapsedMilliseconds;
        }


        [TestMethod]
        public void IntVDbl()
        {
            var sw = new Stopwatch();

            sw.Start();

            for (int i = 0; i < 100000; i++)
            {
                for (int j = 0; j < 10000; j++)
                {
                    var res = i * j;
                }
            }
            sw.Stop();

            var time = sw.ElapsedMilliseconds;
        }


        [TestMethod]
        public void UseFunc()
        {
            var sw = new Stopwatch();
            int marg = 4;

            Func<int, int[]> f = i => new[] { i % (marg + 584), i % (marg + 597), i % (marg + 507), i % (marg + 187), i % (marg + 687), i % (marg + 547) };

            sw.Start();
            for (var i = 0; i < 100000000; i++)
            {
                //var res = new[] { i % (marg + 584), i % (marg + 597), i % (marg + 507), i % (marg + 187), i % (marg + 687), i % (marg + 547) };
                var res = f(i);
                //var res2 = ((i+512) & 511);
                //Assert.AreEqual(res, res2);
            }

            sw.Stop();
            var time = sw.ElapsedMilliseconds;
        }


        [TestMethod]
        public void TestCapture()
        {

            var p = new[] {5};
            var f = MakeFunc(p);
            p[0] = 6;

            var res = f.Invoke();
        }

        Func<int> MakeFunc(int[] param)
        {
            var temp = param[0];
            return () => temp;
        }

    }


}
