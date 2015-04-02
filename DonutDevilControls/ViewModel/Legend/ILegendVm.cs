using System;

namespace DonutDevilControls.ViewModel.Legend
{
    public interface ILegendVm
    {
        LegendType LegendType { get; }

        System.Windows.Media.Color ColorForInterval(float val);

        System.Windows.Media.Color ColorFor1D(float val);

        System.Windows.Media.Color ColorFor2D(float xVal, float yVal);

        IObservable<ILegendVm> OnLegendVmChanged { get; }

    }
}
