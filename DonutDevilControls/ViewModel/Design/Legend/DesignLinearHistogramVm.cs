using System;
using System.Linq;
using System.Windows.Media;
using DonutDevilControls.ViewModel.Legend;
using MathLib;
using WpfUtils.Utils;

namespace DonutDevilControls.ViewModel.Design.Legend
{
    public class DesignLinearHistogramVm : LinearHistogramVm
    {
        public DesignLinearHistogramVm() : base("Designer title", 0.0, 1.0)
        {
            var legendColorSequence = ColorSequence.Quadrupolar(Colors.Red, Colors.Orange, Colors.Green, Colors.Blue, Colorsteps / 4);
            var histogramColorSequence = Colors.White.ToUniformColorSequence(Functions.TrigFuncSteps);
            var randy = new Random();

            DrawLegend(i => legendColorSequence.Colors[(int)(i*Colorsteps)]);

            MakeHistogram(Enumerable.Range(0, Functions.TrigFuncSteps).Select(i => (float) randy.NextDouble()));
        }
    }
}
