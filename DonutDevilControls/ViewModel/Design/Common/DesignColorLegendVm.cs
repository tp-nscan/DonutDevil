using DonutDevilControls.ViewModel.Common;
using WpfUtils.Views.Graphics;

namespace DonutDevilControls.ViewModel.Design.Common
{
    public class DesignColorLegendVm : Plot1DVm
    {
        public DesignColorLegendVm()
        {
            Title = "Designer title";
            MinValue = -1.0;
            MaxValue = 1.0;
            GraphicsInfos = PlotPointEx.TestSequence();
        }
    }
}
