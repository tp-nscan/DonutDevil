using System;
using MathLib.NumericTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathLib.Test
{
    [TestClass]
    public class FunctionsFixture
    {
        [TestMethod]
        public void TestUnitToSinAndCos()
        {
            for (var i = 0.001f; i < 1.0f; i += 0.001f)
            {
                Assert.IsTrue(Math.Abs(( i.UnitToCos() + (i + 0.5).AsMf().UnitToCos() )) < Mf.LookupTablePrecision);
            }

        }
    }
}
