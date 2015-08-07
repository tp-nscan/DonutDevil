using System.Diagnostics;
using System.Linq;
using LibNode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NodeLib.Test
{
    [TestClass]
    public class WhatFixture
    {
        const int TargetLength = 10;
        static ArrayHist MakeArrayHistories()
        {

            var ah = ArrayHistory.Init(
                name: "Ralph",
                arrayLength: 3
                );

            ah = ArrayHistory
                    .Add(ah, Enumerable.Range(10, 3)
                    .Select(i => (float) i), 4, TargetLength);

            ah = ArrayHistory
                    .Add(ah, Enumerable.Range(20, 3)
                    .Select(i => (float)i), 4, TargetLength);

            ah = ArrayHistory
                    .Add(ah, Enumerable.Range(30, 3)
                    .Select(i => (float)i), 4, TargetLength);

            return ah;
        }

        static ArrayHist MakeArrayHistoriesLong()
        {
            var ah = ArrayHistory.Init(
                    name: "Ralph",
                    arrayLength: 3
                );

            for (var ct = 1; ct< 4000; ct++)
            {
                ah = ArrayHistory
                        .Add(ah, Enumerable.Range(ct*10, 3)
                        .Select(i => (float)i), ct, TargetLength);
            }

            return ah;
        }

        [TestMethod]
        public void TestArrayHistory()
        {
            var ah = MakeArrayHistories();
            var d2s = ArrayHistory.GetD2Vals(ah);
            var d2a = d2s.ToArray();
        }

        [TestMethod]
        public void TestArrayHistoryLong()
        {
            var ah = MakeArrayHistoriesLong();
            var d2s = ArrayHistory.GetIndxes(ah);
            var d2a = d2s.ToArray();
            Debug.WriteLine(d2a.Aggregate(string.Empty,(s,i)=> s + "\n" + i));
        }

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
                pNoiseLevel: 0.1f,
                sNoiseLevel: 0.01f,
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
        public void TestDesignAthenaTr()
        {
            var res = ZeusBuilders.DesignAthenaTr(
                seed: 123,
                ngSize: 24,
                memSize: 16,
                ppSig: 0.4,
                glauberRadius: 5
                );

            var p = res.Value;
        }

        [TestMethod]
        public void TestUpdateTrRep()
        {
            var z = ZeusBuilders.CreateRandomZeus(
                    seed:12347,
                    ngSize:24,
                    memSize:16,
                    ppSig: 0.4,
                    glauberRadius:5
                ).Value;

            var a = ZeusBuilders.CreateRandomAthena(
                    seed: 12347,
                    ngSize: 24,
                    pSig: 0.4,
                    sSig: 0.4
                );

            //var res = ZeusF.RepAthenaTr(
            //    zeus:z,
            //    memIndex: 1,
            //    pNoiseLevel: 0.2,
            //    sNoiseLevel: 0.2,
            //    seed: 123,
            //    cPp:0.1f, cSs: 0.1f, cRp: 0.1f, cPs: 0.1f,
            //    athena: a,
            //    reps: 4
            //);
        }

    }
}
