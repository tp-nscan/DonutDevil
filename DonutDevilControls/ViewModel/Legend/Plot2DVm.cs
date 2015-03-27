using WpfUtils;
using WpfUtils.ViewModels.Graphics;

namespace DonutDevilControls.ViewModel.Common
{
    public class Plot2DVm : NotifyPropertyChanged
    {
        public Plot2DVm(int width, int height)
        {
            _wbUniformGridVm = new WbUniformGridVm(width, height);
            
        }

        private readonly WbUniformGridVm _wbUniformGridVm;
        public WbUniformGridVm WbUniformGridVm
        {
            get { return _wbUniformGridVm; }
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
