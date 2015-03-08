using System.Linq;
using DonutDevilControls.ViewModel.Common;
using WpfUtils.Utils;
using WpfUtils.Views.Graphics;

namespace DonutDevilControls.ViewModel.Design.Common
{
    public class DesignPlot2DVm : Plot2DVm
    {
        public DesignPlot2DVm()
        {
            Title = "Designer title";
            MinValueX = -1.0;
            MaxValueX = 1.0;
            MinValueY = -1.0;
            MaxValueY = 1.0;
            GraphicsInfos = PlotPointEx.TestSequence();
        }

    }
}
