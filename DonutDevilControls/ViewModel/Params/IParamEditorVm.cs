using System;
using NodeLib.Params;

namespace DonutDevilControls.ViewModel.Params
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
                        throw new Exception(String.Format("ParamsType {0} not handled in ToParamEditorVm", parameter.ParamType));
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
