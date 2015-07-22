using System.Collections.Generic;
using LibNode;
using WpfUtils;

namespace La.ViewModel
{
    public class WaffleHistoriesVm : NotifyPropertyChanged
    {
        public WaffleHistoriesVm(WaffleHistories waffleHistories)
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
            ArrayHistVm = ArrayHistVms[0];
        }

        WaffleHistories WaffleHistories { get; }

        public IList<ArrayHistVm> ArrayHistVms { get; }

        public ArrayHistVm ArrayHistVm { get; set; }
    }

    public class ArrayHistVm
    {
        public ArrayHistVm(ArrayHist arrayHist)
        {
            ArrayHist = arrayHist;
        }

        ArrayHist ArrayHist { get; }

        public string Name => ArrayHist.Name;

        public IEnumerable<D2Val<float>> GetD2Vals => ArrayHistory.GetD2Vals(ArrayHist);
    }
}
