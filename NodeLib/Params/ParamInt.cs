using System;

namespace NodeLib.Params
{
    public class ParamInt : IParameter
    {
        public ParamInt(int min, int max, int value, string name, bool canChangeAtRunTime = true)
        {
            _min = min;
            _max = max;
            _value = value;
            _name = name;
            _canChangeAtRunTime = canChangeAtRunTime;
        }

        public ParamType ParamType
        {
            get { return ParamType.Int; }
        }

        public Type Type
        {
            get { return typeof(int); }
        }

        private readonly int _min;
        public int Min
        {
            get { return _min; }
        }

        private readonly int _max;
        public int Max
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

        private readonly bool _canChangeAtRunTime;
        public bool CanChangeAtRunTime
        {
            get { return _canChangeAtRunTime; }
        }
    }
}
