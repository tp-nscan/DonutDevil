using System.Linq;
using DonutDevilControls.ViewModel.Common;
using WpfUtils.BitmapGraphics;
using WpfUtils.Utils;

namespace DonutDevilControls.ViewModel.Design.Common
{
    public class DesignColorLegendVm2D : ColorLegendVm2D
    {
        public DesignColorLegendVm2D()
        {
            Title = "Designer title";
            MinValueX = -1.0;
            MaxValueX = 1.0;
            MinValueY = -1.0;
            MaxValueY = 1.0;

            var n1ColorSequence = ColorSequence.TriPolar(128);

            var x = 0;

            GraphicsInfos = n1ColorSequence
                .Colors
                .Select(
                    c => new GraphicsInfo
                        (
                        x: x % 128,
                        y: (x++)/128,
                        color: c
                        )).ToList();
        }

    }
}
