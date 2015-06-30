using System.Diagnostics;
using LibNode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NodeLib.Test
{
    [TestClass]
    public class PerturbFixture
    {
        [TestMethod]
        public void TestPerturb()
        {
            const int ensembleCount = 10;
            const int nodeCount = 15;
            const int initSeed = 145;
            const int perturbSeed = 1145;
            const float maxVal = 0.3f;
            const float mutationRate = 0.08f;
            const int replicationRate = 5;

            IEntityRepo repo = new EntityRepoMem();

            var gen = RmgUtil.MakeRdmEntityData(
                repo: repo, 
                rowCount: ensembleCount, 
                colCount: nodeCount, 
                seed: initSeed, 
                maxVal: maxVal, 
                entityName: "Ralph");
            Assert.IsTrue(gen.IsSuccess);

            var initialEnt = Rop.ExtractResult(gen).Value;


            var perturbed = PerturbBuilder.MakePerturberEntityData(
                    repo: repo,
                    statesData: initialEnt,
                    seed: perturbSeed,
                    mutationRate: mutationRate,
                    replicationRate: replicationRate,
                    unsigned: false,
                    entityName: "testPerturb"
                );
            Assert.IsTrue(perturbed.IsSuccess);
        }
    }
}
