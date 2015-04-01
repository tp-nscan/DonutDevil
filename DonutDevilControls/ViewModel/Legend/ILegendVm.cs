using System;
using WpfUtils.Utils;

namespace DonutDevilControls.ViewModel.Legend
{
    public interface ILegendVm
    {
        DisplaySpaceType DisplaySpaceType { get; }

        System.Windows.Media.Color ColorForInterval(float val);

        System.Windows.Media.Color ColorForRing(float val);

        System.Windows.Media.Color ColorForTorus(float xVal, float yVal);

        IObservable<ILegendVm> OnLegendVmChanged { get; }

    }
}
