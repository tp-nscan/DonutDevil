﻿using System;

namespace NodeLib.ParamsOld
{
    public enum ParamType
    {
        Bool,
        Int,
        Float,
        Enum
    }

    public interface IParameter
    {
        ParamType ParamType { get; }
        Type Type { get; }
        string Name { get; }
        object Value { get; }
        bool CanChangeAtRunTime { get; }
    }
}
