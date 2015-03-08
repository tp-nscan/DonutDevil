using System;
using System.Windows.Media;
using WpfUtils;
using WpfUtils.ViewModels.Graphics;

namespace DonutDevilControls.ViewModel.Common
{
    public class Plot1DVm : NotifyPropertyChanged
    {
        public Plot1DVm(int cellDimX, double heightOverWidth, Func<float, Color> colorMap)
        {
            _wbVerticalStripesVm = new WbVerticalStripesVm(cellDimX, heightOverWidth, colorMap);
        }

        private WbVerticalStripesVm _wbVerticalStripesVm;
        public WbVerticalStripesVm WbVerticalStripesVm
        {
            get { return _wbVerticalStripesVm; }
            set
            {
                _wbVerticalStripesVm = value;
                OnPropertyChanged("WbVerticalStripesVm");
            }
        }

        private double _minValue;
        public double MinValue
        {
            get { return _minValue; }
            set
            {
                _minValue = value;
                OnPropertyChanged("MinValue");
            }
        }

        private double _maxValue;
        public double MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                OnPropertyChanged("MaxValue");
            }
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
