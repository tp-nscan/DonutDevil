using System;
using MathLib.NumericTypes.ModBits;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathLib.Test.NumericTypes.ModBits
{
    [TestClass]
    public class M2Fixture
    {
        [TestMethod]
        public void TestM8Corners()
        {
            var randy = new Random();
            for (var i = 0; i < 100; i++)
            {
                var start = randy.Next(M8By8.Mod);

                var corners = start.ToCornersOnM8By8();

                var upLeftCorners = corners[0].ToCornersOnM8By8();

                var upRightCorners = corners[1].ToCornersOnM8By8();

                var lowLeftCorners = corners[2].ToCornersOnM8By8();

                var lowRightCorners = corners[3].ToCornersOnM8By8();


                Assert.AreEqual(upLeftCorners[3], start);
                Assert.AreEqual(upRightCorners[2], start);
                Assert.AreEqual(lowLeftCorners[1], start);
                Assert.AreEqual(lowRightCorners[0], start);
            }
        }

        [TestMethod]
        public void TestM8Sides()
        {
            var randy = new Random();
            for (var i = 0; i < 100; i++)
            {
                var start = randy.Next(M8By8.Mod);


                var sides = start.ToSidesOnM8By8();

                var topSides = sides[0].ToSidesOnM8By8();

                var rightSides = sides[1].ToSidesOnM8By8();

                var bottomSides = sides[2].ToSidesOnM8By8();

                var leftSides = sides[3].ToSidesOnM8By8();


                Assert.AreEqual(topSides[2], start);
                Assert.AreEqual(rightSides[3], start);
                Assert.AreEqual(bottomSides[0], start);
                Assert.AreEqual(leftSides[1], start);
            }
        }

        [TestMethod]
        public void TestM8ForceFields()
        {
            var randy = new Random();
            for (var i = 0; i < 1000; i++)
            {
                var start = randy.Next(M8By8.Mod);

                var forceFieldsOnM8By8 = start.ToForceFieldsOnM8By8();

                var vX = forceFieldsOnM8By8[1];

                var vY = forceFieldsOnM8By8[2];

                var vSum = vX*vX + vY*vY;

                Assert.IsTrue(Math.Abs(1.0f - vSum) < 0.0001);

            }
        }


        [TestMethod]
        public void TestM8ForceFieldsWithAdj()
        {
            var randy = new Random();
            for (var i = 0; i < 1000; i++)
            {
                var start = randy.Next(M8By8.Mod);

                var forceFieldsOnM8By8 = start.ToForceFieldsOnM8By8();

                var vM = forceFieldsOnM8By8[0];

                System.Diagnostics.Debug.WriteLine("{0}\t{1}", 
                                    vM.ToString("0.00"),    
                                    vM.Tent(0.25f).ToString("0.00"));

            }
        }

    }
}
