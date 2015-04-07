using System;
using NodeLib.Indexers;
using WpfUtils;

namespace DonutDevilControls.ViewModel.NgIndexer
{
    public class CorrelationVm : NotifyPropertyChanged
    {
        public CorrelationVm(string name, INgIndexer ngIndexer)
        {
            _name = name;
            _ngIndexer = ngIndexer;
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

        private readonly INgIndexer _ngIndexer;
        public INgIndexer NgIndexer
        {
            get { return _ngIndexer; }
        }
    }
}
