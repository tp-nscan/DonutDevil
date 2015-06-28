﻿using System;
using System.Collections.Generic;
using System.Linq;
using LibNode;
using Microsoft.FSharp.Core;
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

        [TestMethod]
        public void TestMappz()
        {
            var sa = Enumerable.Range(0, 10).Select(i => i + 7);
            var sb = Enumerable.Range(0, 10).Select(i => i * 7);


        }
    }
}
