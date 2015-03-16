using NodeLib.Indexers;

namespace DonutDevilControls.ViewModel.NgIndexer
{
    public enum NgisDisplayMode
    {
        Ring,
        Torus
    }

    public interface INgisDisplayState
    {
        NgisDisplayMode NgisDisplayMode { get; }
        INgIndexer IndexerRing { get; }
        INgIndexer IndexerTorusX { get; }
        INgIndexer IndexerTorusY { get; }
    }


    public static class NgisDisplayState
    {
        public static INgisDisplayState RingState(INgIndexer ngIndexer)
        {
            return new NgisDisplayStateImpl(NgisDisplayMode.Ring, indexerRing: ngIndexer, indexerTorusX:null, indexerTorusY:null);
        }

        public static INgisDisplayState TorusState(INgIndexer ngIndexerX, INgIndexer ngIndexerY)
        {
            return new NgisDisplayStateImpl(NgisDisplayMode.Ring, indexerRing: null, indexerTorusX:ngIndexerX, indexerTorusY:ngIndexerY);
        }

    }

    public class NgisDisplayStateImpl : INgisDisplayState
    {
        private readonly NgisDisplayMode _ngisDisplayMode;
        private readonly INgIndexer _indexerRing;
        private readonly INgIndexer _indexerTorusX;
        private readonly INgIndexer _indexerTorusY;

        public NgisDisplayStateImpl(NgisDisplayMode ngisDisplayMode, 
                                INgIndexer indexerRing, 
                                INgIndexer indexerTorusX, 
                                INgIndexer indexerTorusY)
        {
            _ngisDisplayMode = ngisDisplayMode;
            _indexerRing = indexerRing;
            _indexerTorusX = indexerTorusX;
            _indexerTorusY = indexerTorusY;
        }

        public NgisDisplayMode NgisDisplayMode
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
