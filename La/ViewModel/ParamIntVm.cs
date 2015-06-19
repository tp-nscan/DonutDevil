using WpfUtils;

namespace La.ViewModel
{
    public class ParamIntVm : NotifyPropertyChanged
    {
        public ParamIntVm(int minVal, int maxVal, int curVal, string name)
        {
            _curVal = curVal;
            MinVal = minVal;
            MaxVal = maxVal;
            Name = name;
        }

        private int _curVal;
        public int CurVal
        {
            get { return _curVal; }
            set
            {
                _curVal = value;
                OnPropertyChanged("CurVal");
                IsDirty = true;
            }
        }
        
        public bool IsDirty { get; private set; }

        public int MaxVal { get; }

        public int MinVal { get; }

        public string Name { get; }

    }
}
