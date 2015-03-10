using DonutDevilControls.ViewModel.Design.Common;
using WpfUtils;

namespace DonutDevilMain.ViewModel
{
    public class TestWbPlotWindowVm : NotifyPropertyChanged
    {
        public TestWbPlotWindowVm()
        {
            DesignRingHistogramVm = new DesignRingHistogramVm();
        }

        public DesignRingHistogramVm DesignRingHistogramVm { get; set; }
    }
}
