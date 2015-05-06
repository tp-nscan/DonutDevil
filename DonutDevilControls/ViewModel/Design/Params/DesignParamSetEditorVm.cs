using System.Linq;
using System.Collections.Generic;
using DonutDevilControls.ViewModel.Params;
using NodeLib;
using NodeLib.Params;

namespace DonutDevilControls.ViewModel.Design.Params
{
    public class DesignParamSetEditorVm : ParamSetEditorVm
    {
        public DesignParamSetEditorVm() : base(DesignParams.ToList(), true)
        {
        }

        public static IEnumerable<IParameter> DesignParams
        {
            get
            {
                yield return new ParamFloat(0f, 1f, 0.1234f, "Float 1", true);
                yield return new ParamInt(16, 166, 33, "Int1", true);
                yield return new ParamEnum(typeof(NeighborhoodType), NeighborhoodType.DoublePerimeter.ToString(), "Enum1", true);
                yield return new ParamBool(true, "Bool1", true);
            }

        }

    }
}
