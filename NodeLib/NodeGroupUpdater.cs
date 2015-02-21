using System;
using System.Collections.Generic;
using System.Linq;

namespace NodeLib
{
    public interface INodeGroupUpdater
    {
        INodeGroup Update(INodeGroup nodeGroup);
    }




    public class NodeGroupUpdaterImpl : INodeGroupUpdater
    {
        public NodeGroupUpdaterImpl(
            IEnumerable<Func<INodeGroup, INode[]>> updateFunctions
            )
        {
            _updateFunctions = updateFunctions.ToArray();
        }

        public INodeGroup Update(INodeGroup nodeGroup)
        {
            return _updateFunctions
                        .AsParallel()
                        .SelectMany(n => n(nodeGroup))
                        .ToNodeGroup(nodeGroup.Values.Count);
        }

        private readonly Func<INodeGroup, INode[]>[] _updateFunctions;

    }
}