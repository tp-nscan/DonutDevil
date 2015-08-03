using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using LibNode;
using WpfUtils;

namespace La.ViewModel
{
    public class ZeusHistoriesVm : NotifyPropertyChanged
    {
        public ZeusHistoriesVm(WaffleHistories waffleHistories, string curName)
        {
            ZeusHistories = waffleHistories;
            ArrayHistVms = new List<ArrayHistVm>
            {
                new ArrayHistVm(ZeusHistories.aeR),
                new ArrayHistVm(ZeusHistories.ahA),
                new ArrayHistVm(ZeusHistories.ahB),
                new ArrayHistVm(ZeusHistories.ahR),
                new ArrayHistVm(ZeusHistories.ahS),
                new ArrayHistVm(ZeusHistories.ahV)
            };
            ArrayHistVm = ArrayHistVms.Single(h => h.Name == curName);
        }

        public WaffleHistories ZeusHistories { get; }

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

        public ZeusHistoriesVm Update(Wng wng, Waffle waffle)
        {
            return new ZeusHistoriesVm(
                waffleHistories: WngBuilder.UpdateHistories(ZeusHistories, wng, waffle),
                curName: ArrayHistVm.Name
                );
        }

        private readonly Subject<ZeusHistoriesVm> _arrayHistVmChanged
                = new Subject<ZeusHistoriesVm>();

        private ArrayHistVm _arrayHistVm;

        public IObservable<ZeusHistoriesVm> OnArrayHistVmChanged
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
