using WpfUtils;

namespace La.ViewModel.Pram
{
    public class ParamDoubleVm : NotifyPropertyChanged, IPramVm
    {
        public ParamDoubleVm(double minVal, double maxVal, 
                             double curVal, string name,
                             string formatString, double increment)
        {
            _curVal = curVal;
            FormatString = formatString;
            Increment = increment;
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

        public void Clean()
        {
            IsDirty = false;
        }

        public string FormatString { get; }

        public double Increment { get; }

        public bool IsDirty { get; private set; }

        public double MaxVal { get; }

        public double MinVal { get; }

        public string Name { get; }
    }
}
