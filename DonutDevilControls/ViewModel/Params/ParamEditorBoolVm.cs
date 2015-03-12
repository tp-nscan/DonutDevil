﻿using WpfUtils;

namespace DonutDevilControls.ViewModel.Params
{
    public class ParamEditorBoolVm : NotifyPropertyChanged, IParamVm
    {
        public ParamEditorBoolVm(string name)
        {
            _name = name;
        }

        public ParamType ParamType
        {
            get { return ParamType.Bool; }
        }

        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }

        private bool _value;
        public bool Value
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
