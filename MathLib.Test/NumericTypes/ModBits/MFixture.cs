using System.Linq;
using MathLib.NumericTypes.ModBits;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathLib.Test.NumericTypes.ModBits
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMoveTowards()
        {
            const int range = 2;
            const int modulus = 10;

            var results = Enumerable.Range(0, modulus)
                .Select(i => 0.MoveTowardsM(i, range, modulus))
                .ToList();

            var results2 = Enumerable.Range(0, modulus)
                .Select(i => 4.MoveTowardsM(i, range, modulus))
                .ToList();
        }

        [TestMethod]
        public void TestAddWithNegativeArgs()
        {
            var m8a = (-0.0001).AsM8();
            var m8b = (1.0).AsM8();


            for (var i = -2.0; i < 2.0; i+=0.001)
            {
                var m8 = i.AsM8();
                System.Diagnostics.Debug.WriteLine("{0} \t {1}", i.ToString("0.0000"), m8);
                //var mp = new M4(i).Add(new M4(-i));
                //Assert.IsTrue(mp.DblValue == 0);
            }
        }

    }
}
