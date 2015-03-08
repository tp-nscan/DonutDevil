using System.Collections.Generic;
using System.Linq;
using WpfUtils;
using WpfUtils.Utils;
using WpfUtils.Views.Graphics;

namespace DonutDevilMain.ViewModel
{
    public class TestWbPlotWindowVm : NotifyPropertyChanged
    {
        public TestWbPlotWindowVm()
        {
            PanelPlotRectangles =
            Enumerable.Range(0, 40000)
                .Select(i => new PlotRectangle(i%100, i/100, 10, 10, i.IntToColor()))
                .ToList();
        }

        private IReadOnlyList<PlotRectangle> _panelPlotRectangles;
        public IReadOnlyList<PlotRectangle> PanelPlotRectangles
        {
            get { return _panelPlotRectangles; }
            set
            {
                _panelPlotRectangles = value;
                OnPropertyChanged("PanelPlotRectangles");
            }
        }
    }
}
