using WpfUtils;

namespace La.ViewModel
{
    public class ParamBoolVm : NotifyPropertyChanged
    {
        public ParamBoolVm(bool curVal, string name)
        {
            _curVal = curVal;
            Name = name;
        }

        private bool _curVal;
        public bool CurVal
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

        public string Name { get; }
    }
}
