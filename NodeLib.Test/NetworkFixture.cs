using System;
using System.Linq;
using LibNode;
using MathLib.NumericTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NodeLib.Test
{
    [TestClass]
    public class NetworkFixture
    {
        Rop.RopResult<RandMatrixGen, string> MakeGenForRandomDenseMatrix(int rowCount, int colCount, int seed, float maxVal)
        {
            return RmgUtil.MakeGenForRandomDenseMatrix(
                                    rowCount: rowCount,
                                    colCount: colCount,
                                    seed: seed,
                                    maxVal: maxVal);
        }


        Rop.RopResult<Entity, string> MakeGenForRandomDenseMatrixEntity(
            IEntityRepo repo, int rowCount, 
            int colCount, int seed,
            float maxVal, string entityName)
        {
            return RmgUtil.MakeRandomDenseMatrixEntity(
                                    repo: repo,
                                    rowCount: rowCount,
                                    colCount: colCount,
                                    seed: seed,
                                    maxVal: maxVal,
                                    entityName: entityName);
        }

        Rop.RopResult<DataRecord, string> MakeRandomDenseMatrixDataRecord(
                IEntityRepo repo, int rowCount,
                int colCount, int seed,
                float maxVal, string entityName)
        {
            return RmgUtil.MakeRandomDenseMatrixDataRecord(
                            repo: repo,
                            rowCount: rowCount,
                            colCount: colCount,
                            seed: seed,
                            maxVal: maxVal,
                            entityName: entityName);
        }

        [TestMethod]
        public void TestRmgGen()
        {
            var genRes = MakeGenForRandomDenseMatrix(rowCount: 10, colCount: 12, seed: 1234, maxVal: 0.3f);
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
            var ent = MakeGenForRandomDenseMatrixEntity(
                repo: repo, rowCount: 10, colCount: 12, seed: 1234, maxVal: 0.3f, entityName: "Ralph");
            Assert.IsTrue(ent.IsSuccess);
        }

        [TestMethod]
        public void TestRmgSaveEntity()
        {
            IEntityRepo repo = new EntityRepoMem();
            var gen = MakeRandomDenseMatrixDataRecord(
                repo: repo, rowCount: 10, colCount: 12, seed: 1234, maxVal: 0.3f, entityName: "Ralph");
            Assert.IsTrue(gen.IsSuccess);

            var ent = Rop.ExtractResult(gen).Value;
        }

    }
}
