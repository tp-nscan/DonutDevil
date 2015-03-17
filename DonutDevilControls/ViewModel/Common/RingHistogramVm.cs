using WpfUtils;
using WpfUtils.ViewModels.Graphics;

namespace DonutDevilControls.ViewModel.Common
{
    public class RingHistogramVm : NotifyPropertyChanged
    {
        public RingHistogramVm(string title)
        {
            _title = title;
            _legendVm = new WbRingPlotVm(80);
            _histogramVm = new WbRingPlotVm(165);
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
    }


}
