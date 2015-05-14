using System;
using System.Collections.Generic;
using System.Linq;
using NodeLib.Common;
using LibNode;

namespace NodeLib.NgUpdaters
{
    public interface INgUpdater
    {
        NodeGroup Update(NodeGroup nodeGroup);
        string Name { get; }
    }


    public class NgUpdaterImpl : INgUpdater
    {
        public NgUpdaterImpl(
            string name,
            IEnumerable<Func<NodeGroup, Node[]>> updateFunctions
        )
        {
            _name = name;
            _updateFunctions = updateFunctions.ToArray();
        }

        public NodeGroup Update(NodeGroup nodeGroup)
        {
            return _updateFunctions
                        .AsParallel()
                        .SelectMany(n => n(nodeGroup))
                        .ToNodeGroup(nodeGroup.Values.Length);
        }

        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }

        private readonly Func<NodeGroup, Node[]>[] _updateFunctions;
    }
}