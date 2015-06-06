using System;
using System.Linq;
using LibNode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NodeLib.Test
{
    [TestClass]
    public class TestMathUtils
    {
        [TestMethod]
        public void TestArray2DRoundTrip()
        {
            const int rowCount = 5;
            const int colCount = 3;
            const int arrayCount = rowCount * colCount;

            var over = Enumerable.Range(0, arrayCount).Select(i => (float)i).ToArray();

            var rowMajorArray =
                MathUtils.Array2DFromRowMajor(rowCount: rowCount, colCount: colCount, values: over);
            var backRm = MathUtils.flattenRowMajor(rowMajorArray.Value).ToArray();
            Assert.IsTrue(MathUtils.CompareFloat32Arrays(over, backRm));


            var colMajorArray =
                MathUtils.Array2DFromColumnMajor(rowCount: rowCount, colCount: colCount, values: over);
            var backCm = MathUtils.flattenColumnMajor(colMajorArray.Value).ToArray();
            Assert.IsTrue(MathUtils.CompareFloat32Arrays(over, backCm));


            var transpArray = MathUtils.TransposeArray2D(colMajorArray.Value);
            var backT = MathUtils.flattenRowMajor(transpArray).ToArray();
            Assert.IsTrue(MathUtils.CompareFloat32Arrays(over, backT));
        }
    }
}
