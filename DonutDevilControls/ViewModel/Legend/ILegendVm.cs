using System;
using WpfUtils.Utils;

namespace DonutDevilControls.ViewModel.Legend
{
    public interface ILegendVm
    {
        DisplaySpaceType DisplaySpaceType { get; }

        System.Windows.Media.Color ColorForUnitInterval(float val);

        System.Windows.Media.Color ColorForUnitRing(float val);

        System.Windows.Media.Color ColorForUnitTorus(float xVal, float yVal);

        IObservable<ILegendVm> OnLegendVmChanged { get; }

    }
}
