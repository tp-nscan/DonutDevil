using System;

namespace NodeLib.Params
{
    public class ParamBool : IParameter
    {
        public ParamBool(bool value, string name, bool canChangeAtRunTime = true)
        {
            _value = value;
            _name = name;
            _canChangeAtRunTime = canChangeAtRunTime;
        }

        public ParamType ParamType
        {
            get { return ParamType.Bool; }
        }

        public Type Type
        {
            get { return typeof(bool); }
        }

        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }

        private readonly object _value;
        public object Value
        {
            get { return _value; }
        }

        private readonly bool _canChangeAtRunTime;
        public bool CanChangeAtRunTime
        {
            get { return _canChangeAtRunTime; }
        }
    }
}
