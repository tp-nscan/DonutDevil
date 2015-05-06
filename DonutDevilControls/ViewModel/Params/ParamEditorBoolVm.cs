using NodeLib.Params;
using WpfUtils;

namespace DonutDevilControls.ViewModel.Params
{
    public class ParamEditorBoolVm : NotifyPropertyChanged, IParamEditorVm
    {
        public ParamEditorBoolVm(ParamBool paramBool)
        {
            _paramBool = paramBool;
            Value = (bool)_paramBool.Value;
        }

        private bool _isDirty;
        public bool IsDirty
        {
            get { return _isDirty; }
            set { _isDirty = value; }
        }

        public ParamType ParamType
        {
            get { return ParamType.Bool; }
        }

        public string Title
        {
            get { return _paramBool.Name; }
        }

        private readonly ParamBool _paramBool;

        private bool _value;
        public bool Value
        {
            get { return _value; }
            set
            {
                _value = value;
                _isDirty =  (bool)_paramBool.Value != _value; 
                OnPropertyChanged("Value");
            }
        }

        public IParameter EditedValue
        {
            get { return new ParamBool(Value, _paramBool.Name, _paramBool.CanChangeAtRunTime); }
        }
    }
}
