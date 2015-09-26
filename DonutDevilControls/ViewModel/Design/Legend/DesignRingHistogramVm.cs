using System;
using System.Linq;
using System.Windows.Media;
using DonutDevilControls.ViewModel.Legend;
using LibNode;
using MathLib;
using MathLib.NumericTypes;
using WpfUtils.Utils;

namespace DonutDevilControls.ViewModel.Design.Legend
{
    public class DesignRingHistogramVm : RingHistogramVm
    {
        public DesignRingHistogramVm() 
            : base("Designer title", 0.0, 1.0)
        {
            var legendColorSequence = ColorSequence.Quadrupolar(Colors.Red, Colors.Orange, Colors.Green, Colors.Blue, Functions.TrigFuncSteps / 4);
            var histogramColorSequence = Colors.White.ToUniformColorSequence(Functions.TrigFuncSteps);
            var randy = new Random();

            LegendVm.AddValues(
                    Enumerable.Range(0, Functions.TrigFuncSteps)
                              .Select(i => new D1Val<Color>(i, legendColorSequence.ToUnitColor((float)i / Functions.TrigFuncSteps)))
                );

            HistogramVm.AddValues(
                    Enumerable.Range(0, Functions.TrigFuncSteps)
                              .Select(i => new D1Val<Color>(i, histogramColorSequence.ToUnitColor((float)randy.NextDouble())))
                );
        }
    }
}
