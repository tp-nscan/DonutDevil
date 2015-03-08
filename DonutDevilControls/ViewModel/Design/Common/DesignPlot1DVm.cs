using System;
using System.Linq;
using System.Windows.Media;
using DonutDevilControls.ViewModel.Common;
using MathLib.NumericTypes;
using WpfUtils.Utils;

namespace DonutDevilControls.ViewModel.Design.Common
{
    public class DesignPlot1DVm : Plot1DVm
    {
        public DesignPlot1DVm()
            : base(LegendSteps, 0.2, ColorMap())
        {
            Title = "Designer title";
            MinValue = 0.0;
            MaxValue = 1.0;
            WbVerticalStripesVm.AddValues
                (
                    Enumerable.Range(0, LegendSteps)
                              .Select(i => new D2Val<float>(i, 0, i / (float)LegendSteps))
                );
        }

        static Func<float, Color> ColorMap()
        {
            var nodeGroupColorSequence = ColorSequence.Quadrupolar(Colors.Red, Colors.Orange, Colors.Green, Colors.Blue, LegendSteps/4);
            return f => nodeGroupColorSequence.ToUnitColor(f);
        }

        private const int LegendSteps = 512;
    }
}
