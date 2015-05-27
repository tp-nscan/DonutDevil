using NodeLib.ParamsOld;
using WpfUtils;

namespace DonutDevilControls.ViewModel.ParamsOld
{
    public class ParamEditorBoolVm : NotifyPropertyChanged, IParamEditorVm
    {
        public ParamEditorBoolVm(ParamBool paramBool)
        {
            _paramBool = paramBool;
            Value = (bool)_paramBool.Value;
        }

        public bool IsDirty { get; set; }

        public ParamType ParamType => ParamType.Bool;

        public string Title => _paramBool.Name;

        private readonly ParamBool _paramBool;

        private bool _value;
        public bool Value
        {
            get { return _value; }
            set
            {
                _value = value;
                IsDirty =  (bool)_paramBool.Value != _value; 
                OnPropertyChanged("Value");
            }
        }

        public IParameter EditedValue => new ParamBool(Value, _paramBool.Name, _paramBool.CanChangeAtRunTime);
    }
}
