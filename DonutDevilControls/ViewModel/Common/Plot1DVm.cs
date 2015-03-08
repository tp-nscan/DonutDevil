using System.Collections.Generic;
using WpfUtils;
using WpfUtils.Views.Graphics;

namespace DonutDevilControls.ViewModel.Common
{
    public class Plot1DVm : NotifyPropertyChanged
    {
        private IReadOnlyList<PlotPoint> _graphicsInfos = new List<PlotPoint>();
        public IReadOnlyList<PlotPoint> GraphicsInfos
        {
            get { return _graphicsInfos; }
            set
            {
                _graphicsInfos = value;
                OnPropertyChanged("GraphicsInfos");
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
