using MathLib.NumericTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathLib.Test.NumericTypes
{
    [TestClass]
    public class DtFixture
    {
        [TestMethod]
        public void TestSides()
        {
            const int width = 6;
            const int height = 7;

            var ulSides = 0.SidesOnDt(width, height);
            var sidesOf21 = 21.SidesOnDt(width, height);
            var sidesOf24 = 24.SidesOnDt(width, height);
            var lrSides = 41.SidesOnDt(width, height);

            Assert.IsTrue(ulSides.AreEqual(  new[] { 36,  1,  6,  5 }, (a, b) => a == b));
            Assert.IsTrue(sidesOf21.AreEqual(new[] { 15, 22, 27, 20 }, (a, b) => a == b));
            Assert.IsTrue(sidesOf24.AreEqual(new[] { 18, 25, 30, 29 }, (a, b) => a == b));
            Assert.IsTrue(lrSides.AreEqual(  new[] { 35, 36,  5, 40 }, (a, b) => a == b));

        }

        [TestMethod]
        public void TestCorners()
        {
            const int width = 6;
            const int height = 7;

            var cornersOnDt = 0.CornersOnDt(width, height);
            var of21 = 21.CornersOnDt(width, height);
            var of24 = 24.CornersOnDt(width, height);
            var ints = 41.CornersOnDt(width, height);

            Assert.IsTrue(cornersOnDt.AreEqual(new[] { 41, 37,  7, 11 }, (a, b) => a == b));
            Assert.IsTrue(of21.AreEqual(new[] {        14, 16, 28, 26 }, (a, b) => a == b));
            Assert.IsTrue(of24.AreEqual(new[] {        23, 19, 31, 35 }, (a, b) => a == b));
            Assert.IsTrue(ints.AreEqual(new[] {        34, 30,  0,  4 }, (a, b) => a == b));

        }


        [TestMethod]
        public void TestPerimeters()
        {
            const int width = 6;
            const int height = 7;

            var cornersOnDt = 0.PerimeterOnDt(width, height);
            var of21 = 21.PerimeterOnDt(width, height);
            var of24 = 24.PerimeterOnDt(width, height);
            var ints = 41.PerimeterOnDt(width, height);

            Assert.IsTrue(cornersOnDt.AreEqual(new[] { 41, 36, 37,  1,  7,  6, 11,  5 }, (a, b) => a == b));
            Assert.IsTrue(of21.AreEqual(new[] {        14, 15, 16, 22, 28, 27, 26, 20 }, (a, b) => a == b));
            Assert.IsTrue(of24.AreEqual(new[] {        23, 18, 19, 25, 31, 30, 35, 29 }, (a, b) => a == b));
            Assert.IsTrue(ints.AreEqual(new[] {        34, 35, 30, 36,  0,  5,  4, 40 }, (a, b) => a == b));

        }


    }
}
