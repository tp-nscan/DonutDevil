using System;
using System.Linq;
using LibNode;
using MathLib.NumericTypes;

namespace La.ViewModel.Design
{
    public class DesignEntityDataVm : EntityDataVm
    {
        public DesignEntityDataVm() : base(DesignEntityData)
        {
        }

        private static EntityData DesignEntityData
        {
            get
            {
                const int ensembleCount = 2;
                const int nodeCount = 25;
                IEntityRepo repo = new EntityRepoMem();

                var drE = RmgDataRecord(repo, rowCount: ensembleCount, colCount: nodeCount, entityName: "ensemble");

                return EntityOps.ToEntityData(Entvert.ToEpn("Matrix"), drE);
            }
        }

        static Entity DesignEntity
        {
            get
            {
                const int ensembleCount = 2;
                const int nodeCount = 25;
                const int nodeSeed = 5;
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

                var genRes = CegBuilder.CreateCliqueEnsembleFromParams
                    (
                        entityRepo: repo,
                        entityData: new[]
                        {
                            EntityOps.ToEntityData(Entvert.ToEpn("Matrix"), drE),
                            EntityOps.ToEntityData(Entvert.ToEpn("Matrix"), drC)
                        },
                        prams: ubA
                    );

                ISym gen = Rop.ExtractResult(genRes).Value;
                System.Diagnostics.Debug.WriteLine(CegBuilder.ExtractEnsemble(gen).Value.Take(50).ToCsvString("0.000"));


                for (var i = 0; i < iterationCount; i++)
                {
                    var nextGenRes = gen.Update();
                    var nextGen = Rop.ExtractResult(nextGenRes).Value;
                    System.Diagnostics.Debug.WriteLine(CegBuilder.ExtractEnsemble(nextGen).Value.Take(50).ToCsvString("0.000"));
                    gen = nextGen;
                }

                var res = EntityOps.SaveEntityGen(repo, gen, "ralph");
                return Rop.ExtractResult(res).Value;
            }
        }

        static DataRecord RmgDataRecord(IEntityRepo repo, int rowCount, int colCount, string entityName)
        {
            var gen = MakeRmg(rowCount: rowCount, colCount: colCount);
            var entRes = EntityOps.SaveEntityGen(repo, gen, entityName);
            var ent = Rop.ExtractResult(entRes).Value;
            var drR = EntityOps.GetResultDataRecord(repo, ent, Entvert.ToEpn("Matrix"));
            var dr = Rop.ExtractResult(drR).Value;
            return dr;
        }

        static IEntityGen MakeRmg(int rowCount, int colCount)
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

    }
}
