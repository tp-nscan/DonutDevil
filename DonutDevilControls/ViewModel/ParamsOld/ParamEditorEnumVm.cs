using System;
using System.Collections.ObjectModel;
using NodeLib.ParamsOld;
using WpfUtils;

namespace DonutDevilControls.ViewModel.ParamsOld
{
    public class ParamEditorEnumVm : NotifyPropertyChanged, IParamEditorVm
    {
        public ParamEditorEnumVm(ParamEnum paramEnum)
        {
            _paramEnum = paramEnum;
            Value = _paramEnum.Value.ToString();
            _enumValues = new ObservableCollection<string>(Enum.GetNames(paramEnum.Type));
        }

        private bool _isDirty;
        public bool IsDirty
        {
            get { return _isDirty; }
            set { _isDirty = value; }
        }

        public ParamType ParamType
        {
            get { return ParamType.Enum; }
        }

        public string Title
        {
            get { return _paramEnum.Name; }
        }

        private readonly ParamEnum _paramEnum;

        private string _value;
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                _isDirty = (value != (string) _paramEnum.Value);
                OnPropertyChanged("Value");
            }
        }

        private ObservableCollection<string> _enumValues;
        public ObservableCollection<string> EnumValues
        {
            get { return _enumValues; }
            set
            {
                _enumValues = value;
                OnPropertyChanged("EnumValues");
            }
        }


        public IParameter EditedValue
        {
            get { return new ParamEnum(_paramEnum.Type, Value, _paramEnum.Name, _paramEnum.CanChangeAtRunTime); }
        }
    }
}
