using WpfUtils;
using WpfUtils.ViewModels.Graphics;

namespace DonutDevilControls.ViewModel.Legend
{
    public class Plot2DVm : NotifyPropertyChanged
    {
        public Plot2DVm(int width, int height)
        {
            WbUniformGridVm = new WbFullUniformGridVm(width, height);
        }

        public WbFullUniformGridVm WbUniformGridVm { get; }

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

        private string _titleX;
        public string TitleX
        {
            get { return _titleX; }
            set
            {
                _titleX = value;
                OnPropertyChanged("TitleX");
            }
        }

        private string _titleY;
        public string TitleY
        {
            get { return _titleY; }
            set
            {
                _titleY = value;
                OnPropertyChanged("TitleY");
            }
        }


    }
}
