using System;
using System.Collections.Generic;
using System.Linq;
using NodeLib.Params;

namespace NodeLib.Updaters
{
    public interface INgUpdater
    {
        INodeGroup Update(INodeGroup nodeGroup);
        string Name { get; }
    }

    public static class NgUpdater
    {
        public static INgUpdater ForStandardRing(int squareSize, int arrayOffset)
        {
            return NgUpdaterBuilder.ForStandardRing
                (
                    squareSize: squareSize,
                    arrayOffset: arrayOffset
                )(ParamSets.StandardRingParams());
        }

        public static INgUpdater ForDoubleRing(int squareSize, int arrayOffset)
        {
            return NgUpdaterBuilder.ForStandardTorus
                (
                    squareSize: squareSize,
                    arrayOffset: arrayOffset
                )(ParamSets.DoubleRingParams());
        }

    }

    public class NgUpdaterImpl : INgUpdater
    {
        public NgUpdaterImpl(
            string name,
            IEnumerable<Func<INodeGroup, INode[]>> updateFunctions
        )
        {
            _name = name;
            _updateFunctions = updateFunctions.ToArray();
        }

        public INodeGroup Update(INodeGroup nodeGroup)
        {
            return _updateFunctions
                        .AsParallel()
                        .SelectMany(n => n(nodeGroup))
                        .ToNodeGroup(nodeGroup.Values.Count, nodeGroup.Generation + 1);
        }

        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }

        private readonly Func<INodeGroup, INode[]>[] _updateFunctions;
    }
}