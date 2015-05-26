using System;

namespace NodeLib.ParamsOld
{
    public class ParamFloat : IParameter
    {
        public ParamFloat(float min, float max, float value, string name, bool canChangeAtRunTime)
        {
            Min = min;
            Max = max;
            Value = value;
            Name = name;
            CanChangeAtRunTime = canChangeAtRunTime;
        }

        public ParamType ParamType => ParamType.Float;

        public Type Type { get; } = typeof(float);

        public float Min { get; }

        public float Max { get; }

        public object Value { get; }

        public string Name { get; }

        public bool CanChangeAtRunTime { get; }
    }
}