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
            : base("Designer title", LegendColorMap(), HistogramColorMap())
        {
            var randy = new Random();
            LegendVm.AddValues(
                    Enumerable.Range(0, Functions.TrigFuncSteps)
                              .Select(i => new D1Val<float>(i, (float)i / Functions.TrigFuncSteps))
                );

            HistogramVm.AddValues(
                    Enumerable.Range(0, Functions.TrigFuncSteps)
                              .Select(i => new D1Val<float>(i, (float)randy.NextDouble()))
                );
        }

        static Func<float, Color> LegendColorMap()
        {
            var nodeGroupColorSequence = ColorSequence.Quadrupolar(Colors.Red, Colors.Orange, Colors.Green, Colors.Blue, Functions.TrigFuncSteps / 4);
            return f => nodeGroupColorSequence.ToUnitColor(f);
        }

        static Func<float, Color> HistogramColorMap()
        {
            var nodeGroupColorSequence = Colors.White.ToUniformColorSequence(Functions.TrigFuncSteps);
            return f => nodeGroupColorSequence.ToUnitColor(f);
        }
    }
}
