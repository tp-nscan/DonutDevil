using System;
using System.Diagnostics;
using System.Linq;
using MathLib.NumericTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathLib.Test.NumericTypes
{
    [TestClass]
    public class NeighborhoodFixture
    {
        [TestMethod]
        public void TestNeighborhoodCtor()
        {
            var nbrhd = new Neighborhood<double>(2);
            nbrhd.ApplyRadiusFunc<double>(v=>v);

            foreach (var ps in nbrhd.ToPairStream())
            {
                Debug.WriteLine("{0} {1} {2}", ps.X, ps.Y, ps.Value);
            }
            
        }

        [TestMethod]
        public void TestCircularGlauber()
        {
            var freq = 2.0;
            var decay = 2.2;
            var nbrhd = NeighborhoodExt.CircularGlauber(2, freq, decay);

            foreach (var ps in nbrhd.ToPairStream())
            {
                Debug.WriteLine("{0} {1} {2}", ps.X, ps.Y, ps.Value);
            }

            foreach (var ps in nbrhd.ToReadingOrder)
            {
                Debug.WriteLine("{0}", ps);
            }

        }



        [TestMethod]
        public void TestCircularGlauberSums()
        {
            var radius = 4;

            for (var freq = 0.5; freq < 0.75; freq += 0.025)
            {
                for (var decay = 0.25; decay < 0.75; decay += 0.025)
                {
                    var nbrhd = NeighborhoodExt.CircularGlauber(radius, freq, decay);

                    var net = nbrhd.ToReadingOrder.Sum();

                    //Debug.WriteLine("{0}\t{1}\t{2}", decay, freq, net.ToString("0.00"));
                    Debug.Write(net.ToString("0.00") +"\t");
                }
                Debug.WriteLine("");
            }

        }


    }
}
