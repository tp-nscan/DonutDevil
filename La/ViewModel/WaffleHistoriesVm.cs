using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using LibNode;
using WpfUtils;

namespace La.ViewModel
{
    public class WaffleHistoriesVm : NotifyPropertyChanged
    {
        public WaffleHistoriesVm(WaffleHistories waffleHistories, string curName)
        {
            WaffleHistories = waffleHistories;
            ArrayHistVms = new List<ArrayHistVm>
            {
                new ArrayHistVm(WaffleHistories.aeR),
                new ArrayHistVm(WaffleHistories.ahA),
                new ArrayHistVm(WaffleHistories.ahB),
                new ArrayHistVm(WaffleHistories.ahR),
                new ArrayHistVm(WaffleHistories.ahS),
                new ArrayHistVm(WaffleHistories.ahV)
            };
            ArrayHistVm = ArrayHistVms.Single(h => h.Name == curName);
        }

        public WaffleHistories WaffleHistories { get; }

        public IList<ArrayHistVm> ArrayHistVms { get; }

        public ArrayHistVm ArrayHistVm
        {
            get { return _arrayHistVm; }
            set
            {
                _arrayHistVm = value;
                OnPropertyChanged("ArrayHistVm");
                _arrayHistVmChanged.OnNext(this);
            }
        }

        public WaffleHistoriesVm Update(Wng wng, Waffle waffle)
        {
            return new WaffleHistoriesVm(
                waffleHistories: WngBuilder.UpdateHistories(WaffleHistories, wng, waffle),
                curName: ArrayHistVm.Name
                );
        }

        private readonly Subject<WaffleHistoriesVm> _arrayHistVmChanged
                = new Subject<WaffleHistoriesVm>();

        private ArrayHistVm _arrayHistVm;

        public IObservable<WaffleHistoriesVm> OnArrayHistVmChanged
        {
            get { return _arrayHistVmChanged; }
        }
    }

    public class ArrayHistVm
    {
        public ArrayHistVm(ArrayHist arrayHist)
        {
            ArrayHist = arrayHist;
        }

        ArrayHist ArrayHist { get; }

        public int ArrayLength => ArrayHist.ArrayLength;

        public string Name => ArrayHist.Name;

        public IEnumerable<D2Val<float>> GetD2Vals => ArrayHistory.GetD2Vals(ArrayHist);
    }
}
