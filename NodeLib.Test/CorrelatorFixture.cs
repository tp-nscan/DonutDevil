using LibNode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NodeLib.Test
{
    [TestClass]
    public class CorrelatorFixture
    {

        [TestMethod]
        public void TestCorrelator()
        {
            const int ensembleCount = 10;
            const int nodeCount = 15;
            const int seed = 145;
            const float maxVal = 0.3f;
            const float clipFrac = 0.8f;

            IEntityRepo repo = new EntityRepoMem();

            var ces = RmgUtil.MakeRdmEntityData
                (
                    repo: repo,
                    rowCount: ensembleCount,
                    colCount: nodeCount,
                    seed: seed,
                    maxVal: maxVal,
                    entityName: "States"
                );

            var dr = Rop.ExtractResult(ces).Value;

            var corr = CgBuilder.MakeCorrelatorDataRecord(
                repo: repo,
                statesData: dr,
                clipFrac: clipFrac,
                entityName: "Correlato"
                );

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestMutator()
        {
        }
    }
}
