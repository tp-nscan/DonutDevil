using System;
using LibNode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NodeLib.Test
{
    [TestClass]
    public class TestNetwork
    {

        [TestMethod]
        public void TestRmgBuilder()
        {
            var entityGuid = Guid.NewGuid();
            var matrixGuid = Guid.NewGuid();
            const int rowCount = 10;
            const int colCount = 12;
            const int seed = 1234;
            const float maxValue = 0.3f;
            var ubA = Parameters.RandomMatrixSet(entityId:entityGuid, matrixId:matrixGuid, rowCount:rowCount, colCount:colCount, seed:seed, maxValue:maxValue);
            var res = RmgBuilder.CreateRandMatrixFromParams(ubA);
            var res1 = Rop.ExtractResult(res).Value;
            var res2 = (IEntityGen) res1;
            var res3 = res2.GetGenResult(EpnConvert.FromString("Matrix"));
            var res4 = Rop.ExtractResult(res3).Value;

            Assert.IsTrue(res2 != null);
        }

        [TestMethod]
        public void TestRmgBuilder2()
        {
            var entityGuid = Guid.NewGuid();
            var matrixGuid = Guid.NewGuid();
            const int rowCount = 10;
            const int colCount = 12;
            const int seed = 1234;
            const float maxValue = 0.3f;
            var ubA = Parameters.RandomMatrixSet(entityGuid, matrixGuid, rowCount, colCount, seed, maxValue);
            var res = RmgBuilder.CreateRandMatrixFromParams(ubA);
            var res1 = Rop.ExtractResult(res).Value;
            var res2 = (IEntityGen)res1;
            var res3 = res2.GetGenResult(EpnConvert.FromString("Matrix"));
            var res4 = Rop.ExtractResult(res3).Value;

            Assert.IsTrue(res2 != null);
        }
    }
}
