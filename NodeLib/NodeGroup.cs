using System;
using System.Collections.Generic;
using System.Linq;

namespace NodeLib
{
    public interface INodeGroup
    {
        IReadOnlyList<float> Values { get; }
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
                              .Select(i=> Node.Make((float)randy.NextDouble(), groupIndex: i)),
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

        public static IEnumerable<INode> Nodes(this INodeGroup nodeGroup)
        {
            return nodeGroup.Values.Select(Node.Make);
        }
    }

    public class NodeGroupImpl : INodeGroup
    {
        public NodeGroupImpl(IEnumerable<INode> nodes, int nodeCount)
        {
            _values = new float[nodeCount];
            foreach (var node in nodes)
            {
                _values[node.GroupIndex] = node.Value;
            }
        }

        private readonly float[] _values;
        public IReadOnlyList<float> Values
        {
            get { return _values; }
        }
    }
}
