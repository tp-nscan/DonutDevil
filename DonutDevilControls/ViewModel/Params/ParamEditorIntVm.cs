using WpfUtils;

namespace DonutDevilControls.ViewModel.Params
{
    public class ParamEditorIntVm: NotifyPropertyChanged, IParamVm
    {
        public ParamEditorIntVm(string name)
        {
            _name = name;
        }

        public ParamType ParamType
        {
            get { return ParamType.Int; }
        }

        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }

        private int _value;
        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }
    }
}
