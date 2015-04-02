using NodeLib.Indexers;

namespace DonutDevilControls.ViewModel.Legend
{
    public enum LegendMode
    {
        OneLayer,
        TwoLayers
    }

    public interface INgDisplayIndexing
    {
        LegendMode LegendMode { get; }
        INgIndexer Indexer1D { get; }
        INgIndexer Indexer2Dx { get; }
        INgIndexer Indexer2Dy { get; }
    }


    public static class NgDisplayState
    {
        public static INgDisplayIndexing RingIndexing(INgIndexer ngIndexer)
        {
            return new NgDisplayIndexingImpl(LegendMode.OneLayer, indexer1D: ngIndexer, indexer2Dx:null, indexer2Dy:null);
        }

        public static INgDisplayIndexing TorusIndexing(INgIndexer ngIndexerX, INgIndexer ngIndexerY)
        {
            return new NgDisplayIndexingImpl(LegendMode.TwoLayers, indexer1D: null, indexer2Dx:ngIndexerX, indexer2Dy:ngIndexerY);
        }

    }

    public class NgDisplayIndexingImpl : INgDisplayIndexing
    {
        private readonly LegendMode _legendMode;
        private readonly INgIndexer _indexer1D;
        private readonly INgIndexer _indexer2Dx;
        private readonly INgIndexer _indexer2Dy;

        public NgDisplayIndexingImpl(LegendMode legendMode, 
                                INgIndexer indexer1D, 
                                INgIndexer indexer2Dx, 
                                INgIndexer indexer2Dy)
        {
            _legendMode = legendMode;
            _indexer1D = indexer1D;
            _indexer2Dx = indexer2Dx;
            _indexer2Dy = indexer2Dy;
        }

        public LegendMode LegendMode
        {
            get { return _legendMode; }
        }

        public INgIndexer Indexer1D
        {
            get { return _indexer1D; }
        }

        public INgIndexer Indexer2Dx
        {
            get { return _indexer2Dx; }
        }

        public INgIndexer Indexer2Dy
        {
            get { return _indexer2Dy; }
        }
    }

}
