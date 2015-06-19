using WpfUtils;

namespace La.ViewModel
{
    public class ParamDoubleVm : NotifyPropertyChanged
    {
        public ParamDoubleVm(double minVal, double maxVal, 
                             double curVal, string name,
                             string formatString)
        {
            _curVal = curVal;
            FormatString = formatString;
            MinVal = minVal;
            MaxVal = maxVal;
            Name = name;
        }

        private double _curVal;
        public double CurVal
        {
            get { return _curVal; }
            set
            {
                _curVal = value;
                OnPropertyChanged("CurVal");
                IsDirty = true;
            }
        }

        public string FormatString { get; }

        public bool IsDirty { get; private set; }

        public double MaxVal { get; }

        public double MinVal { get; }

        public string Name { get; }
    }
}
