using DonutDevilControls.ViewModel.Params;
using NodeLib;
using NodeLib.Params;

namespace DonutDevilControls.ViewModel.Design.Params
{
    public class DesignParamSetEditorVm : ParamSetEditorVm
    {
        public DesignParamSetEditorVm()
        {
            ParamVms.Add(new ParamEditorFloatVm(new ParamFloat(0f, 1f, 0.1234f, "Float 1")));
            ParamVms.Add(new ParamEditorIntVm(new ParamInt(16, 166, 33, "Int1")));
            ParamVms.Add(new ParamEditorEnumVm(new ParamEnum(typeof(NeighborhoodType), NeighborhoodType.DoublePerimeter.ToString(), "Enum1")));
            ParamVms.Add(new ParamEditorBoolVm(new ParamBool(true, "Bool1")));
        }

    }
}
