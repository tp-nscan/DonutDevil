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
        ID2Indexer<float> Indexer1D { get; }
        ID2Indexer<float> Indexer2Dx { get; }
        ID2Indexer<float> Indexer2Dy { get; }
    }


    public static class NgDisplayState
    {
        public static INgDisplayIndexing RingIndexing(ID2Indexer<float> id2Indexer)
        {
            return new NgDisplayIndexingImpl(LegendMode.OneLayer, indexer1D: id2Indexer, indexer2Dx:null, indexer2Dy:null);
        }

        public static INgDisplayIndexing TorusIndexing(ID2Indexer<float> id2IndexerX, ID2Indexer<float> id2IndexerY)
        {
            return new NgDisplayIndexingImpl(LegendMode.TwoLayers, indexer1D: null, indexer2Dx:id2IndexerX, indexer2Dy:id2IndexerY);
        }

    }

    public class NgDisplayIndexingImpl : INgDisplayIndexing
    {
        private readonly LegendMode _legendMode;
        private readonly ID2Indexer<float> _indexer1D;
        private readonly ID2Indexer<float> _indexer2Dx;
        private readonly ID2Indexer<float> _indexer2Dy;

        public NgDisplayIndexingImpl(LegendMode legendMode,
                                ID2Indexer<float> indexer1D,
                                ID2Indexer<float> indexer2Dx,
                                ID2Indexer<float> indexer2Dy)
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

        public ID2Indexer<float> Indexer1D
        {
            get { return _indexer1D; }
        }

        public ID2Indexer<float> Indexer2Dx
        {
            get { return _indexer2Dx; }
        }

        public ID2Indexer<float> Indexer2Dy
        {
            get { return _indexer2Dy; }
        }
    }

}
