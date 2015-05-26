using System;

namespace NodeLib.ParamsOld
{
    public class ParamInt : IParameter
    {
        public ParamInt(int min, int max, int value, string name, bool canChangeAtRunTime)
        {
            Min = min;
            Max = max;
            Value = value;
            Name = name;
            CanChangeAtRunTime = canChangeAtRunTime;
        }

        public ParamType ParamType => ParamType.Int;

        public Type Type => typeof(int);

        public int Min { get; }

        public int Max { get; }

        public object Value { get; }

        public string Name { get; }

        public bool CanChangeAtRunTime { get; }
    }
}
