using System.Collections.Generic;
using WpfUtils;
using WpfUtils.Views.Graphics;

namespace DonutDevilControls.ViewModel.Common
{
    public class Plot2DVm : NotifyPropertyChanged
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


        private double _minValueX;
        public double MinValueX
        {
            get { return _minValueX; }
            set
            {
                _minValueX = value;
                OnPropertyChanged("MinValueX");
            }
        }

        private double _maxValueX;
        public double MaxValueX
        {
            get { return _maxValueX; }
            set
            {
                _maxValueX = value;
                OnPropertyChanged("MaxValueX");
            }
        }

        private double _minValueY;
        public double MinValueY
        {
            get { return _minValueY; }
            set
            {
                _minValueY = value;
                OnPropertyChanged("MinValueY");
            }
        }

        private double _maxValueY;
        public double MaxValueY
        {
            get { return _maxValueY; }
            set
            {
                _maxValueY = value;
                OnPropertyChanged("MaxValueY");
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
