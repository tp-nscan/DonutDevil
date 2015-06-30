using System;
using System.Linq;
using LibNode;
using MathNet.Numerics.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NodeLib.Test
{
    [TestClass]
    public class GeneratorsFixture
    {
        [TestMethod]
        public void TestMakeNode()
        {
            const int arraylength = 5;
            var foo = new NodeGroup(arraylength);

            var nodes = new[] {new Node(1.2f, 3), new Node(1.7f, 4)};
            foo.AddNodes(nodes);

            Assert.AreEqual(arraylength, foo.ArrayLength);

            Assert.AreEqual(arraylength, foo.Values.Length);
        }

        [TestMethod]
        public void TestRandUsFloat32()
        {
            var q = Generators.SeqOfRandSF32(1.0f, new MersenneTwister(1234))
                .Take(500)
                .ToList();

            var tot = q.Sum();

            Assert.IsTrue(Math.Abs(tot) < 35);
        }

        [TestMethod]
        public void TestPerturbInRange()
        {
            var q = Generators.SeqOfRandSF32(0.2f, new MersenneTwister(1234)).Take(500).ToArray();
            var tot = q.Sum();

            Assert.IsTrue(Math.Abs(tot) < 35);

            var mode = Generators.PerturbInRangeF32A(-0.3f, 0.3f, 0.2f, new MersenneTwister(1234), q
                );

            var tot2 = mode.Sum();

            Assert.IsTrue(Math.Abs(tot2) < 35);
        }


        //[TestMethod]
        //public void TestMutateSeq()
        //{
        //    var q = Generators.SeqOfRandSF32(0.2f, new MersenneTwister(1234))
        //                .Take(5).ToArray();
        //    var tot = q.Sum();

        //    Assert.IsTrue(Math.Abs(tot) < 35);

        //    var res = Generators.MesForArraySF32(
        //            noiseLevel: 0.02f,
        //            rng: new MersenneTwister(1234),
        //            count: 2,
        //            array: q
        //        );



        //    Assert.IsTrue(true);
        //}


        [TestMethod]
        public void TestMutateSeq2()
        {
            var dblSeq = new []
            {
                new[] {.01f, .02f, .03f },
                new[] {-.01f, -.02f, -.03f }
            };

            var res = Generators.MesForArraySF32(0.02f, new MersenneTwister(1234), 4, dblSeq
                );

            Assert.IsTrue(true);
        }




        [TestMethod]
        public void TestFlipUF32()
        {
            var ubA = Generators.SeqOfRandUF32Bits(0.5f, new MersenneTwister(1234)).Take(1000).ToArray();
            var tot = ubA.Sum();

            Assert.IsTrue(Math.Abs(tot) - 500 < 45);

            var flipped = Generators.FlipUF32A(0.2f, new MersenneTwister(1234), ubA);
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
            var ubA = Generators.SeqOfRandUF32Bits(0.5f, new MersenneTwister(1234)).Take(1000).ToArray();

            var tot = ubA.Sum();

            Assert.IsTrue(Math.Abs(tot) < 85);

            var flipped = Generators.FlipF32A(0.2f, new MersenneTwister(1234), ubA);


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
            var ubA = Generators.RandomEuclideanPointsF32(true, new MersenneTwister(1234))
                .Take(100)
                .ToArray();

            Assert.IsTrue(ubA.Length == 100);

        }

        [TestMethod]
        public void TestRandomDiscPointF32()
        {
            var ubA = Generators.RandDiscPointsF32(
                rng: new MersenneTwister(1234),
                maxRadius: (float) 2.0).Take(100)
                .Select(MathUtils.PointF32LengthSquared)
                .ToArray();

            Assert.IsTrue(ubA.Length == 100);

        }

        [TestMethod]
        public void TestRandomRingPointF32()
        {
            var ubA = Generators.RandRingPointsF32(new MersenneTwister(1234))
                .Select(MathUtils.PointF32LengthSquared)
                .Take(100)
                .ToArray();

            Assert.IsTrue(ubA.Length == 100);

        }

        [TestMethod]
        public void TestRandomBallPointF32()
        {
            var ubA = Generators.RandBallPointsF32((float) 1.0, new MersenneTwister(1234)
                ).Take(100)
                .Select(MathUtils.TripleF32LengthSquared)
                .ToArray();

            Assert.IsTrue(ubA.Length == 100);

        }

        [TestMethod]
        public void TestRandomSpherePointF32()
        {
            var ubA = Generators.RandSpherePointsF32(new MersenneTwister(1234))
                .Select(MathUtils.TripleF32LengthSquared)
                .Take(100)
                .ToArray();

            Assert.IsTrue(ubA.Length == 100);

        }
    }
}
