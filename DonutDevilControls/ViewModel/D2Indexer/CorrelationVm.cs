using NodeLib.Indexers;
using WpfUtils;

namespace DonutDevilControls.ViewModel.D2Indexer
{
    public class CorrelationVm : NotifyPropertyChanged
    {
        public CorrelationVm(string name, D2IndexerBase<float> id2Indexer)
        {
            _name = name;
            _id2Indexer = id2Indexer;
        }

        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }

        private double _correlation;
        public double Correlation
        {
            get { return _correlation; }
            set
            {
                _correlation = value;
                OnPropertyChanged("Correlation");
            }
        }

        private readonly D2IndexerBase<float> _id2Indexer;
        public D2IndexerBase<float> Id2Indexer
        {
            get { return _id2Indexer; }
        }
    }
}
