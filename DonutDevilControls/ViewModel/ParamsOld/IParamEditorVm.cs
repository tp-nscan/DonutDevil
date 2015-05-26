using System;
using NodeLib.ParamsOld;

namespace DonutDevilControls.ViewModel.ParamsOld
{
    public static class ParamEditorVmExt
    {
        public static IParamEditorVm ToParamEditorVm(this IParameter parameter)
        {
            switch (parameter.ParamType)
            {
                    case ParamType.Bool:
                        return new ParamEditorBoolVm((ParamBool) parameter);
                    case ParamType.Enum:
                        return new ParamEditorEnumVm((ParamEnum) parameter);
                    case ParamType.Float:
                        return new ParamEditorFloatVm((ParamFloat) parameter);
                    case ParamType.Int:
                        return new ParamEditorIntVm((ParamInt) parameter);
                    default:
                        throw new Exception($"ParamsType {parameter.ParamType} not handled in ToParamEditorVm");
            }
        }

    }

    public interface IParamEditorVm
    {
        bool IsDirty { get; set; }
        ParamType ParamType { get; }
        string Title { get; }
        IParameter EditedValue { get; }
    }
}
