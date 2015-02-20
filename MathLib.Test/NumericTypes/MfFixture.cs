using System;
using MathLib.NumericTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathLib.Test.NumericTypes
{
    [TestClass]
    public class MfFixture
    {
        [TestMethod]
        public void TestAsMf()
        {
            const float fOne = 0.0f;
            const float fTwo = 2.3f;

            Assert.IsTrue(Math.Abs(fOne.AsMf() - fOne) < Mf.Epsilon);
            Assert.IsTrue(Math.Abs((-fOne).AsMf() - fOne) < Mf.Epsilon);

            Assert.IsTrue(Math.Abs(fTwo.AsMf() - 0.3f) < Mf.Epsilon);
            Assert.IsTrue(Math.Abs((-fTwo).AsMf() - 0.7f) < Mf.Epsilon);

        }

        [TestMethod]
        public void TestMfAdd2()
        {
            var randy = new Random();

            for (var i = 0; i < 10000; i++)
            {
                var lhs = 0.5f; // (float)randy.NextDouble();
                var rhs = 0.49999999f; // (float)randy.NextDouble();

                var res = lhs.MfAdd(rhs);

            }
        }

        [TestMethod]
        public void TestMfAdd()
        {
            const float fOne = 0.5f;
            const float fTwo = 2.3f;
            const float fThree = 0.65f;
            Assert.IsTrue(Math.Abs(fTwo.MfAdd(fOne) - 0.8f) < Mf.Epsilon);
            Assert.IsTrue(Math.Abs(fThree.MfAdd(-fOne) - 0.15f) < Mf.Epsilon);
            Assert.IsTrue(Math.Abs(fOne.MfAdd(-fThree) - 0.85f) < Mf.Epsilon);
        }

        [TestMethod]
        public void TestMfDeltaAsFloat()
        {
            const float fOne = 0.05f;
            const float fTwo = 0.3f;
            const float fThree = 0.65f;
            const float fFour = 0.85f;

            Assert.IsTrue(Math.Abs(fOne.MfDeltaAsFloat(fOne) - 0.0) < Mf.Epsilon);
            Assert.IsTrue(Math.Abs(fOne.MfDeltaAsFloat(fTwo) - 0.25f) < Mf.Epsilon);
            Assert.IsTrue(Math.Abs(fOne.MfDeltaAsFloat(fThree) + 0.40f) < Mf.Epsilon);
            Assert.IsTrue(Math.Abs(fOne.MfDeltaAsFloat(fFour) + 0.20f) < Mf.Epsilon);

            Assert.IsTrue(Math.Abs(fFour.MfDeltaAsFloat(fOne) - 0.20f) < Mf.Epsilon);
            Assert.IsTrue(Math.Abs(fFour.MfDeltaAsFloat(fTwo) - 0.45f) < Mf.Epsilon);
            Assert.IsTrue(Math.Abs(fFour.MfDeltaAsFloat(fThree) + 0.20f) < Mf.Epsilon);

        }

        [TestMethod]
        public void TestVDiff()
        {
            const float fOne = 0.05f;
            const float fTwo = 0.3f;

            var res0 = Mf2.VDiff(fOne, fOne, fOne, fOne);
            Assert.IsTrue(res0[0] - 0.0 < Mf.Epsilon);
            Assert.IsTrue(res0[1] - 0.0 < Mf.Epsilon);
            Assert.IsTrue(res0[2] - 0.0 < Mf.Epsilon);

            var res1 = Mf2.VDiff(fOne, fOne, fTwo, fTwo);
            Assert.IsTrue(Math.Abs(res1[0] - res1[1]) < Mf.Epsilon);
            Assert.IsTrue(Math.Abs(res1[1] - 0.25 * 0.25) < Mf.Epsilon);
            Assert.IsTrue(Math.Abs(res1[2] - 0.125)  < Mf.Epsilon);

            var res2 = Mf2.VDiff(fTwo, fTwo, fOne, fOne);
            Assert.IsTrue(Math.Abs(res2[0] - res2[1]) < Mf.Epsilon);
            Assert.IsTrue(Math.Abs(res2[1] + 0.25 * 0.25) < Mf.Epsilon);
            Assert.IsTrue(Math.Abs(res2[2] - 0.125) < Mf.Epsilon);

        }
    }
}
