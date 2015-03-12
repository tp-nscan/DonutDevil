using System;

namespace NodeLib.Params
{

    public interface IParameter
    {
        Type Type { get; }
        string Name { get; }
        object Value { get; }
    }
}
