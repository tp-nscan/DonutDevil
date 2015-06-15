using System;
using LibNode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NodeLib.Test
{
    [TestClass]
    public class TestNetwork
    {
        IEntityGen MakeRmg(int rowCount, int colCount)
        {
            var entityGuid = Guid.NewGuid();
            var matrixGuid = Guid.NewGuid();
            const int seed = 1234;
            const float maxValue = 0.3f;
            var ubA = Parameters.RandomMatrixSet(
                                    rowCount: rowCount,
                                    colCount: colCount,
                                    seed: seed,
                                    maxValue: maxValue);
            var res = RmgBuilder.RandMatrixGenFromParams(ubA);
            return Rop.ExtractResult(res).Value;
        }

        [TestMethod]
        public void TestRmgGen()
        {
            var gen = (IEntityGen)MakeRmg(rowCount: 10, colCount: 12);
            var matrixRes = gen.GetGenResult(EpnConvert.FromString("Matrix"));
            var matrix = Rop.ExtractResult(matrixRes).Value;
            Assert.IsTrue(matrixRes.IsSuccess);
            Assert.IsTrue(matrixRes != null);
        }

        [TestMethod]
        public void TestMakeRmgEntity()
        {
            IEntityGen gen = MakeRmg(rowCount:10, colCount:12);
            var entRes = EntityOps.MakeEntity(gen, "Rmg");
            Assert.IsTrue(entRes != null);
        }

        [TestMethod]
        public void TestRmgSaveEntity()
        {
            var gen = MakeRmg(rowCount: 10, colCount: 12);
            IEntityRepo repo = new EntityRepoMem();
            var entRes = EntityOps.SaveEntityGen(repo, gen, "Rmg");
            var ent = Rop.ExtractResult(entRes).Value;
            var drR = EntityOps.GetResultDataRecord(repo, ent, EpnConvert.FromString("Matrix"));
            var dr = Rop.ExtractResult(drR).Value;

            Assert.IsTrue(entRes.IsSuccess);
            Assert.IsTrue(ent != null);
            Assert.IsTrue(drR.IsSuccess);
        }

        [TestMethod]
        public void TestCliqueEnsemble()
        {
            var gen = MakeRmg(rowCount: 10, colCount: 12);
            IEntityRepo repo = new EntityRepoMem();
            var entRes = EntityOps.SaveEntityGen(repo, gen, "Rmg");
            var ent = Rop.ExtractResult(entRes).Value;
            Assert.IsTrue(entRes.IsSuccess);
            Assert.IsTrue(ent != null);
        }
    }
}
