using System;
using System.Linq;
using LibNode;
using MathNet.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NodeLib.Test
{
    [TestClass]
    public class MathUtilsFixture
    {
        [TestMethod]
        public void TestArray2DRoundTrip()
        {
            const int rowCount = 5;
            const int colCount = 3;
            const int arrayCount = rowCount*colCount;

            var over = Enumerable.Range(0, arrayCount).Select(i => (float) i).ToArray();

            var rowMajorArray =
                MathUtils.Array2DFromRowMajor(rowCount: rowCount, colCount: colCount, values: over);
            var backRm = MathUtils.flattenRowMajor(Rop.ExtractResult(rowMajorArray).Value).ToArray();
            Assert.IsTrue(MathUtils.CompareFloat32Arrays(over, backRm));


            var colMajorArray =
                MathUtils.Array2DFromColumnMajor(rowCount: rowCount, colCount: colCount, values: over);
            var backCm = MathUtils.flattenColumnMajor(Rop.ExtractResult(colMajorArray).Value).ToArray();
            Assert.IsTrue(MathUtils.CompareFloat32Arrays(over, backCm));


            var transpArray = MathUtils.TransposeArray2D(Rop.ExtractResult(colMajorArray).Value);
            var backT = MathUtils.flattenRowMajor(transpArray).ToArray();
            Assert.IsTrue(MathUtils.CompareFloat32Arrays(over, backT));
        }

        [TestMethod]
        public void TestSymGen()
        {

            var rng = new MathNet.Numerics.Random.MersenneTwister(123);
            var m = MathNetUtils.RandNormalSqSymDenseSF32(3, rng, 0.7);

            Assert.IsTrue(m != null);
        }

        [TestMethod]
        public void TestVectorShift()
        {
            var v = MathNet.Numerics.LinearAlgebra.Vector<float>.Build
                        .Dense(100, i => i * i);

            var vs = MathNetUtils.VectorShift(v);


            Assert.IsTrue(true);
        }


    }
}
