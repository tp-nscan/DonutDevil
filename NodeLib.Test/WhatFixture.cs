using System;
using System.Diagnostics;
using System.Linq;
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
            var sw = new Stopwatch();

            var what = Spots1dBuilder.CreateRandom(
                    seed: 123,
                    ngSize: 512,
                    cSig: 0.75,
                    pSig: 0.75,
                    noiseLevel: 0.05,
                    stepC: 0.01,
                    stepS: 0.01,
                    stepR: 0.05,
                    glauberRadius: 9
                ).Value;

            sw.Start();
            for (var i = 0; i < 500; i++)
            {
                what = what.Update().Value;
            }

            sw.Stop();
            var time = sw.ElapsedMilliseconds;
            Assert.IsNotNull(what);
        }

        [TestMethod]
        public void TestWhat()
        {
            var sw = new Stopwatch();

            var what = WngBuilder.CreateRandom(
                seed: 123,
                ngSize: 100,
                ppSig: 0.75f,
                pSig: 0.75f,
                sSig: 0.75f,
                pNoise: 0.1f,
                sNoise: 0.01f,
                cPp: 0.01f,
                cSs: 0.01f,
                cRp: 0.01f,
                cPs: 0.01f,
                glauberRadius: 5
                ).Value;

            sw.Start();

            var whatsNext = what.Update();
            for (var i = 0; i < 100; i++)
            {
                for (var j = 0; j < 50; j++)
                {
                    whatsNext = whatsNext.Update();
                }
                whatsNext = whatsNext.Learn(0.05f);
            }

            sw.Stop();


            Assert.IsNotNull(what);
        }

        [TestMethod]
        public void TestUtCoords()
        {
            var rng = new MathNet.Numerics.Random.MersenneTwister(123);
            var res = MathNetUtils.RandNormalSqSymDenseSF32(3, rng, 0.7);


        }
    }
}
