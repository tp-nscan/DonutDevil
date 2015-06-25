using LibNode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NodeLib.Test
{
    [TestClass]
    public class RandMatrixGenFixture
    {
        [TestMethod]
        public void TestRmgGen()
        {
            var genRes = RmgUtil.MakeGenForRdm(rowCount: 10, colCount: 12, seed: 1234, maxVal: 0.3f);
            Assert.IsTrue(genRes.IsSuccess);
            var genMatrix = (IEntityGen)Rop.ExtractResult(genRes).Value;

            var matrixRes = genMatrix.GetGenResult(Entvert.ToEpn("Matrix"));
            Assert.IsTrue(matrixRes.IsSuccess);

            var matrix = Rop.ExtractResult(matrixRes).Value;
            Assert.IsTrue(matrix.ArrayData.IsFloat32Array);
        }


        [TestMethod]
        public void TestMakeRmgEntity()
        {
            IEntityRepo repo = new EntityRepoMem();
            var ent = RmgUtil.MakeRdmEntity(
                repo: repo, rowCount: 10, colCount: 12, seed: 1234, maxVal: 0.3f, entityName: "Ralph");
            Assert.IsTrue(ent.IsSuccess);
        }

        [TestMethod]
        public void TestRmgSaveEntity()
        {
            IEntityRepo repo = new EntityRepoMem();
            var gen = RmgUtil.MakeRdmDataRecord(
                repo: repo, rowCount: 10, colCount: 12, seed: 1234, maxVal: 0.3f, entityName: "Ralph");
            Assert.IsTrue(gen.IsSuccess);

            var ent = Rop.ExtractResult(gen).Value;
        }

    }
}
