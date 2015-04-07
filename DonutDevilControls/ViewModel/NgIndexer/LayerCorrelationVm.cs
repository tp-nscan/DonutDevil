using System;
using System.Collections.Generic;
using NodeLib.Indexers;
using WpfUtils;

namespace DonutDevilControls.ViewModel.NgIndexer
{
    public class LayerCorrelationVm : NotifyPropertyChanged
    {
        public LayerCorrelationVm(
            string name,
            INgIndexer master, 
            IEnumerable<INgIndexer> slaves)
        {
            _name = name;
            _masterNgIndexer = master;
            foreach (var slave in slaves)
            {
                var slave1 = slave;
                _correlationVms.Add
                    (
                        new CorrelationVm
                            (
                              name: slave.Name,
                              ngIndexer: slave1
                            )
                    );
            }
        }

        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }

        private readonly INgIndexer _masterNgIndexer;
        public INgIndexer MasterNgIndexer
        {
            get { return _masterNgIndexer; }
        }

        public void UpdateCorrelations(Func<INgIndexer, INgIndexer, double> correlationFunc)
        {
            foreach (var correlationVm in CorrelationVms)
            {
                correlationVm.Correlation = correlationFunc(MasterNgIndexer, correlationVm.NgIndexer);
            }
        }

        private IList<CorrelationVm> _correlationVms = new List<CorrelationVm>();
        public IList<CorrelationVm> CorrelationVms
        {
            get { return _correlationVms; }
            set { _correlationVms = value; }
        }
    }
}
