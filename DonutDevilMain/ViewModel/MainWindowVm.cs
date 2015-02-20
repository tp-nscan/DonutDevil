using System;
using DonutDevilControls.ViewModel.Common;
using WpfUtils;

namespace DonutDevilMain.ViewModel
{
    public class MainWindowVm : NotifyPropertyChanged
    {
        public MainWindowVm()
        {
            _imageLegendVm = new ImageLegendVm();
            _imageLegendVm.OnPixelsChanged.Subscribe(OnPixelsChanged);
        }

        private readonly ImageLegendVm _imageLegendVm;
        public ImageLegendVm ImageLegendVm
        {
            get { return _imageLegendVm; }
        }

        void OnPixelsChanged(int[,] pixels)
        {
            
        }

    }
}
