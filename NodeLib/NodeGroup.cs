using System;
using System.Collections.Generic;
using System.Linq;
using MathLib.NumericTypes;

namespace NodeLib
{
    public interface INodeGroup
    {
        IReadOnlyList<float> Values { get; }
        int Generation { get; }
    }

    public static class NodeGroup
    {
        public static INodeGroup RandomNodeGroupUnitR(
            int nodeCount,
            int seed
        )
        {
            var randy = new Random(seed);
            return new NodeGroupImpl
                (
                    nodes: Enumerable.Range(0, nodeCount)
                              .Select(i=> Node.Make((float)randy.NextDouble(), groupIndex: i)),
                    nodeCount: nodeCount,
                    generation: 0
                );
        }


        public static INodeGroup RandomNodeGroupUnitZ(
            int nodeCount,
            int seed
        )
        {
            var randy = new Random(seed);
            return new NodeGroupImpl
                (
                    nodes: Enumerable.Range(0, nodeCount)
                              .Select(i => Node.Make((float)randy.NextDouble() * 2 - 1.0f, groupIndex: i)),
                    nodeCount: nodeCount,
                    generation: 0
                );
        }
        public static INodeGroup TorusPeriodicNodeGroup(int squareSize, float waviness)
        {
            var nodes = new INode[squareSize * squareSize * 2];

            for (var i = 0; i < squareSize; i++)
            {
                for (var j = 0; j < squareSize; j++)
                {
                    nodes[i * squareSize + j] = Node.Make
                        (
                            value: Mf.MfSin( (i*waviness)/squareSize),
                            groupIndex: i * squareSize + j
                        );

                    nodes[squareSize * squareSize + i * squareSize + j] = Node.Make
                        (
                            value: Mf.MfSin((j * waviness) / squareSize),
                            groupIndex: squareSize * squareSize + i * squareSize + j
                        );
                }
            }

            return new NodeGroupImpl
                (
                    nodes: nodes,
                    nodeCount: squareSize * squareSize * 2,
                    generation:0
                );
        }

        public static INodeGroup ToNodeGroup
            (
                this IEnumerable<INode> nodes, 
                int nodeCount,
                int generation
            )
        {
            return new NodeGroupImpl(nodes, nodeCount, generation);
        }

        public static IEnumerable<INode> Nodes(this INodeGroup nodeGroup)
        {
            return nodeGroup.Values.Select(Node.Make);
        }

        public static double Activity(this INodeGroup lhs, INodeGroup rhs)
        {
            return 
                Enumerable.Range(0, lhs.Values.Count).Average(
                        i => lhs.Values[i].MfAbsDeltaAsFloat(rhs.Values[i])
                    );
        }
    }

    public class NodeGroupImpl : INodeGroup
    {
        public NodeGroupImpl(IEnumerable<INode> nodes, int nodeCount, int generation)
        {
            _generation = generation;
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

        private readonly int _generation;
        public int Generation
        {
            get { return _generation; }
        }
    }
}
