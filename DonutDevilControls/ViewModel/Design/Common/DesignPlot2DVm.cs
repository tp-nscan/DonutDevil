using System.Linq;
using System.Windows.Media;
using DonutDevilControls.ViewModel.Common;
using DonutDevilControls.ViewModel.Legend;
using MathLib.NumericTypes;
using WpfUtils.Views.Graphics;

namespace DonutDevilControls.ViewModel.Design.Common
{
    public class DesignPlot2DVm : Plot2DVm
    {
        public DesignPlot2DVm()
            : base(128, 128)
        {
            Title = "Designer title";
            MinValueX = -1.0;
            MaxValueX = 1.0;
            MinValueY = -1.0;
            MaxValueY = 1.0;
            WbUniformGridVm.AddValues(
                PlotPointEx.TestSequence()
                    .Select(pp=>new D2Val<Color>(pp.X,pp.Y,pp.Color))
                );
        }

    }
}
