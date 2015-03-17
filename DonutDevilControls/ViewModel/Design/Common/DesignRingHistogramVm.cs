using System;
using System.Linq;
using System.Windows.Media;
using DonutDevilControls.ViewModel.Common;
using MathLib;
using MathLib.NumericTypes;
using WpfUtils.Utils;
using WpfUtils.ViewModels.Graphics;

namespace DonutDevilControls.ViewModel.Design.Common
{
    public class DesignRingHistogramVm : RingHistogramVm
    {
        public DesignRingHistogramVm() 
            : base("Designer title")
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
