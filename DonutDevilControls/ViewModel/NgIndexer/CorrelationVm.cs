using NodeLib.Indexers;
using WpfUtils;

namespace DonutDevilControls.ViewModel.NgIndexer
{
    public class CorrelationVm : NotifyPropertyChanged
    {
        public CorrelationVm(string name, ID2Indexer<float> id2Indexer)
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

        private readonly ID2Indexer<float> _id2Indexer;
        public ID2Indexer<float> Id2Indexer
        {
            get { return _id2Indexer; }
        }
    }
}
