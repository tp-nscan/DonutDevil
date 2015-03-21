using NodeLib.Indexers;

namespace DonutDevilControls.ViewModel.NgIndexer
{
    public enum NgDisplayMode
    {
        Ring,
        Torus
    }

    public interface INgDisplayIndexing
    {
        NgDisplayMode NgisDisplayMode { get; }
        INgIndexer IndexerRing { get; }
        INgIndexer IndexerTorusX { get; }
        INgIndexer IndexerTorusY { get; }
    }


    public static class NgDisplayState
    {
        public static INgDisplayIndexing RingIndexing(INgIndexer ngIndexer)
        {
            return new NgDisplayIndexingImpl(NgDisplayMode.Ring, indexerRing: ngIndexer, indexerTorusX:null, indexerTorusY:null);
        }

        public static INgDisplayIndexing TorusIndexing(INgIndexer ngIndexerX, INgIndexer ngIndexerY)
        {
            return new NgDisplayIndexingImpl(NgDisplayMode.Torus, indexerRing: null, indexerTorusX:ngIndexerX, indexerTorusY:ngIndexerY);
        }

    }

    public class NgDisplayIndexingImpl : INgDisplayIndexing
    {
        private readonly NgDisplayMode _ngisDisplayMode;
        private readonly INgIndexer _indexerRing;
        private readonly INgIndexer _indexerTorusX;
        private readonly INgIndexer _indexerTorusY;

        public NgDisplayIndexingImpl(NgDisplayMode ngisDisplayMode, 
                                INgIndexer indexerRing, 
                                INgIndexer indexerTorusX, 
                                INgIndexer indexerTorusY)
        {
            _ngisDisplayMode = ngisDisplayMode;
            _indexerRing = indexerRing;
            _indexerTorusX = indexerTorusX;
            _indexerTorusY = indexerTorusY;
        }

        public NgDisplayMode NgisDisplayMode
        {
            get { return _ngisDisplayMode; }
        }

        public INgIndexer IndexerRing
        {
            get { return _indexerRing; }
        }

        public INgIndexer IndexerTorusX
        {
            get { return _indexerTorusX; }
        }

        public INgIndexer IndexerTorusY
        {
            get { return _indexerTorusY; }
        }
    }

}
