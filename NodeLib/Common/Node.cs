using System.Collections.Generic;
using System.Linq;
using LibNode;

namespace NodeLib.Common
{
    public static class SNode
    {
        //public static Node Make(float value, int groupIndex)
        //{
        //    return new Node(value, groupIndex);
        //}

        public static IEnumerable<Node> ToNodes(this IEnumerable<float> values, int startingIndex)
        {
            return values.Select(val => new Node(val, startingIndex++));
        }

        public static IEnumerable<Node> ToNodes(this IEnumerable<double> values, int startingIndex)
        {
            return values.Select(val => new Node((float)val, startingIndex++));
        }
    }

}
