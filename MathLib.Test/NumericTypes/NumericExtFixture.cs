using System;
using MathLib.NumericTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathLib.Test.NumericTypes
{
    [TestClass]
    public class NumericExtFixture
    {
        [TestMethod]
        public void FractionOf()
        {
            Assert.IsTrue(Math.Abs(20.FractionOf(60) - 0.333333333) < 0.00001);
        }
    }
}
