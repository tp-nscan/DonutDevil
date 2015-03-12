using DonutDevilControls.ViewModel.Params;

namespace DonutDevilControls.ViewModel.Design.Params
{
    public class DesignParamsVm : ParamsVm
    {
        public DesignParamsVm()
        {
            ParamVms.Add(new ParamEditorFloatVm("Float1"));
            ParamVms.Add(new ParamEditorIntVm("Int1"));
            ParamVms.Add(new ParamEditorEnumVm("Enum1"));
            ParamVms.Add(new ParamEditorBoolVm("Bool1"));
        }

    }
}
