using System;
using LibNode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NodeLib.Test
{
    [TestClass]
    public class TestNetwork
    {
        IEntityGen MakeRmg()
        {
            var entityGuid = Guid.NewGuid();
            var matrixGuid = Guid.NewGuid();
            const int rowCount = 10;
            const int colCount = 12;
            const int seed = 1234;
            const float maxValue = 0.3f;
            var ubA = Parameters.RandomMatrixSet(entityId: entityGuid, matrixId: matrixGuid, rowCount: rowCount, colCount: colCount, seed: seed, maxValue: maxValue);
            var res = RmgBuilder.CreateRandMatrixFromParams(ubA);
            return Rop.ExtractResult(res).Value;
        }

        [TestMethod]
        public void TestRmgBuilder()
        {
            var gen = (IEntityGen)MakeRmg();
            var matrixRes = gen.GetGenResult(EpnConvert.FromString("Matrix"));
            var matrix = Rop.ExtractResult(matrixRes).Value;
            Assert.IsTrue(matrixRes != null);
        }

        [TestMethod]
        public void TestRmgBuilder2()
        {
            IEntityGen gen = MakeRmg();
            IEntityRepo repo = new EntityRepoMem();
            var entRes = EntityOps.SaveEntityGen(repo, gen, "hi");
            //var ent = Rop.ExtractResult(entRes).Value;
            Assert.IsTrue(entRes != null);
        }
    }
}
