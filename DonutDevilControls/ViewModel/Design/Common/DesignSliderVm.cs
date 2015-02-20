using DonutDevilControls.ViewModel.Common;
using MathLib.Intervals;

namespace DonutDevilControls.ViewModel.Design.Common
{
    public class DesignSliderVm : SliderVm
    {
        public DesignSliderVm()
            : base(RealInterval.Make(-3.464, 7), 0.1, "0.00")
        {
            Title = "The control title";

        }
    }
}
