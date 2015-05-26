using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LibNode;

namespace NodeLib.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMakeNode()
        {
            const int arraylength = 5;
            var foo = new NodeGroup(arraylength);

            var nodes = new[] { new Node(1.2f, 3), new Node(1.7f, 4) };
            foo.AddNodes(nodes);

            Assert.AreEqual(arraylength, foo.ArrayLength);

            Assert.AreEqual(arraylength, foo.Values.Length);
        }


        [TestMethod]
        public void TestRandomBools()
        {
            var ubA = MemoryBuilders.MakeRandomBinary(MathUtils.GroupShape.NewRing(5));

            Assert.IsTrue(ubA.IsBinary);

        }
        
        [TestMethod]
        public void TestMakeRandomBinaryBlock()
        {
            var ubA = MemoryBuilders.MakeRandomBinaryDataBlock(MathUtils.GroupShape.NewRing(5), 5, "setName");
            Assert.IsTrue(ubA.IsKvpList);
        }

        [TestMethod]
        public void TestRandUsFloat32()
        {
            var q = Enumerable.Range(0, 500)
                              .Select(i => Generators.RandUsFloat32((float) 0.5))
                              .ToList();

            var tot = q.Sum();

            Assert.IsTrue(Math.Abs(tot) < 35);
        }

        [TestMethod]
        public void TestPerturbInRange()
        {
            var q = Enumerable.Range(0, 500)
                .Select(i => Generators.RandUsFloat32(
                    max: (float) 0.2)).ToArray();

            var tot = q.Sum();

            Assert.IsTrue(Math.Abs(tot) < 35);

            var mode = Generators.PerturbInRangeF32A(
                minVal:   (float) -0.3,
                maxVal:   (float)  0.3,
                maxDelta: (float)  0.2,
                values: q
                );

            var tot2 = mode.Sum();

            Assert.IsTrue(Math.Abs(tot2) < 35);
        }

        [TestMethod]
        public void TestFlipUF32()
        {
            var ubA = Generators.RandBitsUF32.Take(1000).ToArray();

            var tot = ubA.Sum();

            Assert.IsTrue(Math.Abs(tot) - 500 < 45);

            var flipped = Generators.FlipUF32A((float) 0.2, ubA);


            var tot2 = flipped.Sum();

            var d = MathUtils.Euclidean32(ubA, flipped);

            //var mode = Generators.PerturbInRangeF32A(
            //    minVal: (float)-0.3,
            //    maxVal: (float)0.3,
            //    maxDelta: (float)0.2,
            //    values: q
            //    );

            //var tot2 = mode.Sum();

            Assert.IsTrue(Math.Abs(d) < 235);
        }

        [TestMethod]
        public void TestFlipU32()
        {
            var ubA = Generators.RandBitsF32.Take(1000).ToArray();

            var tot = ubA.Sum();

            Assert.IsTrue(Math.Abs(tot) < 85);

            var flipped = Generators.FlipF32A((float)0.2, ubA);


            var tot2 = flipped.Sum();

            var d = MathUtils.Euclidean32(ubA, flipped);

            //var mode = Generators.PerturbInRangeF32A(
            //    minVal: (float)-0.3,
            //    maxVal: (float)0.3,
            //    maxDelta: (float)0.2,
            //    values: q
            //    );

            //var tot2 = mode.Sum();

            Assert.IsTrue(Math.Abs(d) < 235);
        }

        [TestMethod]
        public void TestRandomEuclideanPointF32()
        {
            var ubA = Generators.RandomEuclideanPointsF32(unsigned: true)
                .Take(100)
                .ToArray();

            Assert.IsTrue(ubA.Length == 100);

        }

        [TestMethod]
        public void TestRandomDiscPointF32()
        {
            var ubA = Generators.RandDiscPointsF32(
                maxRadius:  (float)2.0).Take(100)
                        .Select(MathUtils.PointF32LengthSquared)
                        .ToArray();

            Assert.IsTrue(ubA.Length == 100);

        }

        [TestMethod]
        public void TestRandomRingPointF32()
        {
            var ubA = Generators.RandRingPointsF32
                        .Select(MathUtils.PointF32LengthSquared)
                        .Take(100)
                        .ToArray();

            Assert.IsTrue(ubA.Length == 100);

        }

        [TestMethod]
        public void TestRandomBallPointF32()
        {
            var ubA = Generators.RandBallPointsF32(
                maxRadius: (float)1.0).Take(100)
                        .Select(MathUtils.TripleF32LengthSquared)
                        .ToArray();

            Assert.IsTrue(ubA.Length == 100);

        }

        [TestMethod]
        public void TestRandomSpherePointF32()
        {
            var ubA = Generators.RandSpherePointsF32
                        .Select(MathUtils.TripleF32LengthSquared)
                        .Take(100)
                        .ToArray();

            Assert.IsTrue(ubA.Length == 100);

        }

        [TestMethod]
        public void TestArray2DRoundTrip()
        {
            const int rowCount = 5;
            const int colCount = 3;
            const int arrayCount = rowCount*colCount;

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
