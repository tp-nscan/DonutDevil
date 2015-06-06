using System;
using LibNode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NodeLib.Test
{
    [TestClass]
    public class TestNetwork
    {

        [TestMethod]
        public void TestInitCesrDto()
        {
            var ubA = Parameters.RandomCliqueSet;

            var res = CesrBuilder.CreateDtoFromRandomParams(ubA);

            Assert.IsTrue(res.IsSuccess);
        }

        [TestMethod]
        public void TestMakeSimpleNetwork()
        {
            var ubA = Parameters.RandomCliqueSet;
            var res = CesrBuilder.MakeSimpleNetworkFromRandomParams(ubA);
            var res2 = (ISymState)Rop.ExtractResult(res).Value;
            var res3 = Rop.ExtractResult(res2.Update()).Value;

            Assert.IsTrue(res3 != null);
        }
    }
}
