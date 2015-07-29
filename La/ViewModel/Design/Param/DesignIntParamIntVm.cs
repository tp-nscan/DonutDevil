using La.ViewModel.Pram;

namespace La.ViewModel.Design.Param
{
    public class DesignParamIntVm : ParamIntVm
    {
        public DesignParamIntVm() 
            : base(minVal:1, maxVal:100, curVal:42, name:"Design Int name")
        {
        }
    }
}
