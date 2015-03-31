using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using MathLib.NumericTypes;
using WpfUtils.Views.Graphics;

namespace WpfUtils.ViewModels.Graphics
{
    public class WbVerticalStripesVm : WbImageVm
    {
        public WbVerticalStripesVm(int stripeCount, double heightOverWidth)
            : base(2 * stripeCount, (int) (2 * stripeCount * heightOverWidth))
        {
            
        }

        public void AddValues(IEnumerable<D1Val<Color>> ringVals)
        {
            PlotRectangleList.Clear();

            PlotRectangleList = ringVals.Select(
                gv =>
                    new PlotRectangle
                       (
                            x: gv.Index * 2,
                            y: 0,
                            width: 2,
                            height: ImageHeight,
                            color: gv.Value
                       )
                    ).ToList();

            OnPropertyChanged("PlotRectangles");
        }
    }
}
