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
            var ubA = Parameters.RandomMatrixSet(
                                    entityId: entityGuid, 
                                    matrixId: matrixGuid, 
                                    rowCount: rowCount, 
                                    colCount: colCount, 
                                    seed: seed, 
                                    maxValue: maxValue);
            var res = RmgBuilder.CreateRandMatrixFromParams(ubA);
            return Rop.ExtractResult(res).Value;
        }

        [TestMethod]
        public void TestRmgGen()
        {
            var gen = (IEntityGen)MakeRmg();
            var matrixRes = gen.GetGenResult(EpnConvert.FromString("Matrix"));
            var matrix = Rop.ExtractResult(matrixRes).Value;
            Assert.IsTrue(matrixRes.IsSuccess);
            Assert.IsTrue(matrixRes != null);
        }

        [TestMethod]
        public void TestRmgEntity()
        {
            IEntityGen gen = MakeRmg();
            IEntityRepo repo = new EntityRepoMem();
            var entRes = EntityOps.MakeEntityFromGen(repo, gen);
            var ent = Rop.ExtractResult(entRes).Value;
            Assert.IsTrue(entRes.IsSuccess);
            Assert.IsTrue(ent != null);
        }

        [TestMethod]
        public void TestRmgSaveEntity()
        {
            var gen = MakeRmg();
            IEntityRepo repo = new EntityRepoMem();
            var entRes = EntityOps.SaveEntityFromGen(repo, gen);
            var ent = Rop.ExtractResult(entRes).Value;
            Assert.IsTrue(entRes.IsSuccess);
            Assert.IsTrue(ent != null);
        }
    }
}
