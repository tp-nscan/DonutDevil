using System;

namespace NodeLib.ParamsOld
{
    public class ParamBool : IParameter
    {
        public ParamBool(bool value, string name, bool canChangeAtRunTime)
        {
            Value = value;
            Name = name;
            CanChangeAtRunTime = canChangeAtRunTime;
        }

        public ParamType ParamType => ParamType.Bool;

        public Type Type => typeof(bool);

        public string Name { get; }

        public object Value { get; }

        public bool CanChangeAtRunTime { get; }
    }
}
