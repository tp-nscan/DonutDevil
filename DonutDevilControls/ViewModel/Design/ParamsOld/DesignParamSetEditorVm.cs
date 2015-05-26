using System.Collections.Generic;
using System.Linq;
using DonutDevilControls.ViewModel.ParamsOld;
using NodeLib.Common;
using NodeLib.ParamsOld;

namespace DonutDevilControls.ViewModel.Design.ParamsOld
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
