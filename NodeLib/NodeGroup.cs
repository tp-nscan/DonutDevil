using System;
using System.Collections.Generic;
using System.Linq;

namespace NodeLib
{
    public interface INodeGroup
    {
        IReadOnlyList<float> Nodes { get; }
    }

    public static class NodeGroup
    {
        public static INodeGroup RandomNodeGroup(
            int nodeCount,
            int seed
        )
        {
            var randy = new Random(seed);
            return new NodeGroupImpl
                (
                    nodes: Enumerable.Range(0, nodeCount)
                              .Select(i=> Node.Make((float) (randy.NextDouble()*2 -1), groupIndex: i)),
                    nodeCount: nodeCount          
                );
        }

        public static INodeGroup ToNodeGroup
            (
                this IEnumerable<INode> nodes, 
                int nodeCount
            )
        {
            return new NodeGroupImpl(nodes, nodeCount);
        }
    }

    public class NodeGroupImpl : INodeGroup
    {
        public NodeGroupImpl(IEnumerable<INode> nodes, int nodeCount)
        {
            _nodes = new float[nodeCount];
            foreach (var node in nodes)
            {
                _nodes[node.GroupIndex] = node.Value;
            }
        }

        private readonly float[] _nodes;
        public IReadOnlyList<float> Nodes
        {
            get { return _nodes; }
        }
    }
}
