using NodeLib.Indexers;

namespace DonutDevilControls.ViewModel.NgIndexer
{
    public enum NgDisplayMode
    {
        Ring,
        Torus
    }

    public interface INgDisplayState
    {
        NgDisplayMode NgisDisplayMode { get; }
        INgIndexer IndexerRing { get; }
        INgIndexer IndexerTorusX { get; }
        INgIndexer IndexerTorusY { get; }
    }


    public static class NgDisplayState
    {
        public static INgDisplayState RingState(INgIndexer ngIndexer)
        {
            return new NgDisplayStateImpl(NgDisplayMode.Ring, indexerRing: ngIndexer, indexerTorusX:null, indexerTorusY:null);
        }

        public static INgDisplayState TorusState(INgIndexer ngIndexerX, INgIndexer ngIndexerY)
        {
            return new NgDisplayStateImpl(NgDisplayMode.Ring, indexerRing: null, indexerTorusX:ngIndexerX, indexerTorusY:ngIndexerY);
        }

    }

    public class NgDisplayStateImpl : INgDisplayState
    {
        private readonly NgDisplayMode _ngisDisplayMode;
        private readonly INgIndexer _indexerRing;
        private readonly INgIndexer _indexerTorusX;
        private readonly INgIndexer _indexerTorusY;

        public NgDisplayStateImpl(NgDisplayMode ngisDisplayMode, 
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
