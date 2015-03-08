using System.Collections.Generic;
using System.Linq;
using WpfUtils.Utils;
using WpfUtils.Views.Graphics;

namespace DonutDevilMain.ViewModel.Design
{
    public class DesignTestWbPlotWindowVm : TestWbPlotWindowVm
    {
        public DesignTestWbPlotWindowVm()
        {

            var n1ColorSequence = ColorSequence.TriPolar(2);

            var x = 0;

            PlotRectangles = n1ColorSequence
                .Colors
                .Select(
                    c => new PlotRectangle
                        (
                            x: x % 2,
                            y: (x++) / 2,
                            width: 10,
                            height: 10,
                            color: c
                        )
                  ).ToList();
        }

        private IReadOnlyList<PlotRectangle> _plotRectangles = new List<PlotRectangle>();
        public IReadOnlyList<PlotRectangle> PlotRectangles
        {
            get { return _plotRectangles; }
            set
            {
                _plotRectangles = value;
                OnPropertyChanged("GraphicsInfos");
            }
        }

    }
}
