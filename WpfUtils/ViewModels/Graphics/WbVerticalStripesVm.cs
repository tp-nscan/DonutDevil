using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using LibNode;
using MathLib.NumericTypes;
using WpfUtils.Views.Graphics;

namespace WpfUtils.ViewModels.Graphics
{
    public class WbVerticalStripesVm : WbImageVm
    {
        public WbVerticalStripesVm(int stripeCount, double heightOverWidth, int crispness)
            : base(crispness * stripeCount, (int) (crispness * stripeCount * heightOverWidth))
        {
            StripeCount = stripeCount;
            Crispness = crispness;
        }

        public int StripeCount { get; }

        public int Crispness { get; }

        public void AddValues(IEnumerable<D1Val<Color>> stripeVals)
        {
            PlotRectangleList.Clear();

            PlotRectangleList = stripeVals.Select(
                gv =>
                    new PlotRectangle
                       (
                            x: gv.Index * Crispness,
                            y: 0,
                            width: Crispness,
                            height: ImageHeight,
                            color: gv.Val
                       )
                    ).ToList();

            OnPropertyChanged("PlotRectangles");
        }
    }
}
