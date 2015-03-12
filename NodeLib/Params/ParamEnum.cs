using System;

namespace NodeLib.Params
{
    public class ParamEnum : IParameter
    {
        public ParamEnum(Type type, string value, string name)
        {
            _type = type;
            _value = value;
            _name = name;
        }

        private readonly Type _type;
        public Type Type
        {
            get { return _type; }
        }

        private readonly object _value;
        public object Value
        {
            get { return _value; }
        }

        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }
    }
}
