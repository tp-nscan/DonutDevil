using System;

namespace NodeLib.ParamsOld
{
    public class ParamEnum : IParameter
    {
        public ParamEnum(Type type, string value, string name, bool canChangeAtRunTime)
        {
            Type = type;
            Value = value;
            Name = name;
            CanChangeAtRunTime = canChangeAtRunTime;
        }

        public ParamType ParamType { get; } = ParamType.Enum;

        public Type Type { get; }

        public object Value { get; }

        public string Name { get; }

        public bool CanChangeAtRunTime { get; }
    }
}
