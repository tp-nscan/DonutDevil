using System;
using System.Linq;
using LibNode;
using MathLib.NumericTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NodeLib.Test
{
    [TestClass]
    public class CliqueEnsembleFixture
    {
        [TestMethod]
        public void TestCliqueEnsemble()
        {
            const int ensembleCount = 10;
            const int nodeCount = 5;
            const int seed = 145;
            const float maxVal = 0.3f;
            const float noiseLevel = 0.15f;
            const float stepSize = 0.05f;
            IEntityRepo repo = new EntityRepoMem();

            var ces = CliqueEnsemble.MakeGenForRandomCliqueEnsemble
                (
                    repo: repo,
                    ensembleCount: ensembleCount,
                    nodeCount: nodeCount,
                    seed: seed,
                    maxVal: maxVal,
                    noiseLevel: noiseLevel,
                    stepSize: stepSize,
                    useGpu: false,
                    name: "Tom"
                );

            Assert.IsTrue(ces.IsSuccess);
        }

        [TestMethod]
        public void TestCliqueEnsembleIterations()
        {
            const int ensembleCount = 10;
            const int nodeCount = 5;
            const int seed = 145;
            const int iterationCount = 500;
            const float maxVal = 0.3f;
            const float noiseLevel = 0.15f;
            const float stepSize = 0.05f;
            IEntityRepo repo = new EntityRepoMem();

            var ces = CliqueEnsemble.MakeGenForRandomCliqueEnsemble
                (
                    repo: repo,
                    ensembleCount: ensembleCount,
                    nodeCount: nodeCount,
                    seed: seed,
                    maxVal: maxVal,
                    noiseLevel: noiseLevel,
                    stepSize: stepSize,
                    useGpu: false,
                    name: "Tom"
                );

            Assert.IsTrue(ces.IsSuccess);
            ISym gen = Rop.ExtractResult(ces).Value;
            System.Diagnostics.Debug.WriteLine(CegBuilder.ExtractEnsemble(gen).Value.Take(50).ToCsvString("0.000"));

            for (var i = 0; i < iterationCount; i++)
            {
                var nextGenRes = gen.Update();
                var nextGen = Rop.ExtractResult(nextGenRes).Value;
                System.Diagnostics.Debug.WriteLine(CegBuilder.ExtractEnsemble(nextGen).Value.Take(50).ToCsvString("0.000"));
                gen = nextGen;
            }
        }

    }
}
