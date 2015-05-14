using Microsoft.VisualStudio.TestTools.UnitTesting;
using LibNode;

namespace NodeLib.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            const int arraylength = 5;
            var foo = new NodeGroup(arraylength);

            var nodes = new[] { new Node(1.2f, 3), new Node(1.7f, 4) };
            foo.AddNodes(nodes);

            Assert.AreEqual(arraylength, foo.ArrayLength);

            Assert.AreEqual(arraylength, foo.Values.Length);
        }
    }
}
