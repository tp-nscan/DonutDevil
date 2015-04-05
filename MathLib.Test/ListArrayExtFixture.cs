using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathLib.Test
{
    [TestClass]
    public class ListArrayExtFixture
    {
        [TestMethod]
        public void SymmetricArrayValues()
        {
            const int stride = 17;
            const int rowDex = 3;
            var testRFunc = new Func<int,int,int>((i, j) => i + j);
            var testArray = testRFunc.ToLowerTriangular(stride);
            Assert.AreEqual(stride.ToLowerTriangularArraySize(), testArray.Count());

            var rowTrace = testArray.SymmetricArrayValues(rowDex, stride).ToList();

            Assert.AreEqual(stride - 1, rowTrace.Count());

            for (var i = 0; i < rowDex; i++)
            {
                Assert.AreEqual(rowDex + i , rowTrace[i]);
            }

            for (var i = rowDex; i < stride - 1; i++)
            {
                Assert.AreEqual(rowDex + i + 1, rowTrace[i]);
            }
        }

        [TestMethod]
        public void TestAutoCorrelate()
        {
            var tArray = new[] { 1, 1, 1, -1, 1, -1, 1, 1, 1, -1, 1, -1, 1, 1, 1, -1, 1, -1, 1, 1, 1, -1, 1, -1, 1, 1, 1, -1, 1, -1, 1, 1, 1, -1, 1, -1, 1, 1, 1, -1, 1, -1, 1, 1, 1, -1, 1, -1, 1, 1, 1, -1, 1, -1, 1, 1, 1, -1, 1, -1, 1, 1, 1, -1, 1, -1, 1, 1, 1, -1, 1, -1 };

            var sw = new Stopwatch();

            sw.Start();

            for (var i = 0; i < 100000; i++)
            {
                var res = tArray.AutoCorrelate();
            }

            var time = sw.ElapsedMilliseconds;

        }
    }
}
