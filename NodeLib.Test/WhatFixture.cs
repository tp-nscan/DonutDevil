using System;
using LibNode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NodeLib.Test
{
    [TestClass]
    public class WhatFixture
    {

        [TestMethod]
        public void TestSpots1D()
        {
            var what = Spots1dBuilder.CreateRandom(
                seed: 123,
                ngSize: 25,
                cSig: 0.75,
                pSig: 0.75,
                noiseLevel: 0.1,
                stepC: 0.01,
                stepS: 0.01,
                stepR: 0.01,
                glauberRadius: 5
                ).Value;

            what = what.Update().Value;

            Assert.IsNotNull(what);
        }

        [TestMethod]
        public void TestWhat()
        {
            var what = WhatNodeBuilder.CreateRandom(
                seed: 123,
                ngSize: 25,
                cSig: 0.75f,
                pSig: 0.75f,
                sSig: 0.75f,
                noiseLevel: 0.1f,
                stepC: 0.01f,
                stepS: 0.01f,
                stepR: 0.01f,
                pOs: 0.01f,
                glauberRadius: 5
                ).Value;
            
            Assert.IsNotNull(what);
        }


    }
}
