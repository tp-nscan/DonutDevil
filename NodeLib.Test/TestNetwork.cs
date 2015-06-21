using System;
using System.Linq;
using LibNode;
using MathLib.NumericTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NodeLib.Test
{
    [TestClass]
    public class TestNetwork
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

        [TestMethod]
        public void TestCliqueEnsemble()
        {
            const int ensembleCount = 10;
            const int nodeCount = 5;
            const int nodeSeed = 145;
            const float noiseLevel = 0.15f;
            const float stepSize = 0.05f;
            IEntityRepo repo = new EntityRepoMem();

            var drERes = MakeRandomDenseMatrixDataRecord(repo, rowCount: ensembleCount, colCount: nodeCount, entityName:"ensemble", seed:123, maxVal:0.3f);
            var drE = Rop.ExtractResult(drERes).Value;

            var drCRes = MakeRandomDenseMatrixDataRecord(repo, rowCount: nodeCount, colCount: nodeCount, entityName: "connections", seed: 1243, maxVal: 0.3f);
            var drC = Rop.ExtractResult(drCRes).Value;

            var ubA = Parameters.CliqueSet(
                         unsigned: false,
                         stepSize: stepSize,
                         noiseSeed: nodeSeed,
                         noiseLevel: noiseLevel);

            var genRes = CliqueEnsembleBuilder.CreateCliqueEnsembleFromParams
                (
                    entityRepo: repo,
                    ensembleId: drE.DataId,
                    connectionsId: drC.DataId,
                    entityData: new[]
                    {
                        EntityOps.ToEntityData(drE),
                        EntityOps.ToEntityData(drC)
                    },
                    prams: ubA
                );

            var gen = Rop.ExtractResult(genRes).Value;
            System.Diagnostics.Debug.WriteLine(CliqueEnsembleBuilder.ExtractEnsemble(gen).Value.Take(20).ToCsvString("0.000"));

            var nextGenRes = ((ISym) gen).Update();
            var nextGen = Rop.ExtractResult(nextGenRes).Value;
            System.Diagnostics.Debug.WriteLine(CliqueEnsembleBuilder.ExtractEnsemble(nextGen).Value.Take(20).ToCsvString("0.00"));

            Assert.IsTrue(nextGen != null);

        }


        [TestMethod]
        public void TestCliqueEnsembleIterations()
        {
            const int ensembleCount = 2;
            const int nodeCount = 25;
            const int nodeSeed = 145;
            const int nodeSeed2 = 1456;
            const int iterationCount = 500;
            const float noiseLevel = 0.05f;
            const float stepSize = 0.05f;
            IEntityRepo repo = new EntityRepoMem();

            var drERes = MakeRandomDenseMatrixDataRecord(repo, rowCount: ensembleCount, colCount: nodeCount, entityName: "ensemble", seed: 123, maxVal: 0.3f);
            var drE = Rop.ExtractResult(drERes).Value;

            var drCRes = MakeRandomDenseMatrixDataRecord(repo, rowCount: nodeCount, colCount: nodeCount, entityName: "connections", seed: 1243, maxVal: 0.3f);
            var drC = Rop.ExtractResult(drCRes).Value;

            var ubA = Parameters.CliqueSet(
                         unsigned: false,
                         stepSize: stepSize,
                         noiseSeed: nodeSeed,
                         noiseLevel: noiseLevel);

            var genRes = CliqueEnsembleBuilder.CreateCliqueEnsembleFromParams
                (
                    entityRepo: repo,
                    ensembleId: drE.DataId,
                    connectionsId: drC.DataId,
                    entityData: new[]
                    {
                        EntityOps.ToEntityData(drE),
                        EntityOps.ToEntityData(drC)
                    },
                    prams: ubA
                );

            ISym gen = Rop.ExtractResult(genRes).Value;
            System.Diagnostics.Debug.WriteLine(CliqueEnsembleBuilder.ExtractEnsemble(gen).Value.Take(50).ToCsvString("0.000"));

            for (var i = 0; i < iterationCount; i++)
            {
                var nextGenRes = gen.Update();
                var nextGen = Rop.ExtractResult(nextGenRes).Value;
                System.Diagnostics.Debug.WriteLine(CliqueEnsembleBuilder.ExtractEnsemble(nextGen).Value.Take(50).ToCsvString("0.000"));
                gen = nextGen;
            }

        }

    }
}
