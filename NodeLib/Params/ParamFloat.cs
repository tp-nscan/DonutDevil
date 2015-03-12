using System;

namespace NodeLib.Params
{
    public class ParamFloat : IParameter
    {
        public ParamFloat(float min, float max, float value, string name)
        {
            _min = min;
            _max = max;
            _value = value;
            _name = name;
        }

        public Type Type
        {
            get { return typeof(float); }
        }

        private readonly float _min;
        public float Min
        {
            get { return _min; }
        }

        private readonly float _max;
        public float Max
        {
            get { return _max; }
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