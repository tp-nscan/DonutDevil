using System.Windows.Media;
using WpfUtils;
using WpfUtils.Utils;
using WpfUtils.ViewModels.Graphics;

namespace DonutDevilControls.ViewModel.Legend
{
    public class TorusHistogramVm : NotifyPropertyChanged, IHistogramVm
    {
        private const int Colorsteps = 512;

        public TorusHistogramVm(string title)
        {
            _title = title;
            _legendVm = new WbRingPlotVm(80);
            _histogramVm = new WbRingPlotVm(165);

            _histogramColorSequence = Colors.White.ToUniformColorSequence(Colorsteps);
        }

        private readonly IColorSequence _histogramColorSequence;

        public DisplaySpaceType DisplaySpaceType
        {
            get { return DisplaySpaceType.Torus; }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }


        private readonly WbRingPlotVm _legendVm;
        public WbRingPlotVm LegendVm
        {
            get { return _legendVm; }
        }

        private readonly WbRingPlotVm _histogramVm;
        public WbRingPlotVm HistogramVm
        {
            get { return _histogramVm; }
        }
    }
}
