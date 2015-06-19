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
            var gen = MakeRmg(rowCount: 10, colCount: 12);
            var matrixRes = gen.GetGenResult(Entvert.ToEpn("Matrix"));
            var matrix = Rop.ExtractResult(matrixRes).Value;
            Assert.IsTrue(matrixRes.IsSuccess);
            Assert.IsTrue(matrixRes != null);
        }

        [TestMethod]
        public void TestMakeRmgEntity()
        {
            //IEntityGen gen = MakeRmg(rowCount:10, colCount:12);
            //var entRes = EntityOps.MakeEntity(gen);
            //Assert.IsTrue(entRes != null);
        }

        [TestMethod]
        public void TestRmgSaveEntity()
        {
            var gen = MakeRmg(rowCount: 10, colCount: 12);
            IEntityRepo repo = new EntityRepoMem();
            var entRes = EntityOps.SaveEntityGen(repo, gen, "Rmg");
            var ent = Rop.ExtractResult(entRes).Value;
            var drR = EntityOps.GetResultDataRecord(repo, ent, Entvert.ToEpn("Matrix"));
            var dr = Rop.ExtractResult(drR).Value;

            Assert.IsTrue(entRes.IsSuccess);
            Assert.IsTrue(ent != null);
            Assert.IsTrue(drR.IsSuccess);
        }


        private DataRecord RmgDataRecord(IEntityRepo repo, int rowCount, int colCount, string entityName)
        {
            var gen = MakeRmg(rowCount: rowCount, colCount: colCount);
            var entRes = EntityOps.SaveEntityGen(repo, gen, entityName);
            var ent = Rop.ExtractResult(entRes).Value;
            var drR = EntityOps.GetResultDataRecord(repo, ent, Entvert.ToEpn("Matrix"));
            var dr = Rop.ExtractResult(drR).Value;
            return dr;
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

            var drE = RmgDataRecord(repo, rowCount: ensembleCount, colCount: nodeCount, entityName:"ensemble");
            var drC = RmgDataRecord(repo, rowCount: nodeCount, colCount: nodeCount, entityName: "connections");

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
            const int iterationCount = 500;
            const float noiseLevel = 0.05f;
            const float stepSize = 0.05f;
            IEntityRepo repo = new EntityRepoMem();

            var drE = RmgDataRecord(repo, rowCount: ensembleCount, colCount: nodeCount, entityName: "ensemble");
            var drC = RmgDataRecord(repo, rowCount: nodeCount, colCount: nodeCount, entityName: "connections");

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
