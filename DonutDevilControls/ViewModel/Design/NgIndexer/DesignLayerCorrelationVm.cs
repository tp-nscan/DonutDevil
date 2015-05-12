using System.Collections.Generic;
using System.Linq;
using DonutDevilControls.ViewModel.D2Indexer;
using NodeLib;
using NodeLib.Indexers;

namespace DonutDevilControls.ViewModel.Design.NgIndexer
{
    public class DesignLayerCorrelationVm : LayerCorrelationVm
    {
        public DesignLayerCorrelationVm()

            : base("Designer name", MasterIndexer, DesignNgIndexers)
        {
        }

        static IEnumerable<D2IndexerBase<float>> DesignNgIndexers
        {
            get
            {

                yield return NodeLib.Indexers.D2Indexer.MakeRingArray2D("Mem 1", 5);
                yield return NodeLib.Indexers.D2Indexer.MakeRingArray2D("Mem 2", 5);
                yield return NodeLib.Indexers.D2Indexer.MakeRingArray2D("Mem 3", 5);
                yield return NodeLib.Indexers.D2Indexer.MakeRingArray2D("Mem 4", 5);
                yield return NodeLib.Indexers.D2Indexer.MakeRingArray2D("Mem 5", 5);
            }
        }

        static D2IndexerBase<float> MasterIndexer
        {
            get { return NodeLib.Indexers.D2Indexer.MakeRingArray2D("Master", 5); }
        }

        private const int NodesPerLayer = 25;
        private const int LayerCount = 6;

        public static INodeGroup DesignNodeGroup = new NodeGroupImpl(
                    nodes: Enumerable.Range(0, NodesPerLayer * LayerCount)
                              .Select(i => Node.Make((2.0f * i) / (NodesPerLayer * LayerCount) - 1.0f, groupIndex: i)),
                    nodeCount: NodesPerLayer * LayerCount,
                    generation: 0

        );



    }
}
