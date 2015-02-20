using System.Linq;
using System.Windows.Media;
using DonutDevilControls.ViewModel.Common;
using WpfUtils.BitmapGraphics;
using WpfUtils.Utils;

namespace DonutDevilControls.ViewModel.Design.Common
{
    public class DesignColorLegendVm : ColorLegendVm
    {
        public DesignColorLegendVm()
        {
            Title = "Designer title";
            MinValue = -1.0;
            MaxValue = 1.0;

            var n1ColorSequence = ColorSequence.Dipolar(Colors.Red, Colors.Blue, 128);

            var x = 0;

            GraphicsInfos = n1ColorSequence
                .Colors
                .Select(
                    c => new GraphicsInfo
                        (
                        x: x++,
                        y: 0,
                        color: c
                        )).ToList();
        }
    }
}
