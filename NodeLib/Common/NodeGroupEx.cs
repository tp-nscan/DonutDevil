using System;
using System.Collections.Generic;
using System.Linq;
using LibNode;
using MathLib.NumericTypes;

namespace NodeLib.Common
{
    public static class NodeGroupEx
    {
        public static NodeGroup RandomNodeGroupUnitR
        (
            int nodeCount,
            int seed
        )
        {
            var randy = new Random(seed);

            var ng = new NodeGroup(nodeCount);
            ng.AddNodes(Enumerable.Range(0, nodeCount)
                              .Select(i => new Node((float)randy.NextDouble(), groupIndex: i)));
            return ng;
        }

        public static NodeGroup RandomNodeGroupUnitZ
        (
            int nodeCount,
            int seed
        )
        {
            var randy = new Random(seed);

            var ng = new NodeGroup(nodeCount);
            ng.AddNodes(Enumerable.Range(0, nodeCount)
                              .Select(i => new Node((float)randy.NextDouble() * 2 - 1.0f, groupIndex: i)));
            return ng;
        }


        public static NodeGroup TorusPeriodicNodeGroup(int squareSize, float waviness)
        {
            var nodes = new Node[squareSize * squareSize * 2];

            for (var i = 0; i < squareSize; i++)
            {
                for (var j = 0; j < squareSize; j++)
                {
                    nodes[i * squareSize + j] = new Node
                        (
                            value: Mf.MfSin( (i*waviness)/squareSize),
                            groupIndex: i * squareSize + j
                        );

                    nodes[squareSize * squareSize + i * squareSize + j] = new Node
                        (
                            value: Mf.MfSin((j * waviness) / squareSize),
                            groupIndex: squareSize * squareSize + i * squareSize + j
                        );
                }
            }

            var ng = new NodeGroup(squareSize * squareSize * 2);
            ng.AddNodes(nodes);
            return ng;
        }

        public static NodeGroup ToNodeGroup
            (
                this IEnumerable<Node> nodes, 
                int nodeCount
            )
        {
            var ng = new NodeGroup(nodeCount);
            ng.AddNodes(nodes);
            return ng;
        }

        public static IEnumerable<Node> Nodes(this NodeGroup nodeGroup)
        {
            return nodeGroup.Values.Select((v,i) => new Node(v,i));
        }

        public static double Activity(this NodeGroup lhs, NodeGroup rhs)
        {
            return 
                Enumerable.Range(0, lhs.Values.Length).Average(
                        i => lhs.Values[i].MfAbsDeltaAsFloat(rhs.Values[i])
                    );
        }
    }
}
