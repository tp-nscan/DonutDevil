using System;
using System.Collections.Generic;
using NodeLib.Indexers;
using WpfUtils;

namespace DonutDevilControls.ViewModel.D2Indexer
{
    public class LayerCorrelationVm : NotifyPropertyChanged
    {
        public LayerCorrelationVm(
            string name,
            D2IndexerBase<float> master,
            IEnumerable<D2IndexerBase<float>> slaves)
        {
            _name = name;
            _masterId2Indexer = master;
            foreach (var slave in slaves)
            {
                var slave1 = slave;
                _correlationVms.Add
                    (
                        new CorrelationVm
                            (
                              name: slave.Name,
                              id2Indexer: slave1
                            )
                    );
            }
        }

        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }

        private readonly D2IndexerBase<float> _masterId2Indexer;
        public D2IndexerBase<float> MasterId2Indexer
        {
            get { return _masterId2Indexer; }
        }

        public void UpdateCorrelations(Func<D2IndexerBase<float>, D2IndexerBase<float>, double> correlationFunc)
        {
            foreach (var correlationVm in CorrelationVms)
            {
                correlationVm.Correlation = correlationFunc(MasterId2Indexer, correlationVm.Id2Indexer);
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
